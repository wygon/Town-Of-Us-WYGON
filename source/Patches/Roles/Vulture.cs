using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using TownOfUs.Roles;
using TownOfUs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Vulture : Role
    {
        public Vulture(PlayerControl player) : base(player)
        {
            Name = "Vulture";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Eat all bodies!" : "Zjedz wszystkie ciala!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? $"Eat {CustomGameOptions.VultureBodies} dead bodies to win!" : $"Zjedz {CustomGameOptions.VultureBodies} martwych ciał, by wygrac";
            Color = Patches.Colors.Vulture;
            Cooldown = CustomGameOptions.VultureCD;
            RoleType = RoleEnum.Vulture;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public DeadBody CurrentTarget;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int BodiesEaten = 0;
        public DateTime LastEaten { get; set; }
        public bool VultureWins { get; set; }
        public TMPro.TextMeshPro EatenBodies;


        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var vultureTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            vultureTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = vultureTeam;
        }

        public float EatTimer()
        {
            if (!coolingDown) return 0f;
            else if (!PlayerControl.LocalPlayer.inVent)
            {
                Cooldown -= Time.deltaTime;
                return Cooldown;
            }
            else return Cooldown;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (BodiesEaten < CustomGameOptions.VultureBodies) return true;
            Utils.Rpc(CustomRPC.VultureWin, Player.PlayerId);
            Wins();
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            VultureWins = true;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }

        public float VultureTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastEaten;
            var num = CustomGameOptions.VultureCD * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}