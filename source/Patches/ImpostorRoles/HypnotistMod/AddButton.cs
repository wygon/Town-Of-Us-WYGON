using System;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfUs.ImpostorRoles.HypnotistMod
{
    public class AddHysteriaButton
    {
        public static Sprite HysteriaSprite => TownOfUs.HysteriaSprite;

        public static void GenButton(Hypnotist role, int index)
        {
            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = HysteriaSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(TriggerHysteria(role));
            role.HysteriaButton = newButton;
        }


        private static Action TriggerHysteria(Hypnotist role)
        {
            void Listener()
            {
                role.HysteriaButton.Destroy();
                role.HysteriaActive = true;
                role.Hysteria();
                Utils.Rpc(CustomRPC.Hypnotise, PlayerControl.LocalPlayer.PlayerId, (byte)1);
            }

            return Listener;
        }

        public static void AddHypnoButtons(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Hypnotist))
            {
                var hypnotist = (Hypnotist)role;
                hypnotist.HysteriaButton.Destroy();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hypnotist)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var hypnotistrole = Role.GetRole<Hypnotist>(PlayerControl.LocalPlayer);
            if (hypnotistrole.HysteriaActive) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (PlayerControl.LocalPlayer.PlayerId == __instance.playerStates[i].TargetPlayerId)
                {
                    GenButton(hypnotistrole, i);
                }
        }
    }
}