//using HarmonyLib;
//using TownOfUs.CrewmateRoles.InvestigatorMod;
//using TownOfUs.CrewmateRoles.SnitchMod;
//using TownOfUs.CrewmateRoles.TrapperMod;
//using TownOfUs.Roles;
//using UnityEngine;
//using System;
//using TownOfUs.Extensions;
//using TownOfUs.CrewmateRoles.ImitatorMod;
//using AmongUs.GameOptions;
//using TownOfUs.Roles.Modifiers;
//using TownOfUs.ImpostorRoles.BomberMod;
//using TownOfUs.CrewmateRoles.AurialMod;
//using TownOfUs.Patches.ScreenEffects;
//using TownOfUs.Roles.Horseman;
//using Reactor.Utilities;
//using System.Collections.Generic;
//using System.Linq;
//using Reactor.Utilities.Extensions;

//namespace TownOfUs.NeutralRoles.CursedSoulMod
//{
//    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
//    public class PerformKill
//    {
//        public static Sprite Sprite => TownOfUs.Arrow;
//        public static bool Prefix(KillButton __instance)
//        {
//            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.CursedSoul);
//            if (!flag) return true;
//            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
//            if (!PlayerControl.LocalPlayer.CanMove) return false;
//            var role = Role.GetRole<CursedSoul>(PlayerControl.LocalPlayer);
//            if (role.SoulSwapTimer() != 0) return false;

//            if (role.ClosestPlayer == null) return false;
//            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
//            var flag3 = distBetweenPlayers <
//                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
//            if (!flag3) return false;
//            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
//            if (interact[4] == true)
//            {
//                PlayerControl player;
//                var otherPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != role.ClosestPlayer.PlayerId && x.PlayerId != PlayerControl.LocalPlayer.PlayerId).ToList();
//                if (UnityEngine.Random.RandomRangeInt(1, 101) > CustomGameOptions.SoulSwapAccuracy || otherPlayers.Count == 0)
//                {
//                    player = role.ClosestPlayer;
//                    if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, player.PlayerId, (byte)role.RoleType, (byte)0);
//                }
//                else
//                {
//                    player = otherPlayers[UnityEngine.Random.RandomRangeInt(0, otherPlayers.Count)];
//                    if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, player.PlayerId, (byte)role.RoleType, (byte)1);
//                }
//                if (PlayerControl.LocalPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, PlayerControl.LocalPlayer.PlayerId, (byte)role.RoleType, (byte)2);
//                if (player.Data.IsImpostor() && !CustomGameOptions.SoulSwapImp) Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
//                else
//                {
//                    Utils.Rpc(CustomRPC.SoulSwap, PlayerControl.LocalPlayer.PlayerId, player.PlayerId);
//                    SoulSwap(PlayerControl.LocalPlayer, player);
//                }
//            }
//            if (interact[0] == true)
//            {
//                role.LastSwapped = DateTime.UtcNow;
//                return false;
//            }
//            else if (interact[1] == true)
//            {
//                role.LastSwapped = DateTime.UtcNow;
//                role.LastSwapped = role.LastSwapped.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SoulSwapCooldown);
//                return false;
//            }
//            else if (interact[3] == true) return false;
//            return false;
//        }

//        public static void SoulSwap(PlayerControl cursedSoul, PlayerControl target)
//        {
//            var targetRole = Utils.GetRole(target);
//            var cursedSoulRole = Role.GetRole<CursedSoul>(cursedSoul);
//            if (target == PlayerControl.LocalPlayer)
//            {
//                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.CursedSoul));
//                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Were Swapped!" : "Zostales Zamieniony!", 1000 * CustomGameOptions.NotificationDuration);
//            }

//            var swapImp = true;
//            var swapNeut = true;
//            var cursedSoulBread = cursedSoulRole.BreadLeft;
//            var targetBread = Role.GetRole(target).BreadLeft;
//            var targetLastBlood = Role.GetRole(target).LastBlood;
//            var cursedSoulFactionOverride = cursedSoulRole.FactionOverride;
//            var targetFactionOverride = Role.GetRole(target).FactionOverride;

//            Role newCursedSoulRole;

//            switch (targetRole)
//            {
//                case RoleEnum.Sheriff:
//                case RoleEnum.Engineer:
//                case RoleEnum.Mayor:
//                case RoleEnum.Swapper:
//                case RoleEnum.Investigator:
//                case RoleEnum.Medic:
//                case RoleEnum.Seer:
//                case RoleEnum.Spy:
//                case RoleEnum.Snitch:
//                case RoleEnum.Altruist:
//                case RoleEnum.Vigilante:
//                case RoleEnum.Veteran:
//                case RoleEnum.Crewmate:
//                case RoleEnum.Tracker:
//                case RoleEnum.Hunter:
//                case RoleEnum.Transporter:
//                case RoleEnum.Medium:
//                case RoleEnum.Mystic:
//                case RoleEnum.Trapper:
//                case RoleEnum.Detective:
//                case RoleEnum.Imitator:
//                case RoleEnum.VampireHunter:
//                case RoleEnum.Prosecutor:
//                case RoleEnum.Oracle:
//                case RoleEnum.Aurial:
//                case RoleEnum.Inspector:
//                case RoleEnum.Monarch:
//                case RoleEnum.TavernKeeper:
//                case RoleEnum.Undercover:
//                case RoleEnum.Lookout:
//                case RoleEnum.Deputy:
//                case RoleEnum.Bodyguard:
//                case RoleEnum.Crusader:
//                case RoleEnum.Cleric:
//                case RoleEnum.Sage:

//                    swapImp = false;
//                    swapNeut = false;

//                    break;

//                case RoleEnum.Jester:
//                case RoleEnum.Executioner:
//                case RoleEnum.Arsonist:
//                case RoleEnum.Amnesiac:
//                case RoleEnum.Glitch:
//                case RoleEnum.Juggernaut:
//                case RoleEnum.Survivor:
//                case RoleEnum.GuardianAngel:
//                case RoleEnum.Plaguebearer:
//                case RoleEnum.Pestilence:
//                case RoleEnum.Baker:
//                case RoleEnum.Famine:
//                case RoleEnum.Berserker:
//                case RoleEnum.War:
//                case RoleEnum.SoulCollector:
//                case RoleEnum.Death:
//                case RoleEnum.Werewolf:
//                case RoleEnum.Doomsayer:
//                case RoleEnum.Vampire:
//                case RoleEnum.Pirate:
//                case RoleEnum.Inquisitor:
//                case RoleEnum.SerialKiller:
//                case RoleEnum.Witch:
//                case RoleEnum.CursedSoul:
//                case RoleEnum.JKNecromancer:
//                case RoleEnum.Jackal:

//                    swapImp = false;

//                    break;
//                default:
//                    if (target.Is(Faction.Crewmates))
//                    {
//                        swapImp = false;
//                        swapNeut = false;
//                    }
//                    else if (!target.Is(Faction.Impostors))
//                    {
//                        swapImp = false;
//                    }
//                    break;
//            }

//            newCursedSoulRole = Role.GetRole(target);
//            newCursedSoulRole.Player = cursedSoul;

//            if (targetRole == RoleEnum.Aurial && PlayerControl.LocalPlayer == target)
//            {
//                var aurial = Role.GetRole<Aurial>(target);
//                aurial.NormalVision = true;
//                SeeAll.AllToNormal();
//                CameraEffect.singleton.materials.Clear();
//            }

//            if (PlayerControl.LocalPlayer == target)
//            {
//                HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
//                if (targetRole == RoleEnum.Glitch)
//                {
//                    Role.GetRole<Glitch>(target).Reset();
//                }
//                if (targetRole == RoleEnum.Transporter)
//                {
//                    var transporter = Role.GetRole<Transporter>(target);
//                    if (transporter.TransportList != null)
//                    {
//                        transporter.TransportList.Toggle();
//                        transporter.TransportList.gameObject.SetActive(false);
//                        transporter.TransportList.gameObject.DestroyImmediate();
//                    }
//                    transporter.TransportList = null;
//                    transporter.PressedButton = false;
//                    transporter.TransportPlayer1 = null;
//                    transporter.HighlightedPlayer = null;
//                    transporter.PlayerIndex = 0;
//                }
//            }

//            if (targetRole == RoleEnum.Investigator) Footprint.DestroyAll(Role.GetRole<Investigator>(target));

//            if (targetRole == RoleEnum.Snitch) CompleteTask.Postfix(cursedSoul);

//            Role.RoleDictionary.Remove(cursedSoul.PlayerId);
//            Role.RoleDictionary.Remove(target.PlayerId);
//            Role.RoleDictionary.Add(cursedSoul.PlayerId, newCursedSoulRole);

//            if (!(cursedSoul.Is(RoleEnum.Crewmate) || cursedSoul.Is(RoleEnum.Impostor))) newCursedSoulRole.RegenTask();

//            newCursedSoulRole.AddToRoleHistory(newCursedSoulRole.RoleType);

//            switch (CustomGameOptions.SwappedBecomes)
//            {
//                case SwappedBecomes.CursedSoul:
//                    var newCursedSoul = new CursedSoul(target);
//                    newCursedSoul.WasSwapped = true;
//                    newCursedSoul.BreadLeft = targetBread;
//                    newCursedSoul.LastBlood = targetLastBlood;
//                    target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                    RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                    break;
//                case SwappedBecomes.Amnesiac:
//                    var newAmnesiac = new Amnesiac(target);
//                    newAmnesiac.WasSwapped = true;
//                    newAmnesiac.BreadLeft = targetBread;
//                    newAmnesiac.LastBlood = targetLastBlood;
//                    target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                    RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                    break;
//                case SwappedBecomes.Crewmate:
//                    var newCrewmate = new Crewmate(target);
//                    newCrewmate.BreadLeft = targetBread;
//                    newCrewmate.LastBlood = targetLastBlood;
//                    target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                    RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                    break;
//                case SwappedBecomes.Jester:
//                    var newJester = new Jester(target);
//                    newJester.WasSwapped = true;
//                    newJester.BreadLeft = targetBread;
//                    newJester.LastBlood = targetLastBlood;
//                    target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                    RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                    break;
//                case SwappedBecomes.Survivor:
//                    var newSurvivor = new Survivor(target);
//                    newSurvivor.WasSwapped = true;
//                    newSurvivor.BreadLeft = targetBread;
//                    newSurvivor.LastBlood = targetLastBlood;
//                    target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                    RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                    break;
//                case SwappedBecomes.DefaultRole:
//                    if (swapImp)
//                    {
//                        var newDefaultRole = new Impostor(target);
//                        newDefaultRole.BreadLeft = targetBread;
//                        newDefaultRole.LastBlood = targetLastBlood;
//                        target.Data.Role.TeamType = RoleTeamTypes.Impostor;
//                        RoleManager.Instance.SetRole(target, RoleTypes.Impostor);
//                    }
//                    else if (swapNeut)
//                    {
//                        var newDefaultRole = new Survivor(target);
//                        newDefaultRole.WasSwapped = true;
//                        newDefaultRole.BreadLeft = targetBread;
//                        newDefaultRole.LastBlood = targetLastBlood;
//                        target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                        RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                    }
//                    else
//                    {
//                        var newDefaultRole = new Crewmate(target);
//                        newDefaultRole.BreadLeft = targetBread;
//                        newDefaultRole.LastBlood = targetLastBlood;
//                        target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                        RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                    }
//                    break;
//            }
//            if (PlayerControl.LocalPlayer.PlayerId == target.PlayerId) foreach (var button in GameObject.FindObjectsOfType<KillButton>())
//                {
//                    if (button != HudManager.Instance.KillButton) button.gameObject.Destroy();
//                }
//            Role.GetRole(target).ExtraButtons.Clear();
//            target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//            Role.GetRole(target).RegenTask();
//            if (target.Is(AbilityEnum.Assassin))
//            {
//                var Guesses = Ability.GetAbility<Assassin>(target).Guesses;
//                var newAbility = new Assassin(cursedSoul);
//                Ability.AbilityDictionary.Remove(target.PlayerId);
//                newAbility.Guesses = Guesses;
//            }
//            if (swapImp == true)
//            {
//                cursedSoul.Data.Role.TeamType = RoleTeamTypes.Impostor;
//                RoleManager.Instance.SetRole(cursedSoul, RoleTypes.Impostor);
//                cursedSoul.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
//                if (CustomGameOptions.SwappedBecomes != SwappedBecomes.DefaultRole)
//                {
//                    target.Data.Role.TeamType = RoleTeamTypes.Crewmate;
//                    RoleManager.Instance.SetRole(target, RoleTypes.Crewmate);
//                }
//                foreach (var player in PlayerControl.AllPlayerControls)
//                {
//                    if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
//                    {
//                        player.nameText().color = Patches.Colors.Impostor;
//                    }
//                }
//                foreach (var player in PlayerControl.AllPlayerControls)
//                {
//                    if (player.Data.IsImpostor() && !PlayerControl.LocalPlayer.Data.IsImpostor())
//                    {
//                        player.nameText().color = Patches.Colors.Crewmate;
//                    }
//                }
//            }

//            if (targetRole == RoleEnum.Snitch)
//            {
//                var snitchRole = Role.GetRole<Snitch>(cursedSoul);
//                snitchRole.ImpArrows.DestroyAll();
//                snitchRole.SnitchArrows.Values.DestroyAll();
//                snitchRole.SnitchArrows.Clear();
//                CompleteTask.Postfix(cursedSoul);
//                if (target.AmOwner)
//                    foreach (var player in PlayerControl.AllPlayerControls)
//                        player.nameText().color = Color.white;
//                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
//            }

//            else if (targetRole == RoleEnum.Sheriff)
//            {
//                var sheriffRole = Role.GetRole<Sheriff>(cursedSoul);
//                sheriffRole.LastKilled = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Medic)
//            {
//                var medicRole = Role.GetRole<Medic>(cursedSoul);
//            }

//            else if (targetRole == RoleEnum.Mayor)
//            {
//                var mayorRole = Role.GetRole<Mayor>(cursedSoul);
//                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
//            }

//            else if (targetRole == RoleEnum.Prosecutor)
//            {
//                var prosRole = Role.GetRole<Prosecutor>(cursedSoul);
//                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
//            }

//            else if (targetRole == RoleEnum.Vigilante)
//            {
//                var vigiRole = Role.GetRole<Vigilante>(cursedSoul);
//                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
//            }

//            else if (targetRole == RoleEnum.Veteran)
//            {
//                var vetRole = Role.GetRole<Veteran>(cursedSoul);
//                vetRole.LastAlerted = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Tracker)
//            {
//                var trackerRole = Role.GetRole<Tracker>(cursedSoul);
//                trackerRole.LastTracked = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.VampireHunter)
//            {
//                var vhRole = Role.GetRole<VampireHunter>(cursedSoul);
//                vhRole.LastStaked = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Detective)
//            {
//                var detectiveRole = Role.GetRole<Detective>(cursedSoul);
//                detectiveRole.LastExamined = DateTime.UtcNow;
//                detectiveRole.CurrentTarget = null;
//            }

//            else if (targetRole == RoleEnum.Mystic)
//            {
//                var mysticRole = Role.GetRole<Mystic>(cursedSoul);
//                mysticRole.BodyArrows.Values.DestroyAll();
//                mysticRole.BodyArrows.Clear();
//                mysticRole.InteractingPlayers.Clear();
//                mysticRole.PlayersInteracted.Clear();
//                mysticRole.VisionPlayer = byte.MaxValue;
//                mysticRole.UsedAbility = false;
//            }

//            else if (targetRole == RoleEnum.Transporter)
//            {
//                var tpRole = Role.GetRole<Transporter>(cursedSoul);
//                tpRole.PressedButton = false;
//                tpRole.MenuClick = false;
//                tpRole.LastMouse = false;
//                tpRole.TransportList = null;
//                tpRole.TransportPlayer1 = null;
//                tpRole.TransportPlayer2 = null;
//                tpRole.LastTransported = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Medium)
//            {
//                var medRole = Role.GetRole<Medium>(cursedSoul);
//                medRole.LastMediated = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Seer)
//            {
//                var seerRole = Role.GetRole<Seer>(cursedSoul);
//                seerRole.LastInvestigated = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Oracle)
//            {
//                var oracleRole = Role.GetRole<Oracle>(cursedSoul);
//                oracleRole.Confessor = null;
//                oracleRole.LastConfessed = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Cleric)
//            {
//                var clericRole = Role.GetRole<Cleric>(cursedSoul);
//                clericRole.BarrieredPlayer = null;
//                clericRole.LastBarrier = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Crusader)
//            {
//                var crusaderRole = Role.GetRole<Crusader>(cursedSoul);
//                crusaderRole.LastFortified = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Bodyguard)
//            {
//                var bodyguardRole = Role.GetRole<Bodyguard>(cursedSoul);
//                bodyguardRole.GuardedPlayer = null;
//                bodyguardRole.LastGuard = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Aurial)
//            {
//                var aurialRole = Role.GetRole<Aurial>(cursedSoul);
//                aurialRole.LastRadiated = DateTime.UtcNow;
//                aurialRole.NormalVision = false;
//                if (cursedSoul.AmOwner) aurialRole.ApplyEffect();
//                aurialRole.Loaded = true;
//            }

//            else if (targetRole == RoleEnum.Arsonist)
//            {
//                var arsoRole = Role.GetRole<Arsonist>(cursedSoul);
//                arsoRole.LastDoused = DateTime.UtcNow;
//                if (arsoRole.DousedPlayers.Contains(cursedSoul.PlayerId)) arsoRole.DousedPlayers.Remove(cursedSoul.PlayerId);
//            }

//            else if (targetRole == RoleEnum.Survivor)
//            {
//                var survRole = Role.GetRole<Survivor>(cursedSoul);
//                survRole.LastVested = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.GuardianAngel)
//            {
//                var gaRole = Role.GetRole<GuardianAngel>(cursedSoul);
//                gaRole.LastProtected = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Glitch)
//            {
//                var glitchRole = Role.GetRole<Glitch>(cursedSoul);
//                glitchRole.LastKill = DateTime.UtcNow;
//                glitchRole.LastHack = DateTime.UtcNow;
//                glitchRole.LastMimic = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Juggernaut)
//            {
//                var juggRole = Role.GetRole<Juggernaut>(cursedSoul);
//                juggRole.LastKill = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.SerialKiller)
//            {
//                var skRole = Role.GetRole<SerialKiller>(cursedSoul);
//                skRole.LastKill = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Grenadier)
//            {
//                var grenadeRole = Role.GetRole<Grenadier>(cursedSoul);
//                grenadeRole.LastFlashed = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Morphling)
//            {
//                var morphlingRole = Role.GetRole<Morphling>(cursedSoul);
//                morphlingRole.LastMorphed = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Escapist)
//            {
//                var escapistRole = Role.GetRole<Escapist>(cursedSoul);
//                escapistRole.LastEscape = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Swooper)
//            {
//                var swooperRole = Role.GetRole<Swooper>(cursedSoul);
//                swooperRole.LastSwooped = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Venerer)
//            {
//                var venererRole = Role.GetRole<Venerer>(cursedSoul);
//                venererRole.LastCamouflaged = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Blackmailer)
//            {
//                var blackmailerRole = Role.GetRole<Blackmailer>(cursedSoul);
//                blackmailerRole.LastBlackmailed = DateTime.UtcNow;
//                blackmailerRole.Blackmailed = null;
//            }

//            else if (targetRole == RoleEnum.Miner)
//            {
//                var minerRole = Role.GetRole<Miner>(cursedSoul);
//                minerRole.LastMined = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Undertaker)
//            {
//                var dienerRole = Role.GetRole<Undertaker>(cursedSoul);
//                dienerRole.LastDragged = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Sniper)
//            {
//                var sniperRole = Role.GetRole<Sniper>(cursedSoul);
//                sniperRole.LastAim = DateTime.UtcNow;
//                sniperRole.AimedPlayer = null;
//            }

//            else if (targetRole == RoleEnum.Werewolf)
//            {
//                var wwRole = Role.GetRole<Werewolf>(cursedSoul);
//                wwRole.LastRampaged = DateTime.UtcNow;
//                wwRole.LastKilled = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Doomsayer)
//            {
//                var doomRole = Role.GetRole<Doomsayer>(cursedSoul);
//                doomRole.LastObserved = DateTime.UtcNow;
//                doomRole.LastObservedPlayer = null;
//            }

//            else if (targetRole == RoleEnum.Plaguebearer)
//            {
//                var plagueRole = Role.GetRole<Plaguebearer>(cursedSoul);
//                plagueRole.LastInfected = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Baker)
//            {
//                var bakerRole = Role.GetRole<Baker>(cursedSoul);
//                bakerRole.LastBreaded = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.SoulCollector)
//            {
//                var collectorRole = Role.GetRole<SoulCollector>(cursedSoul);
//                collectorRole.LastReaped = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Berserker)
//            {
//                var berserkerRole = Role.GetRole<Berserker>(cursedSoul);
//                berserkerRole.LastKill = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Pestilence)
//            {
//                var pestRole = Role.GetRole<Pestilence>(cursedSoul);
//                pestRole.LastKill = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Famine)
//            {
//                var famRole = Role.GetRole<Famine>(cursedSoul);
//                famRole.LastStarved = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.War)
//            {
//                var warRole = Role.GetRole<War>(cursedSoul);
//                warRole.LastKill = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Death)
//            {
//                var deathRole = Role.GetRole<Death>(cursedSoul);
//                deathRole.LastApocalypse = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Vampire)
//            {
//                var vampRole = Role.GetRole<Vampire>(cursedSoul);
//                vampRole.LastBit = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Trapper)
//            {
//                var trapperRole = Role.GetRole<Trapper>(cursedSoul);
//                trapperRole.LastTrapped = DateTime.UtcNow;
//                if (CustomGameOptions.TrapsRemoveOnNewRound) trapperRole.traps.ClearTraps();
//            }

//            else if (targetRole == RoleEnum.Bomber)
//            {
//                var bomberRole = Role.GetRole<Bomber>(cursedSoul);
//            }

//            else if (targetRole == RoleEnum.Monarch)
//            {
//                var monarchRole = Role.GetRole<Monarch>(cursedSoul);
//                monarchRole.LastKnighted = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Inspector)
//            {
//                var inspectorRole = Role.GetRole<Inspector>(cursedSoul);
//                inspectorRole.LastInspectedPlayer = null;
//                inspectorRole.LastInspected = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.TavernKeeper)
//            {
//                var tavernKeeperRole = Role.GetRole<TavernKeeper>(cursedSoul);
//                tavernKeeperRole.LastDrink = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Spy)
//            {
//                var spy = Role.GetRole<Spy>(cursedSoul);
//                spy.BuggedPlayers = new List<byte>();
//                spy.LastBugged = DateTime.UtcNow;
//                spy.Messages = new List<string>();
//                Utils.Rpc(CustomRPC.UnbugPlayers, PlayerControl.LocalPlayer.PlayerId);
//            }

//            else if (targetRole == RoleEnum.Lookout)
//            {
//                var lookoutRole = Role.GetRole<Lookout>(cursedSoul);
//                lookoutRole.LastWatched = DateTime.UtcNow;
//            }

//            else if (targetRole == RoleEnum.Witch)
//            {
//                var witch = Role.GetRole<Witch>(cursedSoul);
//                witch.LastControl = DateTime.UtcNow;
//                witch.ControledPlayer = null;
//                witch.RevealedPlayers = new List<byte>();
//            }

//            else if (targetRole == RoleEnum.Pirate)
//            {
//                var pirateRole = Role.GetRole<Pirate>(cursedSoul);
//                pirateRole.LastDueled = DateTime.UtcNow;
//                pirateRole.DueledPlayer = null;
//            }

//            else if (targetRole == RoleEnum.Inquisitor)
//            {
//                var inquisitorRole = Role.GetRole<Inquisitor>(cursedSoul);
//                inquisitorRole.LastAbility = DateTime.UtcNow;
//                if (inquisitorRole.heretics.Contains(cursedSoul.PlayerId)) inquisitorRole.heretics.Remove(cursedSoul.PlayerId);
//            }

//            else if (targetRole == RoleEnum.Demagogue)
//            {
//                var demagogueRole = Role.GetRole<Demagogue>(cursedSoul);
//                demagogueRole.LastConvince = DateTime.UtcNow;
//            }
//            else if (targetRole == RoleEnum.Occultist)
//            {
//                var occultistRole = Role.GetRole<Occultist>(cursedSoul);
//                occultistRole.LastMark = DateTime.UtcNow;
//            }
//            else if (targetRole == (RoleEnum)255)
//            {
//                var roleA = Role.GetRole<RoleA>(cursedSoul);
//                roleA.LastA = DateTime.UtcNow;
//                roleA.LastB = DateTime.UtcNow;
//                roleA.LastC = DateTime.UtcNow;
//                if (roleA.AbilityB0)
//                {
//                    Utils.Unmorph(target);
//                    target.myRend().color = Color.white;
//                    roleA.AbilityB0 = false;
//                    target.MyPhysics.ResetMoveState();
//                }
//                roleA.AbilityBActive = false;
//            }
//            else if (targetRole == (RoleEnum)253)
//            {
//                var roleC = Role.GetRole<RoleC>(cursedSoul);
//                roleC.LastA = DateTime.UtcNow;
//                roleC.AbilityA0 = byte.MaxValue;
//            }
//            else if (targetRole == (RoleEnum)252)
//            {
//                var roleD = Role.GetRole<RoleD>(cursedSoul);
//                roleD.LastA = DateTime.UtcNow;
//                foreach (var id in roleD.AbilityA0)
//                {
//                    var player = Utils.PlayerById(id);
//                    if (player == null) continue;
//                    player.Teleport(roleD.Variable0);
//                    player.Visible = true;
//                }
//                roleD.AbilityA0.Clear();
//            }
//            else if (targetRole == (RoleEnum)251)
//            {
//                var roleE = Role.GetRole<RoleE>(cursedSoul);
//                roleE.AbilityA0 = byte.MaxValue;
//            }
//            else if (targetRole == (RoleEnum)250)
//            {
//                var roleF = Role.GetRole<RoleF>(cursedSoul);
//                roleF.LastA = DateTime.UtcNow;
//            }
//            else if (targetRole == (RoleEnum)249)
//            {
//                var roleG = Role.GetRole<RoleG>(cursedSoul);
//                roleG.LastA = DateTime.UtcNow;
//                foreach (var obj in roleG.AbilityA0)
//                {
//                    obj.Destroy();
//                }
//                roleG.AbilityA0.Clear();
//                foreach (var obj in roleG.AbilityA1.Select(x => x.Item2))
//                {
//                    obj.Destroy();
//                }
//                roleG.AbilityA1.Clear();
//            }

//            else if (!(cursedSoul.Is(RoleEnum.Altruist) || cursedSoul.Is(RoleEnum.Amnesiac) || cursedSoul.Is(Faction.Impostors)))
//            {
//                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
//            }
//            DestroyableSingleton<HudManager>.Instance.KillButton.buttonLabelText.text = "";

//            Role.GetRole(cursedSoul).LastBlood = DateTime.UtcNow;
//            Role.GetRole(cursedSoul).LastBlood.AddSeconds(-CustomGameOptions.BloodDuration);
//            Role.GetRole(cursedSoul).Roleblocked = false;
//            Role.GetRole(cursedSoul).Reaped = false;
//            Role.GetRole(cursedSoul).BreadLeft = cursedSoulBread;
//            if (!cursedSoul.Is(RoleEnum.Vampire) && !cursedSoul.Is(RoleEnum.JKNecromancer) && !cursedSoul.Is(RoleEnum.Jackal))
//            {
//                Role.GetRole(cursedSoul).FactionOverride = cursedSoulFactionOverride;
//                Role.GetRole(target).FactionOverride = targetFactionOverride;
//            }
//            Role.GetRole(cursedSoul).RegenTask();
//            Role.GetRole(target).RegenTask();

//            var killsList = (newCursedSoulRole.Kills, newCursedSoulRole.CorrectKills, newCursedSoulRole.IncorrectKills, newCursedSoulRole.CorrectAssassinKills, newCursedSoulRole.IncorrectAssassinKills);
//            var newTargetRole = Role.GetRole(target);
//            newCursedSoulRole.Kills = newTargetRole.Kills;
//            newCursedSoulRole.CorrectKills = newTargetRole.CorrectKills;
//            newCursedSoulRole.IncorrectKills = newTargetRole.IncorrectKills;
//            newCursedSoulRole.CorrectAssassinKills = newTargetRole.CorrectAssassinKills;
//            newCursedSoulRole.IncorrectAssassinKills = newTargetRole.IncorrectAssassinKills;
//            newTargetRole.Kills = killsList.Kills;
//            newTargetRole.CorrectKills = killsList.CorrectKills;
//            newTargetRole.IncorrectKills = killsList.IncorrectKills;
//            newTargetRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
//            newTargetRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;

//            if (cursedSoul.Is(Faction.Impostors) && (!cursedSoul.Is(RoleEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor))
//            {
//                foreach (var snitch in Role.GetRoles(RoleEnum.Snitch))
//                {
//                    var snitchRole = (Snitch)snitch;
//                    if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
//                    {
//                        var gameObj = new GameObject();
//                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
//                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
//                        var renderer = gameObj.AddComponent<SpriteRenderer>();
//                        renderer.sprite = Sprite;
//                        arrow.image = renderer;
//                        gameObj.layer = 5;
//                        snitchRole.SnitchArrows.Add(cursedSoul.PlayerId, arrow);
//                    }
//                    else if (snitchRole.Revealed && PlayerControl.LocalPlayer == cursedSoul)
//                    {
//                        var gameObj = new GameObject();
//                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
//                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
//                        var renderer = gameObj.AddComponent<SpriteRenderer>();
//                        renderer.sprite = Sprite;
//                        arrow.image = renderer;
//                        gameObj.layer = 5;
//                        snitchRole.ImpArrows.Add(arrow);
//                    }
//                }
//            }
//        }
//    }
//}
