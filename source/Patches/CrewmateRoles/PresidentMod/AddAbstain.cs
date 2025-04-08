using HarmonyLib;
using TMPro;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.PresidentMod
{
    public class AddAbstain
    {
        public static void UpdateButton(President role, MeetingHud __instance)
        {
            var skip = __instance.SkipVoteButton;
            role.Abstain.gameObject.SetActive(skip.gameObject.active && !role.VotedOnce);
            role.Abstain.voteComplete = skip.voteComplete;
            role.Abstain.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            if (role.VoteBank != CustomGameOptions.PresidentMaximumBank) role.Abstain.GetComponentsInChildren<TextMeshPro>()[0].text = "Abstain";
            else role.Abstain.GetComponentsInChildren<TextMeshPro>()[0].text = "Hold Off";
        }


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void GenButton(President role, MeetingHud __instance)
            {
                var skip = __instance.SkipVoteButton;
                role.Abstain = Object.Instantiate(skip, skip.transform.parent);
                role.Abstain.Parent = __instance;
                role.Abstain.SetTargetPlayerId(251);
                role.Abstain.transform.localPosition = skip.transform.localPosition +
                                                       new Vector3(0f, -0.17f, 0f);
                skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                UpdateButton(role, __instance);
            }

            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.President)) return;
                var presRole = Role.GetRole<President>(PlayerControl.LocalPlayer);
                GenButton(presRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.President)) return;
                var presRole = Role.GetRole<President>(PlayerControl.LocalPlayer);
                UpdateButton(presRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.President)) return;
                var presRole = Role.GetRole<President>(PlayerControl.LocalPlayer);
                presRole.Abstain.ClearButtons();
                UpdateButton(presRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.President)) return;
                var presRole = Role.GetRole<President>(PlayerControl.LocalPlayer);
                if (__0 != 251) presRole.Abstain.ClearButtons();

                UpdateButton(presRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.President)) return;
                var presRole = Role.GetRole<President>(PlayerControl.LocalPlayer);
                UpdateButton(presRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.President)) return;
                var presRole = Role.GetRole<President>(PlayerControl.LocalPlayer);
                switch (__instance.state)
                {
                    case MeetingHud.VoteStates.Discussion:
                        if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                        {
                            presRole.Abstain.SetDisabled();
                            break;
                        }


                        presRole.Abstain.SetEnabled();
                        break;
                }

                UpdateButton(presRole, __instance);
            }
        }
    }
}