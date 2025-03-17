using HarmonyLib;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Modifiers
{
    public class DrunkPatch
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysics_FixedUpdate
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                if (__instance.myPlayer.Is(ModifierEnum.Drunk))
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                        if (!(Modifier.GetModifier<Drunk>(__instance.myPlayer).RoundsLeft <= 0))
                            __instance.body.velocity *= -1;             
            }
        }
    }
}