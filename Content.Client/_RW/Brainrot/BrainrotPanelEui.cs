using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared._RW.Brainrot;
using JetBrains.Annotations;

namespace Content.Client._RW.Brainrot;

[UsedImplicitly]
public sealed class BrainrotPanelEui : BaseEui
{
    private readonly BrainrotPanelWindow _window;

    public BrainrotPanelEui()
    {
        _window = new BrainrotPanelWindow(this);
        _window.OnClose += SendClosedMessage;
    }

    public override void Opened()
    {
        base.Opened();
        _window.OpenCentered();
    }

    public override void Closed()
    {
        base.Closed();
        _window.OnClose -= SendClosedMessage;
        _window.Close();
        _window.Dispose();
    }

    private void SendClosedMessage()
    {
        SendMessage(new CloseEuiMessage());
    }

    public override void HandleState(EuiStateBase state)
    {
        base.HandleState(state);

        if (state is not BrainrotPanelEuiState s)
            return;

        _window.UpdateState(s);
    }

    public new void SendMessage(EuiMessageBase message)
    {
        base.SendMessage(message);
    }
}
