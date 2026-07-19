// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Store;
using Robust.Shared.Prototypes;

namespace Content.Shared._White.StoreDiscount;

[DataDefinition]
public sealed partial class SalesSpecifier
{
    [DataField]
    public bool Enabled { get; private set; }

    [DataField]
    public float MinMultiplier { get; private set; }

    [DataField]
    public float MaxMultiplier { get; private set; }

    [DataField]
    public int MinItems { get; private set; }

    [DataField]
    public int MaxItems { get; private set; }

    [DataField]
    public ProtoId<StoreCategoryPrototype> SalesCategory { get; private set; } = "UplinkSales";

    [DataField]
    public int PlayersPerDiscountStock { get; private set; } = 10;

    [DataField]
    public bool ShortagesEnabled { get; private set; }

    [DataField]
    public float ShortageMinMultiplier { get; private set; } = 1.1f;

    [DataField]
    public float ShortageMaxMultiplier { get; private set; } = 1.25f;

    [DataField]
    public int ShortageMinItems { get; private set; }

    [DataField]
    public int ShortageMaxItems { get; private set; }

    [DataField]
    public int ShortageMinStock { get; private set; } = 1;

    [DataField]
    public int ShortageMaxStock { get; private set; } = 5;

    [DataField]
    public ProtoId<StoreCategoryPrototype> ShortageCategory { get; private set; } = "UplinkShortages";

    public SalesSpecifier()
    {
    }

    public SalesSpecifier(bool enabled, float minMultiplier, float maxMultiplier, int minItems, int maxItems,
        ProtoId<StoreCategoryPrototype> salesCategory)
    {
        Enabled = enabled;
        MinMultiplier = minMultiplier;
        MaxMultiplier = maxMultiplier;
        MinItems = minItems;
        MaxItems = maxItems;
        SalesCategory = salesCategory;
    }
}