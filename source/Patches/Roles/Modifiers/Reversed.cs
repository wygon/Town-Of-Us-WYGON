using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Reversed : Modifier, IVisualAlteration
    {
        public Reversed(PlayerControl player) : base(player)
        {
            Name = "Reversed";
            TaskText = () => "pesrever era noY";
            Color = Patches.Colors.Reversed;
            ModifierType = ModifierEnum.Reversed;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();

            if (Player == PlayerControl.LocalPlayer)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180);
                foreach (var bubble in GameObject.FindObjectsOfType<ChatBubble>())
                    bubble.transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            return true;
        }
    }
}
