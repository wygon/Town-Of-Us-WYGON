using System;
using System.Collections.Generic;
using TownOfUs.CrewmateRoles.DetectiveMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Detective : Role
    {
        private KillButton _examineButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined { get; set; }
        public CrimeScene CurrentTarget;
        public CrimeScene InvestigatingScene;
        public List<byte> InvestigatedPlayers = new List<byte>();
        public List<GameObject> CrimeScenes = new List<GameObject>();

        public Detective(PlayerControl player) : base(player)
        {
            Name = "Detective";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Inspect Crime Scenes To Catch The Killer" : "Sprawdzaj Miejsca Zbrodni I Zlap Zabojcow";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Examine suspicious players to find evildoers" : "Sprawdzaj podejrzanych graczy by znalezc zloczynców";
            Color = Patches.Colors.Detective;
            LastExamined = DateTime.UtcNow;
            RoleType = RoleEnum.Detective;
            AddToRoleHistory(RoleType);
        }

        public KillButton ExamineButton
        {
            get => _examineButton;
            set
            {
                _examineButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.ExamineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}