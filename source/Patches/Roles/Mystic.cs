using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public class Mystic : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Understand When And Where Kills Happen" : "Zrozum Gdzie I Kiedy Padaja Ciala";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Know When and Where Kills Happen" : "Wiedz kiedy i gdzie sa zabójstwa";
            Color = Patches.Colors.Mystic;
            RoleType = RoleEnum.Mystic;
            AddToRoleHistory(RoleType);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
    }
}