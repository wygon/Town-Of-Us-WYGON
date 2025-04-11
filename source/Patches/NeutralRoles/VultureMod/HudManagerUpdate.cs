using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using AmongUs.GameOptions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;
using TownOfUs;

namespace TownOfUs.NeutralRoles.VultureMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Arrow => TownOfUs.Arrow;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vulture)) return;

            var role = Role.GetRole<Vulture>(PlayerControl.LocalPlayer);

            if (role.EatenBodies == null)
            {
                role.EatenBodies = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.EatenBodies.gameObject.SetActive(false);
                role.EatenBodies.transform.localPosition = new Vector3(
                    role.EatenBodies.transform.localPosition.x + 0.26f,
                    role.EatenBodies.transform.localPosition.y + 0.29f,
                    role.EatenBodies.transform.localPosition.z
                );
                role.EatenBodies.transform.localScale = role.EatenBodies.transform.localScale * 0.65f;
                role.EatenBodies.alignment = TMPro.TextAlignmentOptions.Right;
                role.EatenBodies.fontStyle = TMPro.FontStyles.Bold;
            }

            role.EatenBodies.text = $"{role.BodiesEaten}/{CustomGameOptions.VultureBodies}";

            role.EatenBodies.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;

            var killButton = __instance.KillButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            var allBodies = Object.FindObjectsOfType<DeadBody>();

            foreach (var body in allBodies.Where(x => Vector2.Distance(x.TruePosition, truePosition) <= maxDistance))
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);
                if (!(distance < closestDistance)) continue;

                closestBody = body;
                closestDistance = distance;
            }

            foreach (var arrow in role.BodyArrows)
            {
                arrow.Value.image.color = Patches.Colors.Vulture;
            }

            if (CustomGameOptions.VultureArrow && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x =>
                    Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(CustomGameOptions.VultureArrowDelay) < System.DateTime.UtcNow));

                foreach (var bodyArrow in role.BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    {
                        role.DestroyArrow(bodyArrow);
                    }
                }

                foreach (var body in validBodies)
                {
                    if (!role.BodyArrows.ContainsKey(body.ParentId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        role.BodyArrows.Add(body.ParentId, arrow);
                    }
                    role.BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else
            {
                if (role.BodyArrows.Count != 0)
                {
                    role.BodyArrows.Values.DestroyAll();
                    role.BodyArrows.Clear();
                }
            }

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.EatTimer(), CustomGameOptions.VultureCD);
            KillButtonTarget.SetTarget(killButton, closestBody, role);
        }
    }
}
