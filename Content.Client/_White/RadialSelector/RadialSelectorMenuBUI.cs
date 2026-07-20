using Content.Client.UserInterface.Controls;
using Content.Shared._White.RadialSelector;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.Input;

// ReSharper disable InconsistentNaming

namespace Content.Client._White.RadialSelector;

[UsedImplicitly]
public sealed class AttachedRadialSelectorMenuBUI(EntityUid owner, Enum uiKey)
    : BasedRadialSelectorMenuBUI(owner, uiKey)
{
    [Dependency] private readonly IClyde _displayManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;

    private readonly RadialMenu _menu = new()
    {
        HorizontalExpand = true,
        VerticalExpand = true,
        BackButtonStyleClass = "RadialMenuBackButton",
        CloseButtonStyleClass = "RadialMenuCloseButton"
    };

    private bool _openCentered;
    // RW start
    private bool _positioned;
    // RW end

    protected override void Open()
    {
        base.Open();
        _menu.OnClose += Close;
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not RadialSelectorState radialSelectorState)
            return;

        ClearExistingContainers(_menu);
        CreateMenu(radialSelectorState.Entries, _menu);
        _openCentered = radialSelectorState.OpenCentered;

        // RW start
        if (!_positioned)
        {
            _positioned = true;
            if (_openCentered)
                _menu.OpenCentered();
            else
                _menu.OpenCenteredAt(_inputManager.MouseScreenPosition.Position / _displayManager.ScreenSize);
        }
        // RW end
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _menu.Dispose();
    }
}
