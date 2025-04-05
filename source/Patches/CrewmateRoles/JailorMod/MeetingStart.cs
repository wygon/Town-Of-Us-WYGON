using HarmonyLib;
using Reactor.Utilities;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine;
using System.Collections;
using Reactor.Utilities.Extensions;

namespace TownOfUs.CrewmateRoles.JailorrMod
{
    public class MeetingStart
    {

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void Postfix(MeetingHud __instance)
            {
                var jailors = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Jailor && x.Player != null).Cast<Jailor>();

                if (PlayerControl.LocalPlayer.Data.IsDead) return;
                foreach (var role in jailors)
                {
                    if (role.Jailed?.PlayerId == PlayerControl.LocalPlayer.PlayerId && !role.Jailed.Data.IsDead)
                    {
                        Coroutines.Start(JailShhh());
                    }
                }
                if (PlayerControl.LocalPlayer.IsJailed())
                {
                    string jailedMess;
                    if (PlayerControl.LocalPlayer.Is(Faction.Crewmates))
                    {
                        jailedMess = Patches.TranslationPatches.CurrentLanguage == 0 ?
                            $"You are jailed, provide relevant information to the Jailor, to prove you are Crew" :
                            $"Jestes uwieziony, podaj odpowiednie informacje Jailorowi, aby udowodnic, że jestes Crewmate";
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, jailedMess);
                    }
                    else
                    {
                        jailedMess = Patches.TranslationPatches.CurrentLanguage == 0 ?
                            $"You are jailed, convince the Jailor that you are Crew to avoid being executed" :
                            $"Jestes uwieziony, przekonaj Jailora, że jestes Crewmate, aby uniknac egzekucji";
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, jailedMess);
                    }
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
                {
                    var jailor = Role.GetRole<Jailor>(PlayerControl.LocalPlayer);
                    if (jailor.Jailed.Data.IsDead || jailor.Jailed.Data.Disconnected) return;
                    var jailMess = Patches.TranslationPatches.CurrentLanguage == 0 ?
                        $"Examine <color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>{jailor.Jailed.name.ToUpper()}.</color>\nUse /all to communicate with everyone" :
                        $"Przepytaj <color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>{jailor.Jailed.name.ToUpper()}.</color>\nNapisz /all aby pisac z wszystkimi";
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, jailMess);
                }
            }
            public static IEnumerator JailShhh()
            {
                yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0.5f, 0.5f, 0.5f, 0.98f));
                var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
                var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
                HudManager.Instance.shhhEmblem.transform.localPosition = new Vector3(
                    HudManager.Instance.shhhEmblem.transform.localPosition.x,
                    HudManager.Instance.shhhEmblem.transform.localPosition.y,
                    HudManager.Instance.FullScreen.transform.position.z + 1f);
                var jailedMess = Patches.TranslationPatches.CurrentLanguage == 0 ?
                    "YOU ARE JAILED" :
                    "JESTES UWIEZIONY";
                HudManager.Instance.shhhEmblem.TextImage.text = jailedMess;
                HudManager.Instance.shhhEmblem.HoldDuration = 2.5f;
                yield return HudManager.Instance.ShowEmblem(true);
                HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
                HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
                yield return HudManager.Instance.CoFadeFullScreen(new Color(0.5f, 0.5f, 0.5f, 0.98f), Color.clear);
                yield return null;
            }
        }
    }
}