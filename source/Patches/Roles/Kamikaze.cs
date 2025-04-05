using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.KamikazeMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;

namespace TownOfUs.Roles
{
    public class Kamikaze : Role

    {
        public KillButton _plantButton;
        public float TimeRemaining;
        public bool Enabled = false;
        public bool Detonated = true;
        public Vector3 DetonatePoint;
        public Bomb Bomb = new Bomb();
        public static Material bombMaterial = TownOfUs.bundledAssets.Get<Material>("bomb");
        public DateTime StartingCooldown { get; set; }

        public Kamikaze(PlayerControl player) : base(player)
        {
            Name = "Kamikaze";
            //ImpostorText = () => !TownOfUs.PolishLanguage.Value ? "Sacrifice For Greater Purpose" : "Poswiec Sie Dla Wiekszego Celu";
            ImpostorText = () => "Sacrifice For Greater Purpose";
            //TaskText = () => !TownOfUs.PolishLanguage.Value ? "Kill crewmates and sacrifice yourself at good moment" : "Zabijaj crewmateow i poswiec sie w dobrym momencie";
            TaskText = () => "Kill crewmates and sacrifice yourself at good moment";
            Color = Palette.ImpostorRed;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Kamikaze;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
        public KillButton PlantButton
        {
            get => _plantButton;
            set
            {
                _plantButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public bool Detonating => TimeRemaining > 0f;
        public void DetonateTimer()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (MeetingHud.Instance) Detonated = true;
            if (TimeRemaining <= 0 && !Detonated)
            {
                var kami = GetRole<Kamikaze>(PlayerControl.LocalPlayer);
                kami.Bomb.ClearBomb();
                DetonateKillStart();
            }
        }
        public void DetonateKillStart()
        {
            Detonated = true;
            var playersToDie = Utils.GetClosestPlayers(DetonatePoint, CustomGameOptions.KamikazeDetonateRadius, false);
            playersToDie = Shuffle(playersToDie);
            while (playersToDie.Count > CustomGameOptions.KamikazeMaxKillsInDetonation) playersToDie.Remove(playersToDie[playersToDie.Count - 1]);
            foreach (var player in playersToDie)
            {
                if (!player.Is(RoleEnum.Pestilence) && (!player.IsShielded() || (PlayerControl.LocalPlayer.IsShielded() && PlayerControl.LocalPlayer.Is(RoleEnum.Kamikaze))) &&
                    !player.IsProtected() &&
                    player != ShowRoundOneShield.FirstRoundShielded)// || ShowRoundOneShield.FirstRoundShielded == PlayerControl.LocalPlayer.Is(RoleEnum.Kamikaze)))
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Utils.Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);
                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
        }
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> Shuffle(Il2CppSystem.Collections.Generic.List<PlayerControl> playersToDie)
        {
            var count = playersToDie.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = playersToDie[i];
                playersToDie[i] = playersToDie[r];
                playersToDie[r] = tmp;
            }
            return playersToDie;
        }
    }
}