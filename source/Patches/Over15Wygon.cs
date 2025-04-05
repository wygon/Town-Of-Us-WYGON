//using AmongUs.GameOptions;
//using BepInEx;
//using BepInEx.Unity.IL2CPP;
//using HarmonyLib;
//using Reactor;
//using Reactor.Networking;
//using Reactor.Networking.Attributes;
//using System.Linq;

//namespace Over15Wygon;

//[BepInAutoPlugin("pl.townofus.over15wygon")]
//[BepInProcess("Among Us.exe")]
//[BepInDependency(ReactorPlugin.Id)]
//[BepInDependency("gg.reactor.debugger", BepInDependency.DependencyFlags.SoftDependency)] // fix debugger overwriting MinPlayers
//public partial class Over15WygonPlugin : BasePlugin
//{
//    public const int MaxPlayers = 35;
//    public const int MaxImpostors = 35 / 2;

//    private Harmony Harmony { get; } = new(Id);

//    public override void Load()
//    {
//        NormalGameOptionsV08.RecommendedImpostors = NormalGameOptionsV08.MaxImpostors = Enumerable.Repeat(35, 35).ToArray();
//        NormalGameOptionsV08.MinPlayers = Enumerable.Repeat(4, 35).ToArray();
//        HideNSeekGameOptionsV08.MinPlayers = Enumerable.Repeat(4, 35).ToArray();

//        Harmony.PatchAll();
//    }
//}