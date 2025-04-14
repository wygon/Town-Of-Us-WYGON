using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Patches;
using System.Runtime.CompilerServices;  

namespace TownOfUs.Patches.CrewmateRoles.SpyMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Spy);
            if (!flag) return true;
            var role = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
            if (!__instance.enabled) return false;

            MapOptions mapOptions = GameManager.Instance.GetMapOptions();
            mapOptions.IncludeDeadBodies = true;
            mapOptions.AllowMovementWhileMapOpen = true;
            mapOptions.Mode = (MapOptions.Modes)2;
            DestroyableSingleton<HudManager>.Instance.ToggleMapVisible(mapOptions);

            return false;
        }
    }
}
