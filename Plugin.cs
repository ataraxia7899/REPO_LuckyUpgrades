using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LuckyUpgrades.Patches;

namespace LuckyUpgrades
{
    /// <summary>
    /// LuckyUpgrades 모드의 메인 플러그인 클래스입니다.
    /// 업그레이드 아이템 획득 시 설정된 확률에 따라 모든 플레이어에게 공유하는 기능을 수행합니다.
    /// </summary>
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        /// <summary>
        /// 플러그인의 싱글톤 인스턴스입니다.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <summary>
        /// BepInEx 로거 인스턴스입니다.
        /// </summary>
        internal static ManualLogSource Log { get; private set; }

        /// <summary>
        /// Harmony 패치 관리를 위한 인스턴스입니다.
        /// </summary>
        private Harmony _harmony;

        /// <summary>
        /// 공유 확률 설정을 관리하는 인스턴스입니다.
        /// </summary>
        public static UpgradeConfig UpgradeConfiguration { get; private set; }

        /// <summary>
        /// 플러그인이 로드될 때 호출되어 초기화 작업을 수행합니다.
        /// </summary>
        private void Awake()
        {
            Instance = this;
            Log = Logger;

            Log.LogInfo($"[{PluginInfo.PLUGIN_NAME}] Plugin loading...");

            // 설정 시스템 초기화
            UpgradeConfiguration = new UpgradeConfig(Config);
            Log.LogInfo($"[{PluginInfo.PLUGIN_NAME}] Configuration loaded");

            // Harmony 인스턴스 생성 및 패치 적용
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            
            // 수동 패치 로직 실행 (다양한 업그레이드 클래스 대응)
            UpgradePatcher.PatchAll(_harmony);

            Log.LogInfo($"[{PluginInfo.PLUGIN_NAME}] v{PluginInfo.PLUGIN_VERSION} loaded successfully!");
        }

        /// <summary>
        /// 플러그인이 비활성화되거나 제거될 때 리소스를 해제합니다.
        /// </summary>
        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }

    /// <summary>
    /// 플러그인 식별 정보를 정의하는 정적 클래스입니다.
    /// </summary>
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.reposharemod.luckyupgrades";
        public const string PLUGIN_NAME = "LuckyUpgrades";
        public const string PLUGIN_VERSION = "1.0.4";
    }
}
