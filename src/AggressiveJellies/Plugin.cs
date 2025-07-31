using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
//using UnityEngine;


namespace AggressiveJellies
{
    [BepInPlugin(modGUID, "Aggressive Jellies", "0.1.0")]
    public sealed class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "com.toastbubbles.aggressivejellies";
        private static readonly Harmony harmony = new Harmony(modGUID);
        public static ManualLogSource Log = new ManualLogSource("Aggressive Jellies");
        public static Plugin Instance;

        private void Awake()
        {

            if(Instance == null)
            {
                Instance = this;
            }
            Log = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Logger.LogInfo("Aggressive Jellies is Loading!");
            harmony.PatchAll();
            Logger.LogInfo("Loaded!");
        }
    }
}