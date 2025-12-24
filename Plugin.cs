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
    /// 로컬 플레이어의 SteamID를 가져옵니다.
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

    // PunManager 업그레이드 패치들
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
    /// 다른 플레이어가 업그레이드를 획득했을 때, 확률에 따라 자기 자신에게도 적용합니다.
    /// 모든 플레이어가 모드를 설치해야 작동합니다.
    /// </summary>
    private static void ApplySharedUpgradeToSelf(string upgradeType, string sourceSteamID, int amount, Action<int> applyToSelf)
    {
        try
        {
            // 재귀 호출 방지
            if (_isApplyingSharedUpgrade) return;
            
            // 모드 비활성화 체크
            if (!UpgradeConfiguration.ModEnabled.Value) return;

            // 내 SteamID 가져오기
            string mySteamID = GetMySteamID();
            if (string.IsNullOrEmpty(mySteamID)) return;

            // 내가 먹은 아이템이면 무시 (이미 적용됨)
            if (mySteamID == sourceSteamID) return;

            Logger.LogInfo($"[LuckyUpgrades] ★ 다른 플레이어 업그레이드 감지! {upgradeType}: {sourceSteamID} +{amount}");

            // 업그레이드 소멸 체크
            if (UpgradeConfiguration.ShouldWasteUpgrade())
            {
                Logger.LogInfo("[LuckyUpgrades] 업그레이드 소멸!");
                return;
            }

            // 공유 확률 가져오기
            int shareChance = UpgradeConfiguration.GetShareChance(upgradeType);
            int roll = _random.Next(100);

            Logger.LogInfo($"[LuckyUpgrades] 확률 체크: {roll} < {shareChance}?");

            if (roll < shareChance)
            {
                try
                {
                    _isApplyingSharedUpgrade = true;
                    applyToSelf(amount);
                    Logger.LogInfo($"[LuckyUpgrades] ★★ 나에게 {upgradeType} +{amount} 공유 적용됨!");
                }
                finally
                {
                    _isApplyingSharedUpgrade = false;
                }
            }
            else
            {
                Logger.LogInfo($"[LuckyUpgrades] 확률 실패 - 공유 안 됨");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"[LuckyUpgrades] ApplySharedUpgradeToSelf 예외: {ex.Message}");
        }
    }
}
