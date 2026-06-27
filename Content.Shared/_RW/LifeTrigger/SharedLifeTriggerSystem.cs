using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Verbs;

namespace Content.Shared._RW.LifeTrigger;

public sealed class SharedLifeTriggerSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LifeTriggerComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<LifeTriggerComponent, GetVerbsEvent<AlternativeVerb>>(OnGetVerbs);
        SubscribeLocalEvent<LifeTriggerComponent, ExaminedEvent>(OnExamine);
    }

    private void OnUseInHand(EntityUid uid, LifeTriggerComponent component, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        component.TriggerState = component.TriggerState switch
        {
            MobState.Dead => MobState.Critical,
            MobState.Critical => MobState.HardCritical,
            MobState.HardCritical => MobState.Dead,
            _ => MobState.Dead
        };
        Dirty(uid, component);

        var stateStr = GetStateName(component.TriggerState);
        _popup.PopupClient(Loc.GetString("life-trigger-switched", ("state", stateStr)), uid, args.User);
        args.Handled = true;
    }

    private void OnGetVerbs(EntityUid uid, LifeTriggerComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        foreach (var state in new[] { MobState.Dead, MobState.Critical, MobState.HardCritical })
        {
            var stateName = GetStateName(state);
            var verbState = state;
            var verb = new AlternativeVerb
            {
                Text = Loc.GetString("life-trigger-verb-set-state", ("state", stateName)),
                Act = () =>
                {
                    component.TriggerState = verbState;
                    Dirty(uid, component);
                    _popup.PopupClient(Loc.GetString("life-trigger-switched", ("state", stateName)), uid, args.User);
                },
                Disabled = component.TriggerState == state,
                Priority = (int) state
            };
            args.Verbs.Add(verb);
        }
    }

    private void OnExamine(EntityUid uid, LifeTriggerComponent component, ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("life-trigger-examine", ("state", GetStateName(component.TriggerState))));
    }

    private string GetStateName(MobState state)
    {
        return state switch
        {
            MobState.Dead => Loc.GetString("life-trigger-state-dead"),
            MobState.Critical => Loc.GetString("life-trigger-state-critical"),
            MobState.HardCritical => Loc.GetString("life-trigger-state-severe-critical"),
            _ => Loc.GetString("life-trigger-state-dead")
        };
    }
}
