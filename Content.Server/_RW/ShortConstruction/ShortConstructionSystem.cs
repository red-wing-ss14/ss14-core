using Content.Server.Construction;
using Content.Server.Popups;
using Content.Shared._White.RadialSelector;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.ShortConstruction;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._RW.ShortConstruction;

public sealed class ShortConstructionSystem : EntitySystem
{
    [Dependency] private readonly ConstructionSystem _construction = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShortConstructionComponent, RadialSelectorSelectedMessage>(OnSelected);
    }

    private void OnSelected(Entity<ShortConstructionComponent> ent, ref RadialSelectorSelectedMessage args)
    {
        var user = args.Actor;
        var selectedItem = args.SelectedItem;

        _playerManager.TryGetSessionByEntity(user, out var session);

        _ui.CloseUi(ent.Owner, RadialSelectorUiKey.Key, user);

        var userTransform = Transform(user);
        var dirVec = userTransform.LocalRotation.GetCardinalDir().ToVec();
        var coordinates = userTransform.Coordinates.Offset(dirVec).SnapToGrid(EntityManager);
        var angle = userTransform.LocalRotation;
        var netEnt = GetNetEntity(ent.Owner);

        if (_prototype.TryIndex(selectedItem, out ConstructionPrototype? constructionPrototype))
        {
            foreach (var condition in constructionPrototype.Conditions)
            {
                if (!condition.Condition(user, coordinates, angle.GetCardinalDir()))
                {
                    var message = condition.GenerateGuideEntry()?.Localization;
                    if (message != null)
                    {
                        _popup.PopupEntity(Loc.GetString(message), user, user);
                    }
                    return;
                }
            }
        }

        StartConstruction(user, selectedItem, coordinates, angle, session, netEnt);
    }

    private async void StartConstruction(
        EntityUid user,
        string selectedItem,
        EntityCoordinates coordinates,
        Angle angle,
        ICommonSession? session,
        NetEntity netEnt)
    {
        await _construction.TryStartStructureConstruction(user, selectedItem, coordinates, angle, ack: 0, senderSession: session, with: netEnt);
    }
}
