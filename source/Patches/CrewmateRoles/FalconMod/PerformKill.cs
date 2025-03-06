using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.FalconMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        //public static Sprite WingMan => TownOfUs.WingManSprite;

        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Falcon);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Falcon>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            if (role.sabotageLightsZoom()) return false;
            var wingManButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == wingManButton)
            {
                if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                role.TimeRemaining = CustomGameOptions.WingManDuration;
                role.UsesLeft--;
                role.Zoom();
                return false;
            }
            //if (role.ZoomTimer() != 0) return false;
            //if (!role.isZoom) return false;
            //if (__instance == role.ZoomButton)
            //{
            //    if (role.Player.inVent) return false;
            //    if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            //    var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
            //    if (!abilityUsed) return false;
            //    role.TimeRemaining = CustomGameOptions.ZoomDuration;
            //    role.Zoom();
            //    return false;
            //}
            //if (role.ZoomTimer() != 0) return false;
            //if (!role.isZoom) return false;
            return true;
        }
    }
}
