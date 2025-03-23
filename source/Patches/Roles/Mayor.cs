using System.Linq;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Mayor : Role
    {
        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Reveal Yourself To Save The Town" : "Ujawnij Sie Aby Ocalic Miasto";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Reveal yourself when the time is right" : "Ujawnij sie w odpowiednim momencie";
            Color = Patches.Colors.Mayor;
            RoleType = RoleEnum.Mayor;
            AddToRoleHistory(RoleType);
            Revealed = false;
        }
        public bool Revealed { get; set; }

        public GameObject RevealButton = new GameObject();

        internal override bool Criteria()
        {
            return Revealed && !Player.Data.IsDead || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            if (!Player.Data.IsDead) return Revealed || base.RoleCriteria();
            return false || base.RoleCriteria();
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0) return false;

            return true;
        }
    }
}