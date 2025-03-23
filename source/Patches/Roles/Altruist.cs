namespace TownOfUs.Roles
{
    public class Altruist : Role
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;

        public bool ReviveUsed;
        
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Sacrifice Yourself To Save Another" : "Poswiec sie aby ocalic kumpla";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Revive a dead body at the cost of your own life" : "Wskrzes martwego gracza kosztem wlasnego zycia";
            Color = Patches.Colors.Altruist;
            RoleType = RoleEnum.Altruist;
            AddToRoleHistory(RoleType);
        }
    }
}