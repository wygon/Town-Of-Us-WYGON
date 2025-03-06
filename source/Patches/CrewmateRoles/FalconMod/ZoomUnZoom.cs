using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.FalconMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ZoomUnZoom
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Falcon))
            {
                var falcon = (Falcon)role;
                if (falcon.isZoom)
                    falcon.Zoom();
                else if (falcon.Enabled) falcon.UnZoom();
            }
        }
    }
}