﻿using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.Start))]
    public static class KillButtonAwake
    {
        public static void Prefix(KillButton __instance)
        {
            __instance.transform.Find("Text_TMP").gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillButtonSprite
    {
        private static Sprite Fix => TownOfUs.EngineerFix;
        private static Sprite Medic => TownOfUs.MedicSprite;
        private static Sprite Seer => TownOfUs.SeerSprite;
        private static Sprite Douse => TownOfUs.DouseSprite;
        private static Sprite Revive => TownOfUs.ReviveSprite;
        private static Sprite Alert => TownOfUs.AlertSprite;
        private static Sprite Remember => TownOfUs.RememberSprite;
        private static Sprite Track => TownOfUs.TrackSprite;
        private static Sprite Transport => TownOfUs.TransportSprite;
        private static Sprite Mediate => TownOfUs.MediateSprite;
        private static Sprite Vest => TownOfUs.VestSprite;
        private static Sprite Protect => TownOfUs.ProtectSprite;
        private static Sprite Infect => TownOfUs.InfectSprite;
        private static Sprite Trap => TownOfUs.TrapSprite;
        private static Sprite Inspect => TownOfUs.InspectSprite;
        private static Sprite Observe => TownOfUs.ObserveSprite;
        private static Sprite Bite => TownOfUs.BiteSprite;
        private static Sprite Confess => TownOfUs.ConfessSprite;
        private static Sprite Campaign => TownOfUs.CampaignSprite;
        private static Sprite Fortify => TownOfUs.FortifySprite;
        private static Sprite Jail => TownOfUs.JailSprite;
        private static Sprite Collect => TownOfUs.CollectSprite;
        private static Sprite Watch => TownOfUs.WatchSprite;
        private static Sprite Camp => TownOfUs.CampSprite;
        private static Sprite Zoom => TownOfUs.ZoomPlusActiveButton;
        private static Sprite WingMan => TownOfUs.WingManSprite;
        private static Sprite Rewind => TownOfUs.RewindSprite;
        private static Sprite Admin => TownOfUs.AdminSprite;
        private static Sprite Vitals => TownOfUs.VitalsSprite;

        private static Sprite Kill;


        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

            var flag = false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                __instance.KillButton.graphic.sprite = Seer;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = Medic;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = Douse;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Altruist))
            {
                __instance.KillButton.graphic.sprite = Revive;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = Alert;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
            {
                __instance.KillButton.graphic.sprite = Remember;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = Track;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                __instance.KillButton.graphic.sprite = Transport;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = Mediate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                __instance.KillButton.graphic.sprite = Vest;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.graphic.sprite = Protect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = Infect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
            {
                __instance.KillButton.graphic.sprite = Fix;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                __instance.KillButton.graphic.sprite = Trap;
                flag = true;
            }else if (PlayerControl.LocalPlayer.Is(RoleEnum.Falcon))
            {
                __instance.KillButton.graphic.sprite = WingMan;
                flag = true;
            }else if (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord))
            {
                __instance.KillButton.graphic.sprite = Rewind;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = Inspect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Foreteller))
            {
                __instance.KillButton.graphic.sprite = Observe;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                __instance.KillButton.graphic.sprite = Bite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                __instance.KillButton.graphic.sprite = Confess;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Politician))
            {
                __instance.KillButton.graphic.sprite = Campaign;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Warden))
            {
                __instance.KillButton.graphic.sprite = Fortify;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                __instance.KillButton.graphic.sprite = Jail;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
            {
                __instance.KillButton.graphic.sprite = Collect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                __instance.KillButton.graphic.sprite = Watch;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy))
            {
                __instance.KillButton.graphic.sprite = Camp;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
            {
                __instance.KillButton.graphic.sprite = Admin;
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Kill";
                flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff) || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut);
            }
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors) &&
                GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek)
            {
                __instance.KillButton.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch) 
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Icenberg) || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut) || PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }

            bool KillKey = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("Kill");
            var controller = ConsoleJoystick.player.GetButtonDown(8);
            if ((KillKey || controller) && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.KillButton.DoClick();

            var role = Role.GetRole(PlayerControl.LocalPlayer);
            bool AbilityKey = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU imp/nk");
            if (role?.ExtraButtons != null && AbilityKey && !PlayerControl.LocalPlayer.Data.IsDead)
                role?.ExtraButtons[0]?.DoClick();

            if (Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic/freeze") &&
                !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer).ButtonButton.DoClick();
            }
            else if (Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                     Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic/freeze") &&
                     !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer).DisperseButton.DoClick();
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
                    return;
                }
                var ghostRole = false;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    var haunter = Role.GetRole<Haunter>(PlayerControl.LocalPlayer);
                    if (!haunter.Caught) ghostRole = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);
                    if (!phantom.Caught) ghostRole = true;
                }
                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole && Utils.ShowDeadBodies && !MeetingHud.Instance);
            }
        }
    }
}
