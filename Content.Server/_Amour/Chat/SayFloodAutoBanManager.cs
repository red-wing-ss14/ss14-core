using System.Collections.Generic;
using System.Threading.Tasks;
using Content.Server.Administration;
using Content.Server.Administration.Managers;
using Content.Shared.Database;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server._Amour.Chat;

public sealed class SayFloodAutoBanManager
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IBanManager _banManager = default!;
    [Dependency] private readonly IPlayerLocator _locator = default!;

    private const int WindowSeconds = 60;
    private const int LimitPerWindow = 70;
    private const string BanReason = "Твинк";
    private const string BanningAdminName = "Meowklka";

    private readonly Dictionary<NetUserId, Queue<TimeSpan>> _history = new();
    private readonly HashSet<NetUserId> _banned = new();

    private NetUserId? _banningAdminId;
    private bool _banningAdminLookupFailed;

    public void Initialize()
    {
    }

    public void RegisterSayUsage(ICommonSession player)
    {
        var userId = player.UserId;

        if (_banned.Contains(userId))
            return;

        var now = _timing.RealTime;
        var threshold = now - TimeSpan.FromSeconds(WindowSeconds);

        if (!_history.TryGetValue(userId, out var queue))
        {
            queue = new Queue<TimeSpan>();
            _history[userId] = queue;
        }

        while (queue.Count > 0 && queue.Peek() < threshold)
            queue.Dequeue();

        queue.Enqueue(now);

        if (queue.Count <= LimitPerWindow)
            return;

        _banned.Add(userId);
        _history.Remove(userId);

        IssueBan(player);
    }

    private async void IssueBan(ICommonSession player)
    {
        try
        {
            var banningAdmin = await ResolveBanningAdminAsync();

            _banManager.CreateServerBan(
                player.UserId,
                player.Name,
                banningAdmin,
                null,
                null,
                null,
                NoteSeverity.High,
                BanReason);
        }
        catch
        {

        }
    }

    private async Task<NetUserId?> ResolveBanningAdminAsync()
    {
        if (_banningAdminId.HasValue)
            return _banningAdminId;

        if (_banningAdminLookupFailed)
            return null;

        var data = await _locator.LookupIdByNameAsync(BanningAdminName);
        if (data == null)
        {
            _banningAdminLookupFailed = true;
            return null;
        }

        _banningAdminId = data.UserId;
        return _banningAdminId;
    }
}

