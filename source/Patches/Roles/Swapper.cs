using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<(byte, bool)> ListOfActives = new List<(byte, bool)>();


        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Swap The Votes Of Two People" : "Zamien Glosy Dwoch Ludzi";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Swap two people's votes to save the Crew!" : "Zamien glosy dwóch ludzi by ocalic zaloge!";
            Color = Patches.Colors.Swapper;
            RoleType = RoleEnum.Swapper;
            AddToRoleHistory(RoleType);
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.CrewKillersContinue) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Data.IsImpostor()) > 0) return false;

            return true;
        }
    }
}