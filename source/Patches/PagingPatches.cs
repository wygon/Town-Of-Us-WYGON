using TownOfUs.Components;
using HarmonyLib;

namespace TownOfUs.Patches;

internal static class PagingPatches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingHudStartPatch
    {
        public static void Postfix(MeetingHud __instance)
        {
            __instance.gameObject.AddComponent<MeetingHudBehaviour>().meetingHud = __instance;
        }
    }

    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public static class VitalsMinigameBeginPatch
    {
        public static void Postfix(VitalsMinigame __instance)
        {
            __instance.gameObject.AddComponent<VitalsHudBehaviour>().vitalsMinigame = __instance;
        }
    }

}