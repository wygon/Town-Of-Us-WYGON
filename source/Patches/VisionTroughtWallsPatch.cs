//using HarmonyLib;
//using TownOfUs.Roles;

//[HarmonyPatch(typeof(PlayerControl), "CanSeePlayer")]
//public class VisionThroughWallsPatch
//{
//    public static void Postfix(ref bool __result, PlayerControl __instance)
//    {
//        var role = Role.GetRole<Zoomer>(PlayerControl.LocalPlayer);
//        if (__instance == PlayerControl.LocalPlayer)
//        {
//            if(role.isZoom)//.Instance.Enabled)
//                __result = true; // Widzenie przez ściany, jeśli Zoom jest włączony
//            else
//                __result = false;
//        }
//    }
//}