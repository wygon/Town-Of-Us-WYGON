using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities.Extensions;
using Reactor.Networking.Attributes;
using TownOfUs.CustomOption;
using TownOfUs.Patches;
using TownOfUs.RainbowMod;
using TownOfUs.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using TownOfUs.Patches.ScreenEffects;
using TownOfUs.CrewmateRoles.DetectiveMod;
using TownOfUs.NeutralRoles.SoulCollectorMod;
using System.IO;
using Reactor.Utilities;
using AmongUs.GameOptions;
using System.Linq;

namespace TownOfUs
{
    [BepInPlugin(Id, "Wygon's Town", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
    public class TownOfUs : BasePlugin
    {
        public const string Id = "com.slushiegoose.townofus";
        public const string VersionString = "2.1.2";
        public static System.Version Version = System.Version.Parse(VersionString);
        public const string VersionTag = "";

        public const int MaxPlayers = 35;
        public const int MaxImpostors = 35 / 2;
        public static string WygonTextColor = "0045CD";

        public static AssetLoader bundledAssets;

        public static Sprite JanitorClean;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Footprint;
        public static Sprite MedicSprite;
        public static Sprite SeerSprite;
        public static Sprite SampleSprite;
        public static Sprite MorphSprite;
        public static Sprite Arrow;
        public static Sprite MineSprite;
        public static Sprite SwoopSprite;
        public static Sprite DouseSprite;
        public static Sprite IgniteSprite;
        public static Sprite ReviveSprite;
        public static Sprite ButtonSprite;
        public static Sprite DisperseSprite;
        public static Sprite CycleBackSprite;
        public static Sprite CycleForwardSprite;
        public static Sprite GuessSprite;
        public static Sprite DragSprite;
        public static Sprite DropSprite;
        public static Sprite FlashSprite;
        public static Sprite AlertSprite;
        public static Sprite RememberSprite;
        public static Sprite TrackSprite;
        public static Sprite PlantSprite;
        public static Sprite DetonateSprite;
        public static Sprite TransportSprite;
        public static Sprite MediateSprite;
        public static Sprite VestSprite;
        public static Sprite ProtectSprite;
        public static Sprite BlackmailSprite;
        public static Sprite BlackmailLetterSprite;
        public static Sprite BlackmailOverlaySprite;
        public static Sprite LighterSprite;
        public static Sprite DarkerSprite;
        public static Sprite InfectSprite;
        public static Sprite RampageSprite;
        public static Sprite TrapSprite;
        public static Sprite InspectSprite;
        public static Sprite ExamineSprite;
        public static Sprite EscapeSprite;
        public static Sprite MarkSprite;
        public static Sprite NoclipSprite;
        public static Sprite ImitateSelectSprite;
        public static Sprite ImitateDeselectSprite;
        public static Sprite ObserveSprite;
        public static Sprite BiteSprite;
        public static Sprite RevealSprite;
        public static Sprite ConfessSprite;
        public static Sprite NoAbilitySprite;
        public static Sprite CamouflageSprite;
        public static Sprite CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite;
        public static Sprite HackSprite;
        public static Sprite MimicSprite;
        public static Sprite LockSprite;
        public static Sprite StalkSprite;
        public static Sprite CrimeSceneSprite;
        public static Sprite CampaignSprite;
        public static Sprite FortifySprite;
        public static Sprite HypnotiseSprite;
        public static Sprite HysteriaSprite;
        public static Sprite JailSprite;
        public static Sprite InJailSprite;
        public static Sprite ExecuteSprite;
        public static Sprite CollectSprite;
        public static Sprite ReapSprite;
        public static Sprite SoulSprite;
        public static Sprite WatchSprite;
        public static Sprite CampSprite;
        public static Sprite ShootSprite;
        public static Sprite WingManSprite;
        public static Sprite RewindSprite;
        public static Sprite FreezeSprite;
        public static Sprite BlizzardSprite;
        public static Sprite SuicideSprite;
        public static Sprite EatSprite;

        public static Sprite ToUBanner;
        public static Sprite UpdateTOUButton;
        public static Sprite UpdateSubmergedButton;

        public static Sprite ZoomPlusButton;
        public static Sprite ZoomMinusButton;
        public static Sprite ZoomPlusActiveButton;
        public static Sprite ZoomMinusActiveButton;

        public static Vector3 ButtonPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);

        private static DLoadImage _iCallLoadImage;

        private Harmony _harmony;

        public static ConfigEntry<bool> DeadSeeGhosts { get; set; }

        public static string RuntimeLocation;

        public override void Load()
        {
            RuntimeLocation = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TownOfUs)).Location);
            ReactorCredits.Register<TownOfUs>(ReactorCredits.AlwaysShow);
            System.Console.WriteLine("000.000.000.000/000000000000000000");

            _harmony = new Harmony("com.slushiegoose.townofus");

            NormalGameOptionsV08.RecommendedImpostors = NormalGameOptionsV08.MaxImpostors = Enumerable.Repeat(35, 35).ToArray();
            NormalGameOptionsV08.MinPlayers = Enumerable.Repeat(4, 35).ToArray();
            HideNSeekGameOptionsV08.MinPlayers = Enumerable.Repeat(4, 35).ToArray();

            Generate.GenerateAll();

            bundledAssets = new();

            JanitorClean = CreateSprite("TownOfUs.Resources.Janitor.png");
            EngineerFix = CreateSprite("TownOfUs.Resources.Engineer.png");
            SwapperSwitch = CreateSprite("TownOfUs.Resources.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite("TownOfUs.Resources.SwapperSwitchDisabled.png");
            Footprint = CreateSprite("TownOfUs.Resources.Footprint.png");
            MedicSprite = CreateSprite("TownOfUs.Resources.Medic.png");
            SeerSprite = CreateSprite("TownOfUs.Resources.Seer.png");
            SampleSprite = CreateSprite("TownOfUs.Resources.Sample.png");
            MorphSprite = CreateSprite("TownOfUs.Resources.Morph.png");
            Arrow = CreateSprite("TownOfUs.Resources.Arrow.png");
            MineSprite = CreateSprite("TownOfUs.Resources.Mine.png");
            SwoopSprite = CreateSprite("TownOfUs.Resources.Swoop.png");
            DouseSprite = CreateSprite("TownOfUs.Resources.Douse.png");
            IgniteSprite = CreateSprite("TownOfUs.Resources.Ignite.png");
            ReviveSprite = CreateSprite("TownOfUs.Resources.Revive.png");
            ButtonSprite = CreateSprite("TownOfUs.Resources.Button.png");
            DisperseSprite = CreateSprite("TownOfUs.Resources.Disperse.png");
            DragSprite = CreateSprite("TownOfUs.Resources.Drag.png");
            DropSprite = CreateSprite("TownOfUs.Resources.Drop.png");
            CycleBackSprite = CreateSprite("TownOfUs.Resources.CycleBack.png");
            CycleForwardSprite = CreateSprite("TownOfUs.Resources.CycleForward.png");
            GuessSprite = CreateSprite("TownOfUs.Resources.Guess.png");
            FlashSprite = CreateSprite("TownOfUs.Resources.Flash.png");
            AlertSprite = CreateSprite("TownOfUs.Resources.Alert.png");
            RememberSprite = CreateSprite("TownOfUs.Resources.Remember.png");
            TrackSprite = CreateSprite("TownOfUs.Resources.Track.png");
            PlantSprite = CreateSprite("TownOfUs.Resources.Plant.png");
            DetonateSprite = CreateSprite("TownOfUs.Resources.Detonate.png");
            TransportSprite = CreateSprite("TownOfUs.Resources.Transport.png");
            MediateSprite = CreateSprite("TownOfUs.Resources.Mediate.png");
            VestSprite = CreateSprite("TownOfUs.Resources.Vest.png");
            ProtectSprite = CreateSprite("TownOfUs.Resources.Protect.png");
            BlackmailSprite = CreateSprite("TownOfUs.Resources.Blackmail.png");
            BlackmailLetterSprite = CreateSprite("TownOfUs.Resources.BlackmailLetter.png");
            BlackmailOverlaySprite = CreateSprite("TownOfUs.Resources.BlackmailOverlay.png");
            LighterSprite = CreateSprite("TownOfUs.Resources.Lighter.png");
            DarkerSprite = CreateSprite("TownOfUs.Resources.Darker.png");
            InfectSprite = CreateSprite("TownOfUs.Resources.Infect.png");
            RampageSprite = CreateSprite("TownOfUs.Resources.Rampage.png");
            TrapSprite = CreateSprite("TownOfUs.Resources.Trap.png");
            InspectSprite = CreateSprite("TownOfUs.Resources.Inspect.png");
            ExamineSprite = CreateSprite("TownOfUs.Resources.Examine.png");
            EscapeSprite = CreateSprite("TownOfUs.Resources.Recall.png");
            MarkSprite = CreateSprite("TownOfUs.Resources.Mark.png");
            NoclipSprite = CreateSprite("TownOfUs.Resources.Noclip.png");
            ImitateSelectSprite = CreateSprite("TownOfUs.Resources.ImitateSelect.png");
            ImitateDeselectSprite = CreateSprite("TownOfUs.Resources.ImitateDeselect.png");
            ObserveSprite = CreateSprite("TownOfUs.Resources.Observe.png");
            BiteSprite = CreateSprite("TownOfUs.Resources.Bite.png");
            RevealSprite = CreateSprite("TownOfUs.Resources.Reveal.png");
            ConfessSprite = CreateSprite("TownOfUs.Resources.Confess.png");
            NoAbilitySprite = CreateSprite("TownOfUs.Resources.NoAbility.png");
            CamouflageSprite = CreateSprite("TownOfUs.Resources.Camouflage.png");
            CamoSprintSprite = CreateSprite("TownOfUs.Resources.CamoSprint.png");
            CamoSprintFreezeSprite = CreateSprite("TownOfUs.Resources.CamoSprintFreeze.png");
            HackSprite = CreateSprite("TownOfUs.Resources.Hack.png");
            MimicSprite = CreateSprite("TownOfUs.Resources.Mimic.png");
            LockSprite = CreateSprite("TownOfUs.Resources.Lock.png");
            StalkSprite = CreateSprite("TownOfUs.Resources.Stalk.png");
            CrimeSceneSprite = CreateSprite("TownOfUs.Resources.CrimeScene.png");
            CampaignSprite = CreateSprite("TownOfUs.Resources.Campaign.png");
            FortifySprite = CreateSprite("TownOfUs.Resources.Fortify.png");
            HypnotiseSprite = CreateSprite("TownOfUs.Resources.Hypnotise.png");
            HysteriaSprite = CreateSprite("TownOfUs.Resources.Hysteria.png");
            JailSprite = CreateSprite("TownOfUs.Resources.Jail.png");
            InJailSprite = CreateSprite("TownOfUs.Resources.InJail.png");
            ExecuteSprite = CreateSprite("TownOfUs.Resources.Execute.png");
            CollectSprite = CreateSprite("TownOfUs.Resources.Collect.png");
            ReapSprite = CreateSprite("TownOfUs.Resources.Reap.png");
            SoulSprite = CreateSprite("TownOfUs.Resources.Soul.png");
            WatchSprite = CreateSprite("TownOfUs.Resources.Watch.png");
            CampSprite = CreateSprite("TownOfUs.Resources.Camp.png");
            ShootSprite = CreateSprite("TownOfUs.Resources.Shoot.png");
            WingManSprite = CreateSprite("TownOfUs.Resources.Wingman.png");
            RewindSprite = CreateSprite("TownOfUs.Resources.Rewind.png");
            FreezeSprite = CreateSprite("TownOfUs.Resources.Freeze.png");
            BlizzardSprite = CreateSprite("TownOfUs.Resources.Blizzard.png");
            SuicideSprite = CreateSprite("TownOfUs.Resources.Suicide.png");
            EatSprite = CreateSprite("TownOfUs.Resources.Eat.png");

            ToUBanner = CreateSprite("TownOfUs.Resources.TownOfUsBanner.png");
            UpdateTOUButton = CreateSprite("TownOfUs.Resources.UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite("TownOfUs.Resources.UpdateSubmergedButton.png");

            ZoomPlusButton = CreateSprite("TownOfUs.Resources.Plus.png");
            ZoomMinusButton = CreateSprite("TownOfUs.Resources.Minus.png");
            ZoomPlusActiveButton = CreateSprite("TownOfUs.Resources.PlusActive.png");
            ZoomMinusActiveButton = CreateSprite("TownOfUs.Resources.MinusActive.png");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<CrimeScene>();
            ClassInjector.RegisterTypeInIl2Cpp<Soul>();

            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    var filePath = Application.persistentDataPath;
                    var file = filePath + $"/GameSettings-Slot{i}";
                    if (File.Exists(file))
                    {
                        string newFile = Path.Combine(filePath, $"Saved Settings {i}.txt");
                        File.Move(file, newFile);
                    }
                }
                catch { }
            }

            // RegisterInIl2CppAttribute.Register();

            DeadSeeGhosts = Config.Bind("Settings", "Dead See Other Ghosts", true, "Whether you see other dead player's ghosts while your dead");

            _harmony.PatchAll();
            SubmergedCompatibility.Initialize();

            ServerManager.DefaultRegions = new Il2CppReferenceArray<IRegionInfo>(new IRegionInfo[0]);
        }

        public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);

            var assembly = Assembly.GetExecutingAssembly();
            var tex = AmongUsExtensions.CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }
}
