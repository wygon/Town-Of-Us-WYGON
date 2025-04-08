using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class President : Role
    {
        public List<byte> ExtraVotes = new List<byte>();

        public President(PlayerControl player) : base(player)
        {
            Name = "President";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Keep the votes" : "Zachowaj glosy";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Keep your votes to vote multiple times" : "Zachowaj swoje glosy, by glosowac wielokrotnie";
            Color = Patches.Colors.President;
            RoleType = RoleEnum.President;
            AddToRoleHistory(RoleType);
            VoteBank = CustomGameOptions.PresidentVoteBank;
        }

        public int VoteBank { get; set; }
        public bool SelfVote { get; set; }

        public bool VotedOnce { get; set; }

        public PlayerVoteArea Abstain { get; set; }

        public bool CanVote => VoteBank > 0 && !SelfVote;
    }
}