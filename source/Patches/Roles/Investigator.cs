using System.Collections.Generic;
using TownOfUs.CrewmateRoles.InvestigatorMod;

namespace TownOfUs.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();


        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Find All Impostors By Examining Footprints" : "Znajdz Impostorow Po Ich Krokach";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "You can see everyone's footprints, and check bodies" : "Mozesz widziec wszystkich slady i sprawdzac ciala";
            Color = Patches.Colors.Investigator;
            RoleType = RoleEnum.Investigator;
            AddToRoleHistory(RoleType);
            Scale = 1.4f;
        }
    }
}