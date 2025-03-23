namespace TownOfUs.Roles
{
    public class Janitor : Role
    {
        public KillButton _cleanButton;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Clean Up Bodies" : "Sprzataj ciala";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Clean bodies to prevent Crewmates from discovering them" : "Wyczysc ciala by powstrzymac Crewmate'ów od odkrycia ich";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Janitor;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }

        public DeadBody CurrentTarget { get; set; }

        public KillButton CleanButton
        {
            get => _cleanButton;
            set
            {
                _cleanButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}