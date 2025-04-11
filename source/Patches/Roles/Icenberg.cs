using AmongUs.GameOptions;
using InnerNet;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TownOfUs.DisableAbilities;
using TownOfUs.Extensions;
using TownOfUs.Patches.NeutralRoles;
using TownOfUs.Patches;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using Reactor.Utilities.Extensions;


namespace TownOfUs.Roles
{
    public class Icenberg : Role//, IVisualAlteration
    {
        public static Sprite LockSprite = TownOfUs.LockSprite;
        public static Sprite FreezeSprite = TownOfUs.FreezeSprite;
        public static Sprite BlizzardSprite = TownOfUs.BlizzardSprite;

        public Icenberg(PlayerControl owner) : base(owner)
        {
            Name = "Icenberg";
            Color = Patches.Colors.Icenberg;
            LastFreeze = DateTime.UtcNow;
            LastKill = DateTime.UtcNow;
            FreezeButton = null;
            FreezeAllButton = null;
            KillTarget = null;
            FreezeTarget = null;
            FreezeAllButtonUsed = false;
            RoleType = RoleEnum.Icenberg;
            AddToRoleHistory(RoleType);
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "So cold... Ye?" : "Zimno... Co?";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Freeze to death and win\nFake Tasks:" : "Zamroz na smierc i wygraj\nFake Tasks:";
            Faction = Faction.NeutralKilling;
        }
        public PlayerControl ClosestPlayer;
        public PlayerControl Freezed;
        public DateTime LastFreeze { get; set; }
        public DateTime LastKill { get; set; }
        public KillButton FreezeButton { get; set; }
        public KillButton FreezeAllButton { get; set; }
        public PlayerControl KillTarget { get; set; }
        public PlayerControl FreezeTarget { get; set; }
        public bool IsUsingFreeze { get; set; }
        public bool IcenbergWins { get; set; }
        public bool FreezeAllButtonUsed { get; set; }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.IsCrewKiller())) == 1)
            {
                Utils.Rpc(CustomRPC.IcenbergWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Glitch Edition");
            IcenbergWins = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var icenbergTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            icenbergTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = icenbergTeam;
        }

        public void Update(HudManager __instance)
        {
            if (HudManager.Instance?.Chat != null)
            {
                foreach (var bubble in HudManager.Instance.Chat.chatBubblePool.activeChildren)
                {
                    if (bubble.Cast<ChatBubble>().NameText != null &&
                        Player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                    {
                        bubble.Cast<ChatBubble>().NameText.color = Color;
                    }
                }
            }

            FixedUpdate(__instance);
        }

        public void FixedUpdate(HudManager __instance)
        {
            KillButtonHandler.KillButtonUpdate(this, __instance);

            FreezeButtonHandler.FreezeButtonUpdate(this, __instance);

            FreezeButtonHandler.FreezeAllButtonUpdate(this, __instance);

            if (__instance.KillButton != null && Player.Data.IsDead)
                __instance.KillButton.SetTarget(null);

            if (FreezeButton != null && Player.Data.IsDead)
                FreezeButton.SetTarget(null);
        }

        public bool UseAbility(KillButton __instance)
        {
            if (__instance == FreezeButton)
            {
                FreezeButtonHandler.FreezeButtonPress(this);
                return false;
            }
            if (__instance == FreezeAllButton)
            {
                FreezeButtonHandler.FreezeAllButtonPress(this);
                return false;
            }
            KillButtonHandler.KillButtonPress(this);

            return false;
        }

        public void RpcSetFreezed(PlayerControl freezed)
        {
            //Utils.Rpc(CustomRPC.Freeze, Player.PlayerId, freezed.PlayerId);
            Coroutines.Start(AbilityCoroutineIcenberg.Freeze(this, freezed));
            //SetFreezed(freezed);
        }        
        public void RpcSetFreezedAll()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != this.Player && !player.Data.Disconnected && !player.Data.IsDead)
                {
                    Coroutines.Start(AbilityCoroutineIcenberg.Freeze(this, player));
                }
            }
        }
        public static class AbilityCoroutineIcenberg
        {
            public static Dictionary<byte, DateTime> tickDictionary = new();

            public static IEnumerator Freeze(Icenberg __instance, PlayerControl freezePlayer)
            {
                __instance.LastFreeze = DateTime.UtcNow;
                Utils.Rpc(CustomRPC.Freeze, PlayerControl.LocalPlayer.PlayerId, freezePlayer.PlayerId);

                if (!Utils.AbilityUsed(PlayerControl.LocalPlayer)) yield break;

                Debug.Log("Freeze activation time: " + __instance.LastFreeze);

                var freezeText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                freezeText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                freezeText.Text = $"{__instance.ColorString}Freezing {freezePlayer.Data.PlayerName} ({CustomGameOptions.FreezeDuration}s)</color>";
                PlayerControl.LocalPlayer.myTasks.Insert(0, freezeText);
                //Icenberg couldnt kill while freeze
                //Coroutines.Start(DisableAbility.StopAbility(CustomGameOptions.FreezeDuration));
                __instance.FreezeTarget = freezePlayer;

                while (true)
                {
                    var elapsedTime = (float)(DateTime.UtcNow - __instance.LastFreeze).TotalSeconds;
                    var remainingTime = CustomGameOptions.FreezeDuration - elapsedTime;
                    Debug.Log($"Total freeze time elapsed: {elapsedTime}s");

                    if (__instance.Player.Data.IsDead || remainingTime <= 0)// powinien tam byc freezeplayer a nie instance if (freezePlayer.Data.IsDead || remainingTime <= 0)
                    {
                        Debug.Log("Freeze duration expired or player is dead. Unfreezing...");
                        break;
                    }

                    freezeText.Text = $"{__instance.ColorString}Freezing {freezePlayer.Data.PlayerName} ({Math.Round(remainingTime)}s)</color>";
                    __instance.FreezeButton.SetCoolDown(remainingTime, CustomGameOptions.FreezeDuration);
                    yield return new WaitForSeconds(0.5f);
                }

                PlayerControl.LocalPlayer.myTasks.Remove(freezeText);
                __instance.LastFreeze = DateTime.UtcNow;
                __instance.FreezeTarget = null;
                Debug.Log("Exiting freeze loop");
            }
        }

        public static class KillButtonHandler
        {
            public static void KillButtonUpdate(Icenberg __gInstance, HudManager __instance)
            {
                if (!__gInstance.Player.Data.IsImpostor() && Rewired.ReInput.players.GetPlayer(0).GetButtonDown(8))
                    __instance.KillButton.DoClick();

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);
                __instance.KillButton.SetCoolDown(
                    CustomGameOptions.GlitchKillCooldown -
                    (float)(DateTime.UtcNow - __gInstance.LastKill).TotalSeconds,
                    CustomGameOptions.GlitchKillCooldown);

                __instance.KillButton.SetTarget(null);
                __gInstance.KillTarget = null;

                if (__instance.KillButton.isActiveAndEnabled && __gInstance.Player.moveable)
                {
                    if ((CamouflageUnCamouflage.IsCamoed && CustomGameOptions.CamoCommsKillAnyone) || PlayerControl.LocalPlayer.IsHypnotised()) Utils.SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton);
                    else if (__gInstance.Player.IsLover()) Utils.SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.IsLover()).ToList());
                    else Utils.SetTarget(ref __gInstance.ClosestPlayer, __instance.KillButton);
                    __gInstance.KillTarget = __gInstance.ClosestPlayer;
                }

                __gInstance.KillTarget?.myRend().material.SetColor("_OutlineColor", __gInstance.Color);
            }

            public static void KillButtonPress(Icenberg __gInstance)
            {
                if (__gInstance.KillTarget != null)
                {
                    var interact = Utils.Interact(__gInstance.Player, __gInstance.KillTarget, true);
                    if (interact[4])
                    {
                        return;
                    }
                    else if (interact[0])
                    {
                        __gInstance.LastKill = DateTime.UtcNow;
                        return;
                    }
                    else if (interact[1])
                    {
                        __gInstance.LastKill = DateTime.UtcNow;
                        __gInstance.LastKill = __gInstance.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.GlitchKillCooldown);
                        return;
                    }
                    else if (interact[2])
                    {
                        __gInstance.LastKill = DateTime.UtcNow;
                        __gInstance.LastKill = __gInstance.LastKill.AddSeconds(CustomGameOptions.VestKCReset - CustomGameOptions.GlitchKillCooldown);
                        return;
                    }
                    else if (interact[3])
                    {
                        return;
                    }
                    return;
                }
            }
        }

        public static class FreezeButtonHandler
        {
            public static void FreezeButtonUpdate(Icenberg __gInstance, HudManager __instance)
            {
                if (__gInstance.FreezeButton == null)
                {
                    __gInstance.FreezeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.FreezeButton.gameObject.SetActive(true);
                    __gInstance.FreezeButton.graphic.enabled = true;
                }

                __gInstance.FreezeButton.graphic.sprite = FreezeSprite;

                __gInstance.FreezeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);
                if (__instance.UseButton != null)
                {
                    __gInstance.FreezeButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                        __instance.UseButton.transform.position.y, __instance.UseButton.transform.position.z);
                }
                else
                {
                    __gInstance.FreezeButton.transform.position = new Vector3(
                        Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f,
                        __instance.PetButton.transform.position.y, __instance.PetButton.transform.position.z);
                }
                __gInstance.IsUsingFreeze = true;
                if (__gInstance.IsUsingFreeze)
                {
                    __gInstance.FreezeButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.FreezeButton.graphic.color = Palette.EnabledColor;
                }
                else if (!__gInstance.FreezeButton.isCoolingDown && __gInstance.Player.moveable)
                {
                    __gInstance.FreezeButton.isCoolingDown = false;
                    __gInstance.FreezeButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.FreezeButton.graphic.color = Palette.EnabledColor;
                    if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToU bb/disperse/mimic/freeze")) __gInstance.FreezeButton.DoClick();
                }
                else
                {
                    __gInstance.FreezeButton.isCoolingDown = true;
                    __gInstance.FreezeButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.FreezeButton.graphic.color = Palette.DisabledClear;
                }
                if (__gInstance.IsUsingFreeze)
                {
                    __gInstance.FreezeButton.SetCoolDown(
                        CustomGameOptions.FreezeCooldown -
                        (float)(DateTime.UtcNow - __gInstance.LastFreeze).TotalSeconds,
                        CustomGameOptions.FreezeCooldown);
                }
            }
            public static void FreezeAllButtonUpdate(Icenberg __gInstance, HudManager __instance)
            {
                if (__gInstance.FreezeAllButton == null)
                {
                    __gInstance.FreezeAllButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.FreezeAllButton.gameObject.SetActive(true);
                    __gInstance.FreezeAllButton.graphic.enabled = true;
                }

                __gInstance.FreezeAllButton.graphic.sprite = BlizzardSprite;

                __gInstance.FreezeAllButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);

                __gInstance.FreezeAllButton.transform.position = new Vector3(__gInstance.FreezeButton.transform.position.x,
                    __gInstance.FreezeAllButton.transform.position.y, __instance.ReportButton.transform.position.z);

                if (__gInstance.FreezeAllButtonUsed)
                {
                    __gInstance.FreezeAllButton.graphic.color = Palette.DisabledClear;
                    __gInstance.FreezeAllButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.FreezeAllButton.gameObject.SetActive(false);

                    return;
                }
                __gInstance.IsUsingFreeze = true;
                if (__gInstance.IsUsingFreeze)
                {
                    __gInstance.FreezeAllButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.FreezeAllButton.graphic.color = Palette.EnabledColor;
                }
                else if (!__gInstance.FreezeAllButton.isCoolingDown && __gInstance.Player.moveable)
                {
                    __gInstance.FreezeAllButton.isCoolingDown = false;
                    __gInstance.FreezeAllButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.FreezeAllButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __gInstance.FreezeAllButton.isCoolingDown = true;
                    __gInstance.FreezeAllButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.FreezeAllButton.graphic.color = Palette.DisabledClear;
                }

                if (__gInstance.IsUsingFreeze && !__gInstance.FreezeAllButtonUsed)
                {
                    __gInstance.FreezeAllButton.SetCoolDown(
                        CustomGameOptions.FreezeCooldown -
                        (float)(DateTime.UtcNow - __gInstance.LastFreeze).TotalSeconds,
                        CustomGameOptions.FreezeCooldown);
                }
                if (!__gInstance.FreezeAllButtonUsed)
                {
                    __gInstance.FreezeAllButton.graphic.color = Palette.EnabledColor;
                    __gInstance.FreezeAllButton.graphic.material.SetFloat("_Desat", 0f);
                    return;
                }
            }
            public static void FreezeButtonPress(Icenberg __gInstance)
            {
                List<byte> freezeTargets = new List<byte>();
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != __gInstance.Player && !player.Data.Disconnected)
                    {
                        if (!player.Data.IsDead) freezeTargets.Add(player.PlayerId);
                        else
                        {
                            foreach (var body in Object.FindObjectsOfType<DeadBody>())
                            {
                                if (body.ParentId == player.PlayerId) freezeTargets.Add(player.PlayerId);
                            }
                        }
                    }
                }
                byte[] freezetargetIDs = freezeTargets.ToArray();
                var pk = new PlayerMenu((x) =>
                {
                    Debug.Log($"SET FREEZED {x.PlayerId}   {x.name}");
                    __gInstance.RpcSetFreezed(x);
                }, (y) =>
                {
                    return freezetargetIDs.Contains(y.PlayerId);
                });
                Coroutines.Start(pk.Open(0f, true));
            }
            public static void FreezeAllButtonPress(Icenberg __gInstance)
            {
                if (!__gInstance.FreezeAllButtonUsed)
                {
                    __gInstance.RpcSetFreezedAll();
                    //__gInstance.FreezeCount++;
                }
                    __gInstance.FreezeAllButtonUsed = true;
            }
        }
    }
}