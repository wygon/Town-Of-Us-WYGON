using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.JailorrMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (PlayerControl.LocalPlayer.IsJailed())
            {
                string jailedMess;
                if (PlayerControl.LocalPlayer.Is(Faction.Crewmates))
                {
                    jailedMess = Patches.TranslationPatches.CurrentLanguage == 0 ?
                        "You are jailed, provide relevant information to the Jailor to prove you are Crew" :
                        "Jestes uwieziony, podaj odpowiednie informacje, aby udowodnic, że jestes Crewmate";
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, jailedMess);
                }
                else
                {
                    jailedMess = Patches.TranslationPatches.CurrentLanguage == 0 ?
                        "You are jailed, convince the Jailor that you are Crew to avoid being executed" :
                        "Jestes uwieziony, przekonaj Jailora, że jestes Crewmate, aby uniknac egzekucji";
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, jailedMess);
                }
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                var jailor = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                if (jailor.Jailed.Data.IsDead || jailor.Jailed.Data.Disconnected) return;
                var jailMess = Patches.TranslationPatches.CurrentLanguage == 0 ?
                    "Communicate with jailed" :
                    "Pisz z wiezniem";
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, jailMess);
            }
        }
    }
}
