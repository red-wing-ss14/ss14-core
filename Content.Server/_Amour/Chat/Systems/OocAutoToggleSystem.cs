using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Shared._Amour.CCVar;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Server._Amour.Chat.Systems;

public sealed class OocAutoToggleSystem : EntitySystem
{
    [Dependency] private readonly IChatManager _chat = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    private bool _enabled;
    private int _playerThreshold;

    public override void Initialize()
    {
        base.Initialize();

        _player.PlayerStatusChanged += OnPlayerStatusChanged;
        Subs.CVar(_cfg, AmourCCVars.OocAutoTogglePlayerThreshold, OnPlayerThresholdChanged, true);
        Subs.CVar(_cfg, AmourCCVars.OocAutoToggleEnabled, OnEnabledChanged, true);
        Subs.CVar(_cfg, CCVars.OocEnabled, OnOocEnabledChanged);

        SubscribeLocalEvent<GameRunLevelChangedEvent>(OnGameRunLevelChanged);
    }

    public override void Shutdown()
    {
        base.Shutdown();

        _player.PlayerStatusChanged -= OnPlayerStatusChanged;
    }

    private void OnPlayerThresholdChanged(int value)
    {
        _playerThreshold = Math.Max(0, value);
        RefreshOocState();
    }

    private void OnEnabledChanged(bool value)
    {
        _enabled = value;
        RefreshOocState();
    }

    private void OnOocEnabledChanged(bool value)
    {
        if (!_enabled || value == ShouldEnableOoc())
            return;

        RefreshOocState();
    }

    private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs args)
    {
        RefreshOocState();
    }

    private void OnGameRunLevelChanged(GameRunLevelChangedEvent args)
    {
        RefreshOocState();
    }

    private bool ShouldEnableOoc()
    {
        return _player.PlayerCount <= _playerThreshold;
    }

    private void RefreshOocState()
    {
        if (!_enabled)
            return;

        var desired = ShouldEnableOoc();
        if (_cfg.GetCVar(CCVars.OocEnabled) == desired)
            return;

        _chat.SetOocEnabledSilently(desired);
    }
}
