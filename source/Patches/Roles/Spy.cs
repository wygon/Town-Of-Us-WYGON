namespace TownOfUs.Roles
{
    public class Spy : Role
    {
        public KillButton _vitalsButton;

        public Spy(PlayerControl player) : base(player)
        {
            Name = "Spy";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Snoop Around And Find Stuff Out" : "Wesz Dookola";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Gain extra information on the Admin Table" : "Zdobac dodatkowe informacje na stole w Adminie";
            Color = Patches.Colors.Spy;
            RoleType = RoleEnum.Spy;
            AddToRoleHistory(RoleType);
        }
        public KillButton VitalsButton
        {
            get => _vitalsButton;
            set
            {
                _vitalsButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}