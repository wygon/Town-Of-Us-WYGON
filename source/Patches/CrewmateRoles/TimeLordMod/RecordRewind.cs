using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using TownOfUs.CrewmateRoles.AltruistMod;
using UnityEngine;
using Object = UnityEngine.Object;
using Reactor.Utilities.Extensions;
using Reactor.Utilities;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RecordRewind
    {
        public static bool rewinding = false;
        public static TimeLord whoIsRewinding;
        public static List<PointInTime> points = new List<PointInTime>();
        private static float deadTime;
        private static bool isDead;
        private static float recordTime => CustomGameOptions.RewindDuration;

        public static void Record()
        {
            if (points.Count > Mathf.Round(recordTime / Time.deltaTime)) points.RemoveAt(points.Count - 1);

            if (PlayerControl.LocalPlayer == null) return;

            Vector3 position;
            Vector2 velocity;
            if (!PlayerControl.LocalPlayer.moveable && points.Count > 0)
            {
                position = points[0].position;
                velocity = Vector2.zero;
            }
            else
            {
                position = PlayerControl.LocalPlayer.transform.position;
                velocity = PlayerControl.LocalPlayer.gameObject.GetComponent<Rigidbody2D>().velocity;
            }


            points.Insert(0, new PointInTime(
                position,
                velocity,
                Time.time
            ));

            if (PlayerControl.LocalPlayer.Data.IsDead && !isDead)
            {
                isDead = true;
                deadTime = Time.time;
            }
            else if (!PlayerControl.LocalPlayer.Data.IsDead && isDead)
            {
                isDead = false;
                deadTime = 0;
            }
        }

        public static void Rewind()
        {
            if (Minigame.Instance)
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }
            //System.Console.WriteLine("Rewinding...");
            //System.Console.Write(points.Count);

            if (points.Count > 2)
            {
                points.RemoveAt(0);
                points.RemoveAt(0);
                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }

                if (!PlayerControl.LocalPlayer.inVent)
                {
                    if (!PlayerControl.LocalPlayer.Collider.enabled)
                    {
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        PlayerControl.LocalPlayer.Collider.enabled = true;
                        PlayerControl.LocalPlayer.NetTransform.enabled = true;

                        Utils.Rpc(CustomRPC.FixAnimation, PlayerControl.LocalPlayer.PlayerId);
                    }


                    var currentPoint = points[0];

                    PlayerControl.LocalPlayer.transform.position = currentPoint.position;
                    if (SubmergedCompatibility.isSubmerged())
                    {
                        SubmergedCompatibility.ChangeFloor(currentPoint.position.y > -7);
                    }
                    PlayerControl.LocalPlayer.gameObject.GetComponent<Rigidbody2D>().velocity =
                        currentPoint.velocity * 3;

                    if (isDead && currentPoint.unix < deadTime && PlayerControl.LocalPlayer.Data.IsDead &&
                        CustomGameOptions.RewindRevive)
                    {
                        var player = PlayerControl.LocalPlayer;

                        ReviveBody(player);
                        //player.myTasks.RemoveAt(0);

                        deadTime = 0;
                        isDead = false;

                        Utils.Rpc(CustomRPC.RewindRevive, PlayerControl.LocalPlayer.PlayerId);
                    }
                }

                points.RemoveAt(0);
            }

            else
            {
                StartStop.StopRewind(whoIsRewinding);
            }
        }

        public static void ReviveBody(PlayerControl player)
        {

            var revived = new List<PlayerControl>();
            DeadBody deadbodynew = null;

            foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
            {
                if (deadBody.ParentId == player.PlayerId) { deadbodynew = deadBody; deadBody.gameObject.Destroy(); }
            }

            player.Revive();
            if (player.Is(Faction.Impostors)) RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.transform.position = new Vector2(deadbodynew.TruePosition.x, deadbodynew.TruePosition.y + 0.3636f);

            if (PlayerControl.LocalPlayer == player) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(deadbodynew.TruePosition.x, deadbodynew.TruePosition.y));

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }
            if (player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = Modifier.GetModifier<Lover>(player).OtherLover.Player;

                lover.Revive();
                if (lover.Is(Faction.Impostors)) RoleManager.Instance.SetRole(lover, RoleTypes.Impostor);
                else RoleManager.Instance.SetRole(lover, RoleTypes.Crewmate);
                Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                    {
                        lover.transform.position = new Vector2(deadBody.TruePosition.x, deadBody.TruePosition.y + 0.3636f);
                        if (PlayerControl.LocalPlayer == lover) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(deadBody.TruePosition.x, deadBody.TruePosition.y + 0.3636f));

                        if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == lover.PlayerId)
                        {
                            Patches.SubmergedCompatibility.ChangeFloor(lover.transform.position.y > -7);
                        }
                        deadBody.gameObject.Destroy();
                    }
                }
            }

            var body = Object.FindObjectsOfType<DeadBody>()
                .FirstOrDefault(b => b.ParentId == player.PlayerId);

            if (body != null)
                Object.Destroy(body.gameObject);
        }

        public static void Postfix()
        {
            if (rewinding)
                Rewind();
            else Record();

            foreach (var role in Role.GetRoles(RoleEnum.TimeLord))
            {
                var TimeLord = (TimeLord)role;
                if ((DateTime.UtcNow - TimeLord.StartRewind).TotalMilliseconds >
                    CustomGameOptions.RewindDuration * 1000f && TimeLord.FinishRewind < TimeLord.StartRewind)
                    StartStop.StopRewind(TimeLord);
            }
        }
    }
}