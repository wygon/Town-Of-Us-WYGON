using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TownOfUs.CrewmateRoles.MedicMod;
using AmongUs.GameOptions;

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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vulture)) return;

            var role = Role.GetRole<Vulture>(PlayerControl.LocalPlayer);

            if (role.CleanButton == null)
            {
                role.CleanButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CleanButton.graphic.enabled = true;
                role.CleanButton.gameObject.SetActive(false);
            }

            role.CleanButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.CleanButton.graphic.sprite = TownOfUs.EatSprite;

            if (role.EatCountText == null)
            {
                role.EatCountText = Object.Instantiate(__instance.KillButton.cooldownTimerText, role.CleanButton.transform);
                role.EatCountText.gameObject.SetActive(false);
                role.EatCountText.transform.localPosition = new Vector3(
                    role.EatCountText.transform.localPosition.x + 0.26f,
                    role.EatCountText.transform.localPosition.y + 0.29f,
                    role.EatCountText.transform.localPosition.z
                );
                role.EatCountText.transform.localScale = role.EatCountText.transform.localScale * 0.65f;
                role.EatCountText.alignment = TMPro.TextAlignmentOptions.Right;
                role.EatCountText.fontStyle = TMPro.FontStyles.Bold;
            }

            role.EatCountText.text = $"{role.eatenBodies}/{CustomGameOptions.VultureEatCount}";

            role.EatCountText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead)
                && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver)
                && PlayerControl.LocalPlayer.CanMove;

            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] { "Players", "Ghost" }));
            var killButton = role.CleanButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();
                if (!(Vector2.Distance(truePosition, component.TruePosition) <= maxDistance)) continue;
                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            role.CleanButton.SetCoolDown(role.VultureTimer(), CustomGameOptions.VultureKillCooldown);

            if (CustomGameOptions.VultureRememberArrows && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x =>
                    Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(CustomGameOptions.VultureRememberArrowDelay) < System.DateTime.UtcNow));

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
        }
    }
}
