using TMPro;

namespace TownOfUs.Roles
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Maintain Important Systems On The Ship" : "Utrzymuj Wa¿ne Systemy Na Statku";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Vent around and fix sabotages" : "Ventuj i naprawiaj sabotaze";
            Color = Patches.Colors.Engineer;
            RoleType = RoleEnum.Engineer;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.MaxFixes;
        }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;
    }
}