using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Prosecutor : Role
    {
        public Prosecutor(PlayerControl player) : base(player)
        {
            Name = "Prosecutor";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Exile One Person Of Your Choosing" : "Wygnaj Wybrana Osobe";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Choose to exile anyone you want" : "Wybierz by wyrzucic osobe twojego uznania";
            Color = Patches.Colors.Prosecutor;
            RoleType = RoleEnum.Prosecutor;
            AddToRoleHistory(RoleType);
            StartProsecute = false;
            Prosecuted = false;
            HasProsecuted = false;
            ProsecuteThisMeeting = false;
        }
        public bool ProsecuteThisMeeting { get; set; }
        public bool Prosecuted { get; set; }
        public bool HasProsecuted { get; set; }
        public bool StartProsecute { get; set; }
        public PlayerVoteArea Prosecute { get; set; }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0 && !HasProsecuted) return false;

            return true;
        }
    }
}
