using System.Linq;
using HarmonyLib;
using InnerNet;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.IcenbergMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal class HudManagerUpdate
    {
        private static void Postfix(HudManager __instance)
        {
            var icenberg = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Icenberg);
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                if (icenberg != null)
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Icenberg))
                        Role.GetRole<Icenberg>(PlayerControl.LocalPlayer).Update(__instance);
        }
    }
}