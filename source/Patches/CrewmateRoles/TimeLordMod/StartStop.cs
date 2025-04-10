
using System;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.TimeLordMod
{
    public class StartStop
    {
        public static Color oldColor;

        public static void StartRewind(TimeLord role)
        {
            //System.Console.WriteLine("START...");
            RecordRewind.rewinding = true;
            RecordRewind.whoIsRewinding = role;
            PlayerControl.LocalPlayer.moveable = false;
            oldColor = HudManager.Instance.FullScreen.color;
            HudManager.Instance.FullScreen.gameObject.active = true;
            HudManager.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            HudManager.Instance.FullScreen.enabled = true;
            role.StartRewind = DateTime.UtcNow;
        }

        public static void StopRewind(TimeLord role)
        {
            //System.Console.WriteLine("STOP...");
            role.FinishRewind = DateTime.UtcNow;
            RecordRewind.rewinding = false;
            PlayerControl.LocalPlayer.moveable = true;
            Patches.SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                if (fullscreen.color.Equals(new Color(0f, 0.5f, 0.8f, 0.3f)))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                    fullscreen.enabled = false;
                }
            }
            HudManager.Instance.FullScreen.enabled = false;
            HudManager.Instance.FullScreen.color = oldColor;
        }
    }
}
