using Content.Server.GameTicking.Events;
using Content.Shared.Roles;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._Amour.Discord;

public sealed class DiscordLinkRequirementSystem : EntitySystem
{
    [Dependency] private readonly IDiscordLinkChecker _discordLinkChecker = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IsRoleAllowedEvent>(OnIsRoleAllowed);
    }

    private void OnIsRoleAllowed(ref IsRoleAllowedEvent ev)
    {
        if (ev.Jobs == null)
            return;

        var roleSystem = EntityManager.System<SharedRoleSystem>();

        foreach (var jobId in ev.Jobs)
        {
            if (!_prototypeManager.TryIndex<JobPrototype>(jobId, out var job))
                continue;

            var requirements = roleSystem.GetRoleRequirements(job);
            if (requirements == null)
                continue;

            foreach (var requirement in requirements)
            {
                if (requirement is not Content.Shared._Amour.Discord.DiscordLinkRequirement { Inverted: false })
                    continue;

                if (!_discordLinkChecker.IsDiscordLinkedCached(ev.Player.UserId))
                {
                    ev.Cancelled = true;
                    _ = EntityManager.System<DiscordLinkSystem>().SendLinkStatus(ev.Player);
                }

                return;
            }
        }
    }
}
