using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
namespace TownOfUs.ImpostorRoles.KamikazeMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SuicideSprite => TownOfUs.SuicideSprite;

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Kamikaze)) return;
            var role = Role.GetRole<Kamikaze>(PlayerControl.LocalPlayer);
            
            
            if (role.PlantButton == null)
            {
                role.PlantButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PlantButton.graphic.enabled = true;
                role.PlantButton.graphic.sprite = SuicideSprite;
                role.PlantButton.gameObject.SetActive(false);
            }

            role.PlantButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            //role.PlantButton.SetCoolDown()
            if (role.Detonating)
            {
                role.PlantButton.graphic.sprite = SuicideSprite;
                role.PlantButton.graphic.color = Palette.EnabledColor;
                role.PlantButton.graphic.material.SetFloat("_Desat", 0f);
                role.DetonateTimer();
                role.PlantButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.KamikazeDetonateDelay);
            }
            else
            {
                role.PlantButton.graphic.sprite = SuicideSprite;
                if (!role.Detonated) role.DetonateKillStart();
                if (PlayerControl.LocalPlayer.killTimer > 0 || !PlayerControl.LocalPlayer.moveable)
                {
                    role.PlantButton.graphic.color = Palette.DisabledClear;
                    role.PlantButton.graphic.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    role.PlantButton.graphic.color = Palette.EnabledColor;
                    role.PlantButton.graphic.material.SetFloat("_Desat", 0f);
                }
                role.PlantButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer,
                    GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }

            if (role.PlantButton.graphic.sprite == SuicideSprite) role.PlantButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer, 
                GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            else role.PlantButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.KamikazeDetonateDelay);
        }
    }
}
