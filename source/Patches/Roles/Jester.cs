using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;
        public bool SpawnedAs = true;


        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Get Voted Out" : "Zostan Wyglosowany";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? (SpawnedAs ? "Get voted out!\nFake Tasks:" : "Your target was killed. Now you get voted out!\nFake Tasks:") : (SpawnedAs ? "Zostan wyglosowany!\nFake Tasks:" : "Twoj cel zostal zabity. Teraz zostan wyglosowany!!\nFake Tasks:");
            Color = Patches.Colors.Jester;
            RoleType = RoleEnum.Jester;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var jestTeam = new List<PlayerControl>();
            jestTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = jestTeam;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (!VotedOut || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            VotedOut = true;
        }
    }
}