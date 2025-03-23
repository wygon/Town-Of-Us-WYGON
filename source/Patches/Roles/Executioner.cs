using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Executioner : Role
    {
        public PlayerControl target;
        public bool TargetVotedOut;

        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 
                ? (target == null ? "You don't have a target for some reason... weird..." : $"Vote {target.name} Out") 
                : (target == null ? "Nie masz celu... dziwne..." : $"Wyglosuj {target.name}!");
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ?
                (target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Vote {target.name} out!\nFake Tasks:") :
                (target == null
                    ? "Nie masz celu z jakiegos powodu... dziwne..."
                    : $"Wyglosuj {target.name}!\nFake Tasks:");
            Color = Patches.Colors.Executioner;
            RoleType = RoleEnum.Executioner;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
            Scale = 1.4f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var exeTeam = new List<PlayerControl>();
            exeTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = exeTeam;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!TargetVotedOut || !target.Data.IsDead) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;
            TargetVotedOut = true;
        }
    }
}