using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Vulture : Role
    {
        public Vulture(PlayerControl player) : base(player)
        {
            Name = "Vulture";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Eat the bodies" : "Zjadaj ciala";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? $"Eat {CustomGameOptions.VultureEatCount} dead bodies to win!" : $"Zjedz {CustomGameOptions.VultureEatCount} martwych ciał, by wygrac";
            Color = Patches.Colors.Vulture;
            RoleType = RoleEnum.Vulture;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public DeadBody CurrentTarget;
        public float Cooldown;
        public bool coolingDown => Cooldown > 0f;
        public int BodiesEaten = 0;
        public bool VultureWins = false;
        public DateTime LastEaten { get; set; }

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
    }
}

/*using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Vulture : Role
    {
        public KillButton _cleanButton;
        public int eatenBodies = 0;
        public bool vultureWin = false;
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public DateTime LastEaten { get; set; }
        public TMPro.TextMeshPro EatCountText;

        public Vulture(PlayerControl player) : base(player)
        {
            Name = "Vulture";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Eat the bodies" : "Zjedz ciala";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Eat enough body to win" : "Zjedz wystarczajaco duzo cial, aby wygrac";
            Color = Patches.Colors.Vulture;
            RoleType = RoleEnum.Vulture;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        public DeadBody CurrentTarget { get; set; }

        public KillButton CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var vultureTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            vultureTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = vultureTeam;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (eatenBodies < CustomGameOptions.VultureEatCount) return true;
            Utils.Rpc(CustomRPC.VultureWin, Player.PlayerId);
            Wins();
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            vultureWin = true;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null) Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null) Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }

        public float VultureTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastEaten;
            var num = CustomGameOptions.VultureKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
*/