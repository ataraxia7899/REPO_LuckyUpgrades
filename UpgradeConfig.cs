using BepInEx.Configuration;

namespace LuckyUpgrades
{
    /// <summary>
    /// 업그레이드 공유 확률을 관리하는 설정 클래스입니다.
    /// 각 업그레이드 종류별로 다른 플레이어에게 적용될 확률을 정의합니다.
    /// </summary>
    public class UpgradeConfig
    {
        // === 전역 설정 (Global Settings) ===
        
        /// <summary>
        /// 모드 활성화 여부입니다.
        /// </summary>
        public ConfigEntry<bool> ModEnabled { get; private set; }

        /// <summary>
        /// 호스트 전용 모드 실행 여부입니다.
        /// </summary>
        public ConfigEntry<bool> HostOnly { get; private set; }

        /// <summary>
        /// 모든 업그레이드에 단일 확률을 적용할지 여부입니다.
        /// </summary>
        public ConfigEntry<bool> UseOneChanceForAll { get; private set; }

        /// <summary>
        /// 모든 플레이어에게 적용될 전역 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> GlobalChanceToActivate { get; private set; }

        /// <summary>
        /// 업그레이드가 누구에게도 적용되지 않고 소멸될 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToWasteUpgrade { get; private set; }

        // === 개별 업그레이드 확률 (Specific Upgrade Chances) ===

        /// <summary>
        /// 체력 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerHealth { get; private set; }

        /// <summary>
        /// 스태미나(에너지) 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerEnergy { get; private set; }

        /// <summary>
        /// 이동 속도 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerSprintSpeed { get; private set; }

        /// <summary>
        /// 추가 점프 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerExtraJump { get; private set; }

        /// <summary>
        /// 구르기 도약 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerTumbleLaunch { get; private set; }

        /// <summary>
        /// 잡기 범위 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerGrabRange { get; private set; }

        /// <summary>
        /// 잡기 힘 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerGrabStrength { get; private set; }

        /// <summary>
        /// 던지기 힘 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerGrabThrow { get; private set; }

        /// <summary>
        /// 지도 플레이어 수 관련 업그레이드 공유 확률입니다.
        /// </summary>
        public ConfigEntry<int> ChanceToActivateMapPlayerCount { get; private set; }

        /// <summary>
        /// 설정 파일을 초기화하고 속성들을 바인딩합니다.
        /// </summary>
        /// <param name="config">BepInEx 설정 파일 인스턴스</param>
        public UpgradeConfig(ConfigFile config)
        {
            // === Global 섹션 바인딩 ===
            ModEnabled = config.Bind(
                "Global",
                "ModEnabled",
                true,
                "Enable or disable the mod"
            );

            HostOnly = config.Bind(
                "Global",
                "HostOnly",
                true,
                "Only the host will run the mod logic (recommended: true)"
            );

            UseOneChanceForAll = config.Bind(
                "Global",
                "UseOneChanceForAll",
                true,
                "If true, the GlobalChanceToActivate will be used for all upgrades"
            );

            GlobalChanceToActivate = config.Bind(
                "Global",
                "GlobalChanceToActivate",
                25,
                new ConfigDescription(
                    "% Chance to activate the upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToWasteUpgrade = config.Bind(
                "Global",
                "ChanceToWasteUpgrade",
                0,
                new ConfigDescription(
                    "% Chance to waste the upgrade and activate it for nobody",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            // === SpecificUpgrades 섹션 바인딩 ===
            ChanceToActivatePlayerHealth = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerHealth",
                50,
                new ConfigDescription(
                    "% Chance to activate the Health upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerEnergy = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerEnergy",
                25,
                new ConfigDescription(
                    "% Chance to activate the Energy (Stamina) upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerSprintSpeed = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerSprintSpeed",
                25,
                new ConfigDescription(
                    "% Chance to activate the Sprint Speed upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerExtraJump = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerExtraJump",
                25,
                new ConfigDescription(
                    "% Chance to activate the Extra Jump upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerTumbleLaunch = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerTumbleLaunch",
                25,
                new ConfigDescription(
                    "% Chance to activate the Tumble Launch upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerGrabRange = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerGrabRange",
                25,
                new ConfigDescription(
                    "% Chance to activate the Grab Range upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerGrabStrength = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerGrabStrength",
                25,
                new ConfigDescription(
                    "% Chance to activate the Grab Strength upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerGrabThrow = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivatePlayerGrabThrow",
                25,
                new ConfigDescription(
                    "% Chance to activate the Grab Throw upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivateMapPlayerCount = config.Bind(
                "SpecificUpgrades",
                "ChanceToActivateMapPlayerCount",
                25,
                new ConfigDescription(
                    "% Chance to activate the Map Player Count upgrade for every player",
                    new AcceptableValueRange<int>(0, 100)
                )
            );
        }

        /// <summary>
        /// 업그레이드 종류에 따른 공유 확률 수치를 반환합니다.
        /// </summary>
        /// <param name="upgradeType">업그레이드 종류 이름</param>
        /// <returns>공유 확률 (0 ~ 100)</returns>
        public int GetShareChance(string upgradeType)
        {
            // 전역 확률 단일 적용 옵션이 켜져 있으면 전역 확률 반환
            if (UseOneChanceForAll.Value)
            {
                return GlobalChanceToActivate.Value;
            }

            string typeLower = upgradeType.ToLower();

            if (typeLower.Contains("health"))
                return ChanceToActivatePlayerHealth.Value;
            if (typeLower.Contains("energy") || typeLower.Contains("stamina"))
                return ChanceToActivatePlayerEnergy.Value;
            if (typeLower.Contains("sprint") || typeLower.Contains("speed"))
                return ChanceToActivatePlayerSprintSpeed.Value;
            if (typeLower.Contains("jump"))
                return ChanceToActivatePlayerExtraJump.Value;
            if (typeLower.Contains("tumble") || typeLower.Contains("launch"))
                return ChanceToActivatePlayerTumbleLaunch.Value;
            if (typeLower.Contains("range"))
                return ChanceToActivatePlayerGrabRange.Value;
            if (typeLower.Contains("strength"))
                return ChanceToActivatePlayerGrabStrength.Value;
            if (typeLower.Contains("throw"))
                return ChanceToActivatePlayerGrabThrow.Value;
            if (typeLower.Contains("map") || typeLower.Contains("count"))
                return ChanceToActivateMapPlayerCount.Value;

            // 알 수 없는 타입의 경우 전역 확률 사용
            return GlobalChanceToActivate.Value;
        }

        /// <summary>
        /// 업그레이드가 소멸(버려짐)되어야 하는지 확률적으로 계산합니다.
        /// </summary>
        /// <returns>소멸 여부</returns>
        public bool ShouldWasteUpgrade()
        {
            if (ChanceToWasteUpgrade.Value <= 0)
                return false;

            int roll = new System.Random().Next(0, 100);
            return roll < ChanceToWasteUpgrade.Value;
        }
    }
}
