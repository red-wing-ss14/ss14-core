using Content.Shared._RW.Brainrot;

namespace Content.Client._RW.Brainrot;

public sealed class RwBrainrotClientSystem : EntitySystem
{
    private readonly List<string> _customTriggers = new();

    public IReadOnlyList<string> CustomTriggers => _customTriggers;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<SyncBrainrotTriggersMessage>(OnSyncTriggers);
    }

    private void OnSyncTriggers(SyncBrainrotTriggersMessage msg)
    {
        _customTriggers.Clear();
        _customTriggers.AddRange(msg.Triggers);
    }
}
