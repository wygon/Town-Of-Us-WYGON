using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;
using AmongUs.GameOptions;
using System;
using TownOfUs.Roles;
using TownOfUs;

namespace TownOfUs.NeutralRoles.VultureMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill

    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vulture);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Vulture>(PlayerControl.LocalPlayer);

            if (__instance == DestroyableSingleton<HudManager>.Instance.KillButton)
            {
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (!__instance.enabled) return false;
                if (!role.CurrentTarget) return false;
                var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                var abilityUsed = Utils.AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }

                Utils.Rpc(CustomRPC.VultureEat, PlayerControl.LocalPlayer.PlayerId, playerId);

                Coroutines.Start(Coroutine.EatCoroutine(role.CurrentTarget, role));
                role.Cooldown = CustomGameOptions.VultureCD;
                return false;
            }

            return true;
        }
    }
}