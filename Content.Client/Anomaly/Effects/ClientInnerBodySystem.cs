using Content.Client.DisplacementMap;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects;
using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.Anomaly.Effects
{
    public sealed class ClientInnerBodyAnomalySystem : SharedInnerBodyAnomalySystem
    {
        [Dependency] private readonly SpriteSystem _sprite = default!;
        [Dependency] private readonly DisplacementMapSystem _displacement = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override void Initialize()
        {
            SubscribeLocalEvent<InnerBodyAnomalyComponent, AfterAutoHandleStateEvent>(OnAfterHandleState);
            SubscribeLocalEvent<InnerBodyAnomalyComponent, ComponentShutdown>(OnCompShutdown);
        }

        private void OnAfterHandleState(Entity<InnerBodyAnomalyComponent> ent, ref AfterAutoHandleStateEvent args)
        {
            if (!TryComp<SpriteComponent>(ent, out var sprite))
                return;

            if (ent.Comp.FallbackSprite is null)
                return;

            var index = _sprite.LayerMapReserve((ent.Owner, sprite), ent.Comp.LayerMap);

            if (TryComp<HumanoidAppearanceComponent>(ent, out var humanoidAppearance) &&
                ent.Comp.SpeciesSprites.TryGetValue(humanoidAppearance.Species, out var speciesSprite))
            {
                _sprite.LayerSetSprite((ent.Owner, sprite), index, speciesSprite);
            }
            else
            {
                _sprite.LayerSetSprite((ent.Owner, sprite), index, ent.Comp.FallbackSprite);
            }

            _sprite.LayerSetVisible((ent.Owner, sprite), index, true);
            sprite.LayerSetShader(index, "unshaded");

            if (TryComp<InnerBodyAnomalyVisualsComponent>(ent, out var visuals) && visuals.Displacement != null)
            {
                if (_prototypeManager.TryIndex<DisplacementDataPrototype>(visuals.Displacement.Value.Id, out var displacement))
                {
                    _displacement.TryAddDisplacement(displacement.Displacement,
                        (ent.Owner, sprite),
                        index,
                        ent.Comp.LayerMap,
                        out _);
                }
                else
                {
                    _displacement.EnsureDisplacementIsNotOnSprite((ent.Owner, sprite), ent.Comp.LayerMap);
                }
            }
        }

        private void OnCompShutdown(Entity<InnerBodyAnomalyComponent> ent, ref ComponentShutdown args)
        {
            if (!TryComp<SpriteComponent>(ent, out var sprite))
                return;

            var index = _sprite.LayerMapGet((ent.Owner, sprite), ent.Comp.LayerMap);
            _sprite.LayerSetVisible((ent.Owner, sprite), index, false);

            _displacement.EnsureDisplacementIsNotOnSprite((ent.Owner, sprite), ent.Comp.LayerMap);
        }
    }
}