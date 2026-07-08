using Content.Shared.DisplacementMap;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Nutrition.Components
{
    [Access(typeof(SharedCreamPieSystem))]
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class CreamPiedComponent : Component
    {
        [ViewVariables, AutoNetworkedField]
        public bool CreamPied { get; set; } = false;

        /// <summary>
        /// If set, applies a displacement map to the pie sprite.
        /// </summary>
        [DataField, AutoNetworkedField]
        public ProtoId<DisplacementDataPrototype>? Displacement;
    }

    [Serializable, NetSerializable]
    public enum CreamPiedVisuals
    {
        Creamed,
    }
}