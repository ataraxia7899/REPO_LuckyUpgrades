using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;

namespace LuckyUpgrades;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("REPO.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public static Plugin Instance { get; private set; }
    public static UpgradeConfig UpgradeConfiguration { get; private set; }

    private static bool _isApplyingSharedUpgrade = false;
    private static readonly System.Random _random = new System.Random();
    private static string _mySteamID = null;

    private Harmony harmony;

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        UpgradeConfiguration = new UpgradeConfig(Config);

        harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(typeof(Plugin));

        Logger.LogInfo("Harmony patches applied!");
    }

    /// <summary>
    /// Gets the local player's SteamID.
    /// </summary>
    private static string GetMySteamID()
    {
        if (string.IsNullOrEmpty(_mySteamID))
        {
            var localPlayer = SemiFunc.PlayerAvatarLocal();
            if (localPlayer != null)
            {
                _mySteamID = SemiFunc.PlayerGetSteamID(localPlayer);
            }
        }
        return _mySteamID;
    }

    // PunManager upgrade patches
    [HarmonyPatch(typeof(PunManager), "UpgradePlayerHealth")]
    [HarmonyPostfix]
    public static void UpgradePlayerHealth_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("Health", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerHealth(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerEnergy")]
    [HarmonyPostfix]
    public static void UpgradePlayerEnergy_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("Energy", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerEnergy(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerExtraJump")]
    [HarmonyPostfix]
    public static void UpgradePlayerExtraJump_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("ExtraJump", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerExtraJump(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerGrabRange")]
    [HarmonyPostfix]
    public static void UpgradePlayerGrabRange_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("GrabRange", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerGrabRange(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerGrabStrength")]
    [HarmonyPostfix]
    public static void UpgradePlayerGrabStrength_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("GrabStrength", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerGrabStrength(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerThrowStrength")]
    [HarmonyPostfix]
    public static void UpgradePlayerThrowStrength_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("GrabThrow", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerThrowStrength(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerSprintSpeed")]
    [HarmonyPostfix]
    public static void UpgradePlayerSprintSpeed_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("SprintSpeed", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerSprintSpeed(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerTumbleLaunch")]
    [HarmonyPostfix]
    public static void UpgradePlayerTumbleLaunch_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("TumbleLaunch", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerTumbleLaunch(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradeMapPlayerCount")]
    [HarmonyPostfix]
    public static void UpgradeMapPlayerCount_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("MapPlayerCount", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradeMapPlayerCount(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerTumbleClimb")]
    [HarmonyPostfix]
    public static void UpgradePlayerTumbleClimb_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("TumbleClimb", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerTumbleClimb(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerTumbleWings")]
    [HarmonyPostfix]
    public static void UpgradePlayerTumbleWings_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("TumbleWings", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerTumbleWings(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradePlayerCrouchRest")]
    [HarmonyPostfix]
    public static void UpgradePlayerCrouchRest_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("CrouchRest", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradePlayerCrouchRest(myID, amount);
        });
    }

    [HarmonyPatch(typeof(PunManager), "UpgradeDeathHeadBattery")]
    [HarmonyPostfix]
    public static void UpgradeDeathHeadBattery_Postfix(string _steamID, int value)
    {
        ApplySharedUpgradeToSelf("DeathHeadBattery", _steamID, value, (amount) =>
        {
            string myID = GetMySteamID();
            if (!string.IsNullOrEmpty(myID))
                PunManager.instance.UpgradeDeathHeadBattery(myID, amount);
        });
    }

    /// <summary>
    /// When another player gets an upgrade, apply it to self based on probability.
    /// Requires all players to have the mod installed.
    /// </summary>
    private static void ApplySharedUpgradeToSelf(string upgradeType, string sourceSteamID, int amount, Action<int> applyToSelf)
    {
        try
        {
            // Prevent recursive calls
            if (_isApplyingSharedUpgrade) return;

            // Get my SteamID
            string mySteamID = GetMySteamID();
            if (string.IsNullOrEmpty(mySteamID)) return;

            // Ignore if I picked up the item (already applied)
            if (mySteamID == sourceSteamID) return;

            // Get share chance and roll
            int shareChance = UpgradeConfiguration.GetShareChance(upgradeType);
            int roll = _random.Next(100);

            if (roll < shareChance)
            {
                try
                {
                    _isApplyingSharedUpgrade = true;
                    applyToSelf(amount);
                    Logger.LogInfo($"[LuckyUpgrades] Shared upgrade applied: {upgradeType} +{amount} (roll: {roll} < {shareChance})");
                }
                finally
                {
                    _isApplyingSharedUpgrade = false;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"[LuckyUpgrades] Error in ApplySharedUpgradeToSelf: {ex.Message}");
        }
    }
}
