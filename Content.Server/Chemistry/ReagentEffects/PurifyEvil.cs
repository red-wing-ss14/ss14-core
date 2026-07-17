using System.Threading;
using Content.Shared.EntityEffects;
using Content.Shared.Jittering;
using Content.Shared._RW.BloodCult.BloodCultist;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReagentEffects;

public sealed partial class PurifyEvilSystem : EntityEffectSystem<BloodCultistComponent, PurifyEvil>
{
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;

    protected override void Effect(Entity<BloodCultistComponent> entity, ref EntityEffectEvent<PurifyEvil> args)
    {
        if (entity.Comp.DeconvertToken is not null)
            return;

        _jitter.DoJitter(entity.Owner, args.Effect.Time, true, args.Effect.Amplitude, args.Effect.Frequency);

        entity.Comp.DeconvertToken = new CancellationTokenSource();
        Robust.Shared.Timing.Timer.Spawn(args.Effect.Time, () => DeconvertCultist(entity.Owner), entity.Comp.DeconvertToken.Token);
    }

    private void DeconvertCultist(EntityUid uid)
    {
        if (HasComp<BloodCultistComponent>(uid))
            RemComp<BloodCultistComponent>(uid);
    }
}

[UsedImplicitly]
public sealed partial class PurifyEvil : EntityEffectBase<PurifyEvil>
{
    [DataField]
    public float Amplitude = 10.0f;

    [DataField]
    public float Frequency = 4.0f;

    [DataField]
    public TimeSpan Time = TimeSpan.FromSeconds(30.0f);

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        return Loc.GetString("reagent-effect-guidebook-purify-evil");
    }
}
