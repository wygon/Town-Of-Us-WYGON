using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] NetworkedPlayerInfo player,
            ref float __result)
        {
     
            //__instance.MeetingBackground.
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (GameOptionsManager.Instance.currentHideNSeekGameOptions.useFlashlight)
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorFlashlightSize;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewmateFlashlightSize;
                }
                else
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorLightMod;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewLightMod;
                }
                return false;
            }

            if (player == null || player.IsDead)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            var switchSystem = GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? null : __instance.Systems[SystemTypes.Electrical]?.TryCast<SwitchSystem>();
            if (player.IsImpostor() || player._object.Is(RoleEnum.Glitch) ||
                player._object.Is(RoleEnum.Juggernaut) || player._object.Is(RoleEnum.Pestilence) ||
                (player._object.Is(RoleEnum.Icenberg) && CustomGameOptions.IcenbergImpVision) ||
                (player._object.Is(RoleEnum.Jester) && CustomGameOptions.JesterImpVision) ||
                (player._object.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoImpVision) ||
                (player._object.Is(RoleEnum.Vampire) && CustomGameOptions.VampImpVision))
            {
                __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                return false;
            }
            else if (player._object.Is(RoleEnum.Werewolf))
            {
                var role = Role.GetRole<Werewolf>(player._object);
                if (role.Rampaged)
                {
                    __result = __instance.MaxLightRadius;// * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                    return false;
                }
            }
            else if (player._object.Is(RoleEnum.Falcon))
            {
                var role = Role.GetRole<Falcon>(player._object);
                if(role.isZoom)
                {
                    if (!role.sabotageLightsZoom())
                    {
                        //__result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                        __result = __instance.MaxLightRadius * 5.0f;
                    }
                    else
                    {
                        if (player._object.Is(ModifierEnum.Torch)) 
                            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                        else
                            __result = 0.1f;
                        role.UnZoom();
                        //__instance.MinLightRadius
                        //__result = __instance.MinLightRadius;// __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                    }
                    
                    //__result = __instance.MaxLightRadius;
                    return false;
                }
            }

            if (Patches.SubmergedCompatibility.isSubmerged())
            {
                if (player._object.Is(ModifierEnum.Torch)) __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                return false;
            }

            var t = switchSystem != null ? switchSystem.Value / 255f : 1;

            if (player._object.Is(ModifierEnum.Torch)) t = 1;

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
            return false;
        }
    }
}