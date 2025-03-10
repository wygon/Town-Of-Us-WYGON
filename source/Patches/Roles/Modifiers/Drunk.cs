using System;

namespace TownOfUs.Roles.Modifiers
{
    public class Drunk : Modifier
    {
        public int RoundsLeft;
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => "I don't feel so good...";
            Color = Patches.Colors.Drunk;
            ModifierType = ModifierEnum.Drunk;
            RoundsLeft = CustomGameOptions.DrunkDuration;
        }
    }
}