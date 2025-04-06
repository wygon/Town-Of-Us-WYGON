using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SpyMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Spy)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            var role = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
            if (__instance == DestroyableSingleton<HudManager>.Instance.KillButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                DestroyableSingleton<HudManager>.Instance.ToggleMapVisible(new MapOptions
                {
                    Mode = MapOptions.Modes.CountOverlay
                });
                PlayerControl.LocalPlayer.NetTransform.Halt();
                return false;
            }
            else if (__instance == role.VitalsButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                if (!__instance.enabled) return false;
                var scientist = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Scientist).Cast<ScientistRole>();
                scientist.minigame = Object.Instantiate<VitalsMinigame>(scientist.VitalsPrefab);
                scientist.minigame.transform.SetParent(Camera.main.transform, false);
                scientist.minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                scientist.minigame.Begin(null);
                return false;
            }
            return false;
        }
    }
}