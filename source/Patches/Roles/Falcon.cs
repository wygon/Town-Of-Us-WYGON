using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace TownOfUs.Roles
{
    public class Falcon : Role
    {
        public DateTime LastZoom;
        public bool Enabled;
        public float TimeRemaining;
        public int UsesLeft;
        public TextMeshPro UsesText;
        public float Range;
        public Falcon(PlayerControl player) : base(player)
        {
            Name = "Falcon";
            ImpostorText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "See more!" : "Zobacz wiecej!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Use your friend to get more information" : "Uzyj przyjaciela aby zobaczyc wiecej";
            Color = Patches.Colors.Falcon;
            RoleType = RoleEnum.Falcon;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.WingManMaxUses;
            Range = CustomGameOptions.WingManRange;
        }
        public bool isZoom => TimeRemaining > 0f;
        public bool ButtonUsable => UsesLeft != 0 && !sabotageLightsZoom();
        public float ZoomTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastZoom;
            var cooldown = CustomGameOptions.WingManCd * 1000f;
            var flag2 = cooldown - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (cooldown - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void Zoom()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            //if (sabotageLightsZoom())
            Zooming(Enabled);
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
                //Debug.Log($"[WingMan] deactivated for player: {Player.name}");
            }
            //Debug.Log($"[WingMan] activated for player: {Player.name} uses left {UsesLeft}");
        }
        public void UnZoom()
        {
            Enabled = false;
            LastZoom = DateTime.UtcNow;
            Zooming(Enabled);
            //Debug.Log($"[WingMan] Player {Player.name} unzoomed.");
        }
        private void Zooming(bool enabled)
        {
            var size = enabled ? Range : 3f;// 6f = Range
            Camera.main.orthographicSize = size;
            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }
            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
        }
        public bool sabotageLightsZoom()
        {
            var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
            {
                case 0:
                case 3:
                case 1:
                case 2:
                case 4:
                case 6:
                case 7:
                    if (lights.IsActive)
                    {      
                        //Debug.Log("[MAP ID]" + GameOptionsManager.Instance.currentNormalGameOptions.MapId);
                        return true;
                    }
                    //break;
                    return false;
                default:
                    return false;
            }

            //return false;
        }
        //public void EnableVisionThroughWalls(bool enable)
        //{
        //    var role = Role.GetRole<Zoomer>(PlayerControl.LocalPlayer);
        //    if (role != null)
        //    {
        //        if (enable)
        //        {
        //            role.MyLight.radius = 50f; // Maksymalna widoczność
        //            role.MyLight.mask = -1;    // Usunięcie ograniczeń oświetlenia
        //        }
        //        else
        //        {
        //            role.MyLight.radius = 5f;  // Przywrócenie normalnego pola widzenia
        //        }
        //    }
        //}
        //[HarmonyPatch(typeof(PlayerControl), "CanSeePlayer")]
        //public class VisionThroughWallsPatch
        //{
        //    public static void Postfix(ref bool __result, PlayerControl __instance)
        //    {
        //        var role = Role.GetRole<Zoomer>(PlayerControl.LocalPlayer);
        //        if (__instance == PlayerControl.LocalPlayer && role.isZoom)
        //        {
        //            __result = true;
        //        }
        //        else
        //            __result = false;
        //    }
        //}
    }
}