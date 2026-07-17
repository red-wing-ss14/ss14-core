using Content.Server.Chat.Systems;
using Content.Shared.Mind;
using Content.Goobstation.Common.LastWords;
using Content.Shared.Chat;
using Content.Shared.Mobs.Components;

namespace Content.Goobstation.Server.LastWords;

public sealed class LastWordsSystem : EntitySystem
{
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<MobStateComponent, Content.Server.Chat.Systems.EntitySpokeEvent>(OnEntitySpoke);
    }

    private void OnEntitySpoke(EntityUid uid, MobStateComponent _, Content.Server.Chat.Systems.EntitySpokeEvent args)
    {
        _mindSystem.TryGetMind(uid, out var mindId, out var _);

        if (TryComp<LastWordsComponent>(mindId, out var lastWordsComp))
            lastWordsComp.LastWords = args.Message;
    }
}
