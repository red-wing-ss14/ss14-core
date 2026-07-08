using Content.Client.DisplacementMap;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client.Nutrition.EntitySystems
{
    public sealed class CreamPiedSystem : SharedCreamPieSystem
    {
        [Dependency] private readonly SpriteSystem _sprite = default!;
        [Dependency] private readonly DisplacementMapSystem _displacement = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<CreamPiedComponent, AppearanceChangeEvent>(OnAppearanceChange);
            SubscribeLocalEvent<CreamPiedComponent, ComponentShutdown>(OnShutdown);
        }

        private void OnShutdown(EntityUid uid, CreamPiedComponent component, ComponentShutdown args)
        {
            if (TryComp<SpriteComponent>(uid, out var sprite))
            {
                _displacement.EnsureDisplacementIsNotOnSprite((uid, sprite), "clownedon");
            }
        }

        private void OnAppearanceChange(EntityUid uid, CreamPiedComponent component, ref AppearanceChangeEvent args)
        {
            if (args.Sprite == null)
                return;

            if (!component.CreamPied)
            {
                _displacement.EnsureDisplacementIsNotOnSprite((uid, args.Sprite), "clownedon");
                return;
            }

            if (component.Displacement != null && _prototypeManager.TryIndex<Content.Shared.DisplacementMap.DisplacementDataPrototype>(component.Displacement.Value.Id, out var displacementProto))
            {
                if (_sprite.LayerMapTryGet((uid, args.Sprite), "clownedon", out var index, false))
                {
                    _displacement.TryAddDisplacement(displacementProto.Displacement, (uid, args.Sprite), index, "clownedon", out _);
                }
            }
        }
    }
}