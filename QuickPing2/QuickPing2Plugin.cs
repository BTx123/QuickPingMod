using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using QuickPing2.Patches;

namespace QuickPing2;

[BepInDependency(Jotunn.Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.NotEnforced, VersionStrictness.Minor)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class QuickPing2Plugin : BaseUnityPlugin
{

    public static QuickPing2Plugin Instance { get; set; }


    public static ManualLogSource Log { get; private set; }


    // Use this class to add your own localization to the game
    // https://valheim-modding.github.io/Jotunn/tutorials/localization.html
    //public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();

    private void Awake()
    {
        Log = Logger;
        Instance = this;
        // Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Settings.Init();
        Harmony.CreateAndPatchAll(typeof(Player_Patch), MyPluginInfo.PLUGIN_GUID);
        Harmony.CreateAndPatchAll(typeof(ChatPing_Patch), MyPluginInfo.PLUGIN_GUID);
        Harmony.CreateAndPatchAll(typeof(Minimap_Patch), MyPluginInfo.PLUGIN_GUID);
        Harmony.CreateAndPatchAll(typeof(Terminal_Patch), MyPluginInfo.PLUGIN_GUID);
        Harmony.CreateAndPatchAll(typeof(MineRock5_Patch), MyPluginInfo.PLUGIN_GUID);
        Harmony.CreateAndPatchAll(typeof(Destructible_Patch), MyPluginInfo.PLUGIN_GUID);
        Harmony.CreateAndPatchAll(typeof(ZNet_Patch), MyPluginInfo.PLUGIN_GUID);
        Harmony.CreateAndPatchAll(typeof(WearNTear_Patch), MyPluginInfo.PLUGIN_GUID);

        Player_Patch.OnPlayerPing.AddListener(Player_Patch.SendPing);
        Player_Patch.OnPlayerPing.AddListener(Minimap_Patch.AddPin);
        Player_Patch.OnPlayerForcePing.AddListener(Player_Patch.SendPing);
        Player_Patch.OnPlayerForcePing.AddListener(Minimap_Patch.ForceAddPin);
        Player_Patch.OnPlayerRename.AddListener(Player_Patch.SendRename);
        Player_Patch.OnPlayerRename.AddListener(Minimap_Patch.RenamePin);

        // To learn more about Jotunn's features, go to
        // https://valheim-modding.github.io/Jotunn/tutorials/overview.html
    }
}