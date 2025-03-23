using System;

namespace TownOfUs.Roles
{
    public class Blackmailer : Role
    {
        public KillButton _blackmailButton;
        
        public PlayerControl ClosestPlayer;
        public PlayerControl Blackmailed;
        public DateTime LastBlackmailed { get; set; }

        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Silence Crewmates During Meetings" : "Wycisz Crewmate Podczas Spotkan";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Silence a crewmate for the next meeting" : "Wycisz crewmate'a na nastepne spotkanie";
            Color = Patches.Colors.Impostor;
            LastBlackmailed = DateTime.UtcNow;
            RoleType = RoleEnum.Blackmailer;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public KillButton BlackmailButton
        {
            get => _blackmailButton;
            set
            {
                _blackmailButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = CustomGameOptions.BlackmailCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool CanSeeBlackmailed(byte playerId)
        {
            return !CustomGameOptions.BlackmailInvisible || Blackmailed?.PlayerId == playerId || Player.PlayerId == playerId || Utils.PlayerById(playerId).Data.IsDead;
        }
    }
}