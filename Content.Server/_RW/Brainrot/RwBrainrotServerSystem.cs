using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Shared._RW.Brainrot;
using Robust.Server.Player;
using Robust.Shared.Player;

namespace Content.Server._RW.Brainrot;

public sealed class RwBrainrotServerSystem : EntitySystem
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private readonly List<string> _customTriggers = new();

    public IReadOnlyList<string> CustomTriggers => _customTriggers;

    public override async void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerJoinedLobbyEvent>(OnPlayerJoinedLobby);

        // Load custom triggers from database
        try
        {
            var triggers = await _db.GetBrainrotTriggersAsync();
            _customTriggers.Clear();
            _customTriggers.AddRange(triggers);
            // Sync to any players who joined while the DB query was in flight
            RaiseNetworkEvent(new SyncBrainrotTriggersMessage(new List<string>(_customTriggers)));
        }
        catch (Exception e)
        {
            Log.Error($"Failed to load custom brainrot triggers from DB: {e}");
        }
    }

    private void OnPlayerJoinedLobby(PlayerJoinedLobbyEvent ev)
    {
        var session = ev.PlayerSession;
        RaiseNetworkEvent(new SyncBrainrotTriggersMessage(new List<string>(_customTriggers)), session);
    }

    public async Task AddTrigger(string trigger)
    {
        var normalized = trigger.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalized))
            return;

        if (_customTriggers.Contains(normalized))
            return;

        try
        {
            await _db.AddBrainrotTriggerAsync(normalized);
            _customTriggers.Add(normalized);
            RaiseNetworkEvent(new SyncBrainrotTriggersMessage(new List<string>(_customTriggers)));
        }
        catch (Exception e)
        {
            Log.Error($"Failed to add custom brainrot trigger '{normalized}' to DB: {e}");
        }
    }

    public async Task RemoveTrigger(string trigger)
    {
        var normalized = trigger.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalized))
            return;

        if (!_customTriggers.Contains(normalized))
            return;

        try
        {
            await _db.RemoveBrainrotTriggerAsync(normalized);
            _customTriggers.Remove(normalized);
            RaiseNetworkEvent(new SyncBrainrotTriggersMessage(new List<string>(_customTriggers)));
        }
        catch (Exception e)
        {
            Log.Error($"Failed to remove custom brainrot trigger '{normalized}' from DB: {e}");
        }
    }
}
