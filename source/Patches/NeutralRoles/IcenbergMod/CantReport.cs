//using HarmonyLib;
//using Reactor.Utilities;
//using System.Linq;
//using TownOfUs.Roles;
//using static TownOfUs.Utils;
//using static TownOfUs.Roles.Icenberg;

//namespace TownOfUs.NeutralRoles.IcenbergMod
//{
//    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
//    public class StopReport
//    {
//        [HarmonyPriority(Priority.First)]
//        public static bool Prefix(ReportButton __instance)
//        {
//            if (PlayerControl.LocalPlayer.IsFreezed())
//            {
//                var icenbergRole = Utils.GetIcenberg(PlayerControl.LocalPlayer);
//                Utils.Freeze(icenbergRole.Player, PlayerControl.LocalPlayer);
//                return false;
//            }
//            return true;
//        }
//    }
//}
