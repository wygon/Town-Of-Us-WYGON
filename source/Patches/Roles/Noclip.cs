using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using TownOfUs.Patches;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Noclip : Role
    {
        private KillButton _noclipButton;
        public DateTime LastNoclip;
        public bool Enabled;
        public Vector3 NoclipSafePoint = new();
        public float TimeRemaining;

        public Noclip(PlayerControl player) : base(player)
        {
            Name = "Noclip";
            ImpostorText = () => "Walk Through Walls and Kill Your Enemies";
            TaskText = () => "Use your power to surprise your enemies";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.Noclip;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;

        }

        public KillButton NoclipButton
        {
            get => _noclipButton;
            set
            {
                _noclipButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public bool Noclipped => TimeRemaining > 0f;
        public float NoclipTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastNoclip;
            var cooldown = CustomGameOptions.NoclipCd * 1000f;
            var flag2 = cooldown - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (cooldown - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void WallWalk()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Player.Collider.enabled = false;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
                Debug.Log($"[UnWallWalk] Noclip deactivated for player: {Player.name}");
            }
        }
        public void UnWallWalk()
        {
            Enabled = false;
            LastNoclip = DateTime.UtcNow;
            Player.Collider.enabled = true;
            Debug.Log($"[UnWallWalk] Player {Player.name} unmorphed.");
        }
    }
}