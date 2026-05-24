using Content.Server._Amour.Ghost.Roles.Components;
using Content.Server._Amour.Ghost.Roles.UI;
using Content.Server.EUI;
using Content.Server.Ghost.Roles;
using Content.Server.Ghost.Roles.Components;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Content.Shared.SSDIndicator;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Amour.Ghost.Roles;

public sealed class SsdAmnesiacGhostRoleSystem : EntitySystem
{
    private static readonly TimeSpan DisconnectedDelay = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    [Dependency] private readonly EuiManager _eui = default!;
    [Dependency] private readonly GhostRoleSystem _ghostRole = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedJobSystem _jobs = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    private readonly HashSet<NetUserId> _pendingBodyTakenNotices = new();
    private TimeSpan _nextUpdate;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
        SubscribeLocalEvent<PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<SsdAmnesiacGhostRoleComponent, MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<SsdAmnesiacGhostRoleComponent, TakeGhostRoleEvent>(OnTakeGhostRole,
            after: new[] { typeof(GhostRoleSystem) });

        _player.PlayerStatusChanged += OnPlayerStatusChanged;
    }

    public override void Shutdown()
    {
        base.Shutdown();

        _player.PlayerStatusChanged -= OnPlayerStatusChanged;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        if (curTime < _nextUpdate)
            return;

        _nextUpdate = curTime + UpdateInterval;

        var query = EntityQueryEnumerator<SsdAmnesiacGhostRoleComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.Taken || component.RoleCreated || component.MakeAvailableAt > curTime)
                continue;

            if (_player.TryGetSessionById(component.OriginalUserId, out var session) &&
                session.Status == SessionStatus.InGame &&
                IsOriginalAttachedToBody((uid, component), session.AttachedEntity))
            {
                RemoveGhostRole((uid, component));
                continue;
            }

            TryCreateGhostRole((uid, component));
        }
    }

    public bool TryCreateImmediateGhostRole(EntityUid body, EntityUid mindId, MindComponent mind, ICommonSession player)
    {
        if (mind.UserId != player.UserId)
            return false;

        if (!TryStartTracking(body, mindId, mind, player.UserId, _timing.CurTime, out var tracked))
            return false;

        if (tracked is not { } role)
            return false;

        return TryCreateGhostRole(role);
    }

    private void OnRoundRestart(RoundRestartCleanupEvent ev)
    {
        _pendingBodyTakenNotices.Clear();
    }

    private void OnPlayerAttached(PlayerAttachedEvent args)
    {
        HandleOriginalReturned(args.Player, args.Entity);
    }

    private void OnMobStateChanged(Entity<SsdAmnesiacGhostRoleComponent> ent, ref MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Alive || ent.Comp.Taken)
            return;

        RemoveGhostRole(ent);
    }

    private void OnTakeGhostRole(Entity<SsdAmnesiacGhostRoleComponent> ent, ref TakeGhostRoleEvent args)
    {
        if (!args.TookRole)
            return;

        ent.Comp.Taken = true;
        _pendingBodyTakenNotices.Add(ent.Comp.OriginalUserId);
        TryShowBodyTakenNotice(ent.Comp.OriginalUserId);
        RemCompDeferred<SsdAmnesiacGhostRoleComponent>(ent.Owner);
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs args)
    {
        switch (args.NewStatus)
        {
            case SessionStatus.Disconnected:
            case SessionStatus.Zombie:
                TryTrackDisconnectedBody(args.Session);
                break;
            case SessionStatus.InGame:
                HandleOriginalReturned(args.Session, args.Session.AttachedEntity);
                break;
        }
    }

    private void TryTrackDisconnectedBody(ICommonSession session)
    {
        if (session.AttachedEntity is not { Valid: true } body)
            return;

        if (!_mind.TryGetMind(body, out var mindId, out var mind) ||
            mind.UserId != session.UserId)
            return;

        TryStartTracking(body, mindId, mind, session.UserId, _timing.CurTime + DisconnectedDelay, out _);
    }

    private void HandleOriginalReturned(ICommonSession session, EntityUid? attachedEntity)
    {
        if (_pendingBodyTakenNotices.Contains(session.UserId))
        {
            TryShowBodyTakenNotice(session.UserId);
            return;
        }

        var query = EntityQueryEnumerator<SsdAmnesiacGhostRoleComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (component.OriginalUserId != session.UserId)
                continue;

            if (component.Taken)
            {
                _pendingBodyTakenNotices.Add(session.UserId);
                TryShowBodyTakenNotice(session.UserId);
                continue;
            }

            if (!IsOriginalAttachedToBody((uid, component), attachedEntity))
                continue;

            RemoveGhostRole((uid, component));
        }
    }

    private static bool IsOriginalAttachedToBody(Entity<SsdAmnesiacGhostRoleComponent> ent, EntityUid? attachedEntity)
    {
        return attachedEntity == ent.Owner;
    }

    private bool TryStartTracking(
        EntityUid body,
        EntityUid mindId,
        MindComponent mind,
        NetUserId userId,
        TimeSpan makeAvailableAt,
        out Entity<SsdAmnesiacGhostRoleComponent>? tracked)
    {
        tracked = null;

        if (!CanUseBody(body, mindId, mind, userId, out var jobId, out var jobName))
            return false;

        if (TryComp(body, out SsdAmnesiacGhostRoleComponent? component))
        {
            if (component.Taken || component.RoleCreated)
                return false;
        }
        else if (HasComp<GhostRoleComponent>(body) || HasComp<GhostTakeoverAvailableComponent>(body))
        {
            return false;
        }
        else
        {
            component = EnsureComp<SsdAmnesiacGhostRoleComponent>(body);
        }

        component.OriginalUserId = userId;
        component.OriginalMind = mindId;
        component.MakeAvailableAt = makeAvailableAt;
        component.Job = jobId;
        component.JobName = jobName;
        tracked = (body, component);
        return true;
    }

    private bool TryCreateGhostRole(Entity<SsdAmnesiacGhostRoleComponent> ent)
    {
        if (ent.Comp.RoleCreated || ent.Comp.Taken)
            return false;

        if (!TryComp(ent.Comp.OriginalMind, out MindComponent? mind) ||
            !CanUseBody(ent.Owner, ent.Comp.OriginalMind, mind, ent.Comp.OriginalUserId, out var jobId, out var jobName))
        {
            RemoveGhostRole(ent);
            return false;
        }

        if (TryComp(ent.Owner, out MindContainerComponent? container) &&
            container.Mind is { } currentMind &&
            currentMind != ent.Comp.OriginalMind)
        {
            RemoveGhostRole(ent);
            return false;
        }

        var takeover = EnsureComp<GhostTakeoverAvailableComponent>(ent.Owner);
        takeover.IgnoreMindCheck = true;

        var role = EnsureComp<GhostRoleComponent>(ent.Owner);
        role.RoleName = Loc.GetString("amour-ssd-amnesiac-ghost-role-name", ("job", jobName));
        role.RoleDescription = Loc.GetString("amour-ssd-amnesiac-ghost-role-description", ("job", jobName));
        role.RoleRules = Loc.GetString("amour-ssd-amnesiac-ghost-role-rules");
        role.ReregisterOnGhost = false;
        role.MakeSentient = true;
        role.AllowMovement = true;
        role.AllowSpeech = true;
        role.JobProto = jobId;
        _ghostRole.SetTaken(role, false);

        ent.Comp.RoleCreated = true;
        ent.Comp.Job = jobId;
        ent.Comp.JobName = jobName;
        return true;
    }

    private bool CanUseBody(
        EntityUid body,
        EntityUid mindId,
        MindComponent mind,
        NetUserId userId,
        out ProtoId<JobPrototype>? jobId,
        out string jobName)
    {
        jobId = null;
        jobName = string.Empty;

        if (!Exists(body) || Deleted(body) || Terminating(body))
            return false;

        if (HasComp<GhostComponent>(body) || !HasComp<SSDIndicatorComponent>(body))
            return false;

        if (!_mobState.IsAlive(body))
            return false;

        if (mind.UserId != userId)
            return false;

        if (!_jobs.MindTryGetJobId(mindId, out jobId) || jobId == null)
            return false;

        _jobs.MindTryGetJobName(mindId, out jobName);
        return true;
    }

    private void RemoveGhostRole(Entity<SsdAmnesiacGhostRoleComponent> ent)
    {
        if (TryComp(ent.Owner, out GhostRoleComponent? role))
            _ghostRole.UnregisterGhostRole((ent.Owner, role));

        RemCompDeferred<GhostRoleComponent>(ent.Owner);
        RemCompDeferred<GhostTakeoverAvailableComponent>(ent.Owner);
        RemCompDeferred<SsdAmnesiacGhostRoleComponent>(ent.Owner);
    }

    private bool TryShowBodyTakenNotice(NetUserId userId)
    {
        if (!_player.TryGetSessionById(userId, out var session) ||
            session.Status != SessionStatus.InGame)
            return false;

        _pendingBodyTakenNotices.Remove(userId);
        _eui.OpenEui(new SsdAmnesiacGhostRoleReturnEui(), session);
        return true;
    }
}
