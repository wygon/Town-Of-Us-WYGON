using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.NoclipMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Noclip => TownOfUs.NoclipSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Noclip)) return;
            var role = Role.GetRole<Noclip>(PlayerControl.LocalPlayer);
            if (role.NoclipButton == null)
            {
                role.NoclipButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.NoclipButton.graphic.enabled = true;
                role.NoclipButton.gameObject.SetActive(false);
            }
            role.NoclipButton.graphic.sprite = Noclip;
            role.NoclipButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.Noclipped)
            {
                role.NoclipButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.NoclipDuration);
                role.NoclipButton.graphic.color = Palette.EnabledColor;
                role.NoclipButton.graphic.material.SetFloat("_Desat", 0f);
                //return;
            }
            else if (PlayerControl.LocalPlayer.moveable && role.NoclipTimer() == 0f)
            {
                role.NoclipButton.SetCoolDown(role.NoclipTimer(), CustomGameOptions.NoclipCd);
                role.NoclipButton.graphic.color = Palette.EnabledColor;
                role.NoclipButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.NoclipButton.SetCoolDown(role.NoclipTimer(), CustomGameOptions.NoclipCd);
                role.NoclipButton.graphic.color = Palette.DisabledClear;
                role.NoclipButton.graphic.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
