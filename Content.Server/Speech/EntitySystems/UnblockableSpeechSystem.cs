// SPDX-License-Identifier: MIT

using Content.Server.Chat.Systems;
using Content.Server.Speech.Components;
using Content.Shared.Chat;

namespace Content.Server.Speech.EntitySystems
{
    public sealed class UnblockableSpeechSystem : EntitySystem
    {
        public override void Initialize()
        {
            SubscribeLocalEvent<UnblockableSpeechComponent, Content.Server.Chat.Systems.CheckIgnoreSpeechBlockerEvent>(OnCheck);
        }

        private void OnCheck(EntityUid uid, UnblockableSpeechComponent component, Content.Server.Chat.Systems.CheckIgnoreSpeechBlockerEvent args)
        {
            args.IgnoreBlocker = true;
        }
    }
}
