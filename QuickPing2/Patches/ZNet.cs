using HarmonyLib;
using QuickPing2.Utilities;

namespace QuickPing2.Patches
{
    public class ZNet_Patch
    {
        [HarmonyPatch(typeof(ZNet), nameof(ZNet.LoadWorld))]
        [HarmonyPostfix]
        private static void LoadWorld(ZNet __instance)
        {
            DataManager.Load(ZNet.m_world, Game.instance.GetPlayerProfile());
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.SaveWorldThread))]
        [HarmonyPostfix]
        private static void SaveWorldThread()
        {
            bool cloudSaveFailed = DataManager.Save(ZNet.m_world, Game.instance.GetPlayerProfile());

            QuickPing2Plugin.Log.LogInfo($"cloud save : {!cloudSaveFailed}");
        }
    }
}