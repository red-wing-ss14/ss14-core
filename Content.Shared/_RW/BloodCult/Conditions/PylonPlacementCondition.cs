using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Content.Shared._RW.BloodCult.Components;
using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Shared._RW.BloodCult.Conditions;

[UsedImplicitly]
[DataDefinition]
public sealed partial class PylonPlacementCondition : IConstructionCondition
{
    [DataField]
    public float Range = 10f;

    public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
    {
        var entManager = IoCManager.Resolve<IEntityManager>();
        if (!entManager.TrySystem<EntityLookupSystem>(out var lookup))
            return false;

        var entities = lookup.GetEntitiesInRange(location, Range);
        foreach (var entity in entities)
        {
            if (entManager.HasComponent<PylonComponent>(entity))
            {
                return false;
            }
        }

        return true;
    }

    public ConstructionGuideEntry GenerateGuideEntry()
    {
        return new ConstructionGuideEntry
        {
            Localization = "pylon-placement-another-pylon-nearby",
        };
    }
}
