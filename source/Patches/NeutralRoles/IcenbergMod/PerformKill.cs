using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.IcenbergMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    internal class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Icenberg) && __instance.isActiveAndEnabled &&
                !__instance.isCoolingDown && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.inVent)
                return Role.GetRole<Icenberg>(PlayerControl.LocalPlayer).UseAbility(__instance);

            return true;
        }
    }
}
