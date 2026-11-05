using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

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
    internal static string _mySteamID = null;

    // Guest-only bonuses to merge into authoritative host snapshots
    internal static readonly Dictionary<string, int> _sharedUpgrades = new Dictionary<string, int>();

    private Harmony harmony;

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        UpgradeConfiguration = new UpgradeConfig(Config);

        harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(typeof(Plugin));

        // Create a separate GameObject for the Update loop
        var updateRunner = new GameObject("LuckyUpgrades_UpdateRunner");
        updateRunner.AddComponent<UpgradeReapplyRunner>();
        UnityEngine.Object.DontDestroyOnLoad(updateRunner);
        updateRunner.hideFlags = HideFlags.HideAndDontSave;

        Logger.LogInfo("Harmony patches applied!");
    }

    /// <summary>
    /// Gets the local player's SteamID.
    /// </summary>
    internal static string GetMySteamID()
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

    // Merge guest-only bonuses into host snapshots, once per received dictionary.
    [HarmonyPatch(typeof(PunManager), "ReceiveSyncData")]
    [HarmonyPrefix]
    public static void ReceiveSyncData_Prefix(ref ExitGames.Client.Photon.Hashtable __0,
        PhotonMessageInfo __2, out bool __state)
    {
        __state = false;
#pragma warning disable Harmony003 // PhotonMessageInfo.Sender is a struct getter, not an assignment.
        var sender = __2.Sender;
#pragma warning restore Harmony003
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient ||
            sender != PhotonNetwork.MasterClient || _sharedUpgrades.Count == 0 || __0 == null) return;

        LogSyncedStats(__0, "before");
        var data = new ExitGames.Client.Photon.Hashtable();
        foreach (var entry in __0) data[entry.Key] = entry.Value;
        SharedUpgradeSync.Merge(data, GetMySteamID(), _sharedUpgrades, message => Logger.LogInfo(message));
        __0 = data;
        __state = true;
    }

    [HarmonyPatch(typeof(PunManager), "ReceiveSyncData")]
    [HarmonyPostfix]
    public static void ReceiveSyncData_Postfix(ExitGames.Client.Photon.Hashtable __0, bool __state)
    {
        if (__state) LogSyncedStats(__0, "after");
    }

    private static void LogSyncedStats(ExitGames.Client.Photon.Hashtable data, string phase)
    {
        string myID = GetMySteamID();
        if (string.IsNullOrEmpty(myID) || StatsManager.instance == null) return;
        foreach (var upgrade in _sharedUpgrades)
        {
            if (!SharedUpgradeSync.StatNames.TryGetValue(upgrade.Key, out string statName) ||
                !data.ContainsKey(statName)) continue;
            if (StatsManager.instance.dictionaryOfDictionaries.TryGetValue(statName, out var stats))
            {
                int value = stats.TryGetValue(myID, out int current) ? current : 0;
                Logger.LogInfo($"[LuckyUpgrades] Sync {phase} {upgrade.Key}: actual={value}");
            }
        }
    }

    /// <summary>
    /// Tracks a shared upgrade for later reapplication.
    /// </summary>
    private static void TrackSharedUpgrade(string upgradeType, int amount)
    {
        if (_sharedUpgrades.ContainsKey(upgradeType))
        {
            _sharedUpgrades[upgradeType] += amount;
        }
        else
        {
            _sharedUpgrades[upgradeType] = amount;
        }
        Logger.LogInfo($"[LuckyUpgrades] Tracked: {upgradeType} (total: {_sharedUpgrades[upgradeType]})");
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
    /// </summary>
    private static void ApplySharedUpgradeToSelf(string upgradeType, string sourceSteamID, int amount, Action<int> applyToSelf)
    {
        try
        {
            if (_isApplyingSharedUpgrade || amount <= 0) return;

            string mySteamID = GetMySteamID();
            if (string.IsNullOrEmpty(mySteamID)) return;
            if (mySteamID == sourceSteamID) return;

            int shareChance = UpgradeConfiguration.GetShareChance(upgradeType);
            int roll = _random.Next(100);

            if (roll < shareChance)
            {
                try
                {
                    _isApplyingSharedUpgrade = true;
                    string statName = SharedUpgradeSync.StatNames[upgradeType];
                    var stats = StatsManager.instance.dictionaryOfDictionaries[statName];
                    int before = stats.TryGetValue(mySteamID, out int previous) ? previous : 0;
                    applyToSelf(amount);
                    int after = stats.TryGetValue(mySteamID, out int current) ? current : 0;
                    Logger.LogInfo($"[LuckyUpgrades] Share {upgradeType}: before={before}, after={after}");
                    int applied = after - before;
                    if (applied <= 0) return;
                    TrackSharedUpgrade(upgradeType, applied);
                    Logger.LogInfo($"[LuckyUpgrades] Shared upgrade applied: {upgradeType} +{applied} (rolled: {roll} | chance: {shareChance}%) ✓");
                }
                finally
                {
                    _isApplyingSharedUpgrade = false;
                }
            }
            else
            {
                // 확률 실패 시 로그
                Logger.LogInfo($"[LuckyUpgrades] Shared upgrade missed: {upgradeType} (rolled: {roll} | chance: {shareChance}%) ✗");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"[LuckyUpgrades] Error in ApplySharedUpgradeToSelf: {ex.Message}");
        }
    }
}

/// <summary>
/// Detect session boundaries and clear guest-only bonuses.
/// </summary>
public class UpgradeReapplyRunner : MonoBehaviour
{
    // 세션 종료로 판정할 레벨 이름 목록 (정확한 이름만 사용, Contains 미사용)
    private static readonly HashSet<string> SESSION_END_LEVELS = new HashSet<string>
    {
        "Level - Main Menu",
        "Level - Lobby Menu"
    };

    private string _lastLevelName = "";
    private void Update()
    {
        // Get current level name from RunManager
        string currentLevel = "";
        try
        {
            if (RunManager.instance != null && RunManager.instance.levelCurrent != null)
            {
                currentLevel = RunManager.instance.levelCurrent.name;
            }
        }
        catch
        {
            // Ignore errors when RunManager is not available
        }

        // 레벨 변경 감지
        if (!string.IsNullOrEmpty(currentLevel) && currentLevel != _lastLevelName)
        {
            Plugin.Logger.LogInfo($"[LuckyUpgrades] Level changed: {_lastLevelName} -> {currentLevel}");
            _lastLevelName = currentLevel;

            // 세션 종료 레벨(메인메뉴/로비)로 이동 시 모든 추적 데이터 초기화
            if (SESSION_END_LEVELS.Contains(currentLevel))
            {
                Plugin._sharedUpgrades.Clear();
                Plugin._mySteamID = null;
                Plugin.Logger.LogInfo("[LuckyUpgrades] Session ended. All tracked data cleared.");
                return;
            }
        }
    }
}
