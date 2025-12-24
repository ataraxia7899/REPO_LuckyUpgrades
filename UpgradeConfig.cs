using BepInEx.Configuration;

namespace LuckyUpgrades
{
    /// <summary>
    /// Manages upgrade sharing probability settings.
    /// Defines the chance for each upgrade type to be shared with other players.
    /// </summary>
    public class UpgradeConfig
    {
        // === Upgrade Chances ===

        /// <summary>
        /// Health upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerHealth { get; private set; }

        /// <summary>
        /// Energy (Stamina) upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerEnergy { get; private set; }

        /// <summary>
        /// Sprint Speed upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerSprintSpeed { get; private set; }

        /// <summary>
        /// Extra Jump upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerExtraJump { get; private set; }

        /// <summary>
        /// Tumble Launch upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerTumbleLaunch { get; private set; }

        /// <summary>
        /// Grab Range upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerGrabRange { get; private set; }

        /// <summary>
        /// Grab Strength upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerGrabStrength { get; private set; }

        /// <summary>
        /// Grab Throw upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerGrabThrow { get; private set; }

        /// <summary>
        /// Tumble Climb upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerTumbleClimb { get; private set; }

        /// <summary>
        /// Tumble Wings upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerTumbleWings { get; private set; }

        /// <summary>
        /// Crouch Rest upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivatePlayerCrouchRest { get; private set; }

        /// <summary>
        /// Death Head Battery upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivateDeathHeadBattery { get; private set; }

        /// <summary>
        /// Map Player Count upgrade share chance.
        /// </summary>
        public ConfigEntry<int> ChanceToActivateMapPlayerCount { get; private set; }

        /// <summary>
        /// Initializes the config file and binds all settings.
        /// </summary>
        public UpgradeConfig(ConfigFile config)
        {
            ChanceToActivatePlayerHealth = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerHealth",
                25,
                new ConfigDescription(
                    "% Chance to share the Health upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerEnergy = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerEnergy",
                25,
                new ConfigDescription(
                    "% Chance to share the Energy (Stamina) upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerSprintSpeed = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerSprintSpeed",
                25,
                new ConfigDescription(
                    "% Chance to share the Sprint Speed upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerExtraJump = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerExtraJump",
                25,
                new ConfigDescription(
                    "% Chance to share the Extra Jump upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerTumbleLaunch = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerTumbleLaunch",
                25,
                new ConfigDescription(
                    "% Chance to share the Tumble Launch upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerGrabRange = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerGrabRange",
                25,
                new ConfigDescription(
                    "% Chance to share the Grab Range upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerGrabStrength = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerGrabStrength",
                25,
                new ConfigDescription(
                    "% Chance to share the Grab Strength upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerGrabThrow = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerGrabThrow",
                25,
                new ConfigDescription(
                    "% Chance to share the Grab Throw upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerTumbleClimb = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerTumbleClimb",
                25,
                new ConfigDescription(
                    "% Chance to share the Tumble Climb upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerTumbleWings = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerTumbleWings",
                25,
                new ConfigDescription(
                    "% Chance to share the Tumble Wings upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivatePlayerCrouchRest = config.Bind(
                "Upgrades",
                "ChanceToActivatePlayerCrouchRest",
                25,
                new ConfigDescription(
                    "% Chance to share the Crouch Rest upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivateDeathHeadBattery = config.Bind(
                "Upgrades",
                "ChanceToActivateDeathHeadBattery",
                25,
                new ConfigDescription(
                    "% Chance to share the Death Head Battery upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );

            ChanceToActivateMapPlayerCount = config.Bind(
                "Upgrades",
                "ChanceToActivateMapPlayerCount",
                25,
                new ConfigDescription(
                    "% Chance to share the Map Player Count upgrade",
                    new AcceptableValueRange<int>(0, 100)
                )
            );
        }

        /// <summary>
        /// Gets the share chance for the specified upgrade type.
        /// </summary>
        public int GetShareChance(string upgradeType)
        {
            string typeLower = upgradeType.ToLower();

            if (typeLower.Contains("health"))
                return ChanceToActivatePlayerHealth.Value;
            if (typeLower.Contains("energy") || typeLower.Contains("stamina"))
                return ChanceToActivatePlayerEnergy.Value;
            if (typeLower.Contains("sprint") || typeLower.Contains("speed"))
                return ChanceToActivatePlayerSprintSpeed.Value;
            if (typeLower.Contains("jump"))
                return ChanceToActivatePlayerExtraJump.Value;
            if (typeLower.Contains("tumblelaunch") || typeLower.Contains("launch"))
                return ChanceToActivatePlayerTumbleLaunch.Value;
            if (typeLower.Contains("tumbleclimb") || typeLower.Contains("climb"))
                return ChanceToActivatePlayerTumbleClimb.Value;
            if (typeLower.Contains("tumblewing") || typeLower.Contains("wing"))
                return ChanceToActivatePlayerTumbleWings.Value;
            if (typeLower.Contains("crouch") || typeLower.Contains("rest"))
                return ChanceToActivatePlayerCrouchRest.Value;
            if (typeLower.Contains("deathhead") || typeLower.Contains("battery"))
                return ChanceToActivateDeathHeadBattery.Value;
            if (typeLower.Contains("range"))
                return ChanceToActivatePlayerGrabRange.Value;
            if (typeLower.Contains("strength"))
                return ChanceToActivatePlayerGrabStrength.Value;
            if (typeLower.Contains("throw"))
                return ChanceToActivatePlayerGrabThrow.Value;
            if (typeLower.Contains("map") || typeLower.Contains("count"))
                return ChanceToActivateMapPlayerCount.Value;

            // Default chance for unknown types
            return 25;
        }
    }
}
