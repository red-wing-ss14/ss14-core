using Content.Server.EUI;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared._RW.Brainrot;

namespace Content.Server._RW.Brainrot;

public sealed class BrainrotPanelEui : BaseEui
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IAdminManager _adminManager = default!;

    private readonly RwBrainrotServerSystem _brainrotSystem;

    public BrainrotPanelEui()
    {
        IoCManager.InjectDependencies(this);
        _brainrotSystem = _entManager.System<RwBrainrotServerSystem>();
    }

    public override void Opened()
    {
        base.Opened();

        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Admin))
        {
            Close();
            return;
        }

        StateDirty();
    }

    public override EuiStateBase GetNewState()
    {
        return new BrainrotPanelEuiState(new List<string>(_brainrotSystem.CustomTriggers));
    }

    public override async void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Admin))
            return;

        switch (msg)
        {
            case AddBrainrotTriggerMsg addMsg:
                await _brainrotSystem.AddTrigger(addMsg.Trigger);
                StateDirty();
                break;
            case RemoveBrainrotTriggerMsg removeMsg:
                await _brainrotSystem.RemoveTrigger(removeMsg.Trigger);
                StateDirty();
                break;
        }
    }
}
