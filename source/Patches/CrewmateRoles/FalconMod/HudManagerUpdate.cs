using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.Patches.FalconMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        //public static Sprite WingMan => TownOfUs.WingManSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Falcon)) return;
            var wingManButton = __instance.KillButton;

            var role = Role.GetRole<Falcon>(PlayerControl.LocalPlayer);
            //if (role.sabotageLightsZoom()) return;
            if (role.UsesText == null && role.UsesLeft > 0)// lub role.UsesText == null zobacz na engeeniera
            {
                role.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if(role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }
            wingManButton.SetCoolDown(0f, 10f);//startowy cooldown
            wingManButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (role.isZoom) wingManButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.WingManDuration);
            else if (role.ButtonUsable) wingManButton.SetCoolDown(role.ZoomTimer(), CustomGameOptions.WingManCd);
            else wingManButton.SetCoolDown(0f, CustomGameOptions.WingManCd);

            var renderer = wingManButton.graphic;
            if (role.isZoom || (!wingManButton.isCoolingDown && role.ButtonUsable && PlayerControl.LocalPlayer.moveable && !role.sabotageLightsZoom()))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
