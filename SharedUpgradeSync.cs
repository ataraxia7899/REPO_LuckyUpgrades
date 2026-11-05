using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace LuckyUpgrades;

internal static class SharedUpgradeSync
{
    // These are the game's stat dictionary names, not the configurable upgrade names.
    internal static readonly Dictionary<string, string> StatNames = new Dictionary<string, string>
    {
        { "Health", "playerUpgradeHealth" },
        { "Energy", "playerUpgradeStamina" },
        { "ExtraJump", "playerUpgradeExtraJump" },
        { "GrabRange", "playerUpgradeRange" },
        { "GrabStrength", "playerUpgradeStrength" },
        { "GrabThrow", "playerUpgradeThrow" },
        { "SprintSpeed", "playerUpgradeSpeed" },
        { "TumbleLaunch", "playerUpgradeLaunch" },
        { "MapPlayerCount", "playerUpgradeMapPlayerCount" },
        { "TumbleClimb", "playerUpgradeTumbleClimb" },
        { "TumbleWings", "playerUpgradeTumbleWings" },
        { "CrouchRest", "playerUpgradeCrouchRest" },
        { "DeathHeadBattery", "playerUpgradeDeathHeadBattery" }
    };

    internal static void Merge(Hashtable data, string steamID,
        Dictionary<string, int> sharedUpgrades, Action<string> log)
    {
        if (data == null || string.IsNullOrEmpty(steamID)) return;

        foreach (var upgrade in sharedUpgrades)
        {
            if (upgrade.Value <= 0 || !StatNames.TryGetValue(upgrade.Key, out string statName)) continue;
            // Delta syncs omit unchanged dictionaries. Never add a missing dictionary.
            if (!(data[statName] is Hashtable hostStats)) continue;
            int hostValue = hostStats.TryGetValue(steamID, out object rawValue) ? (int)rawValue : 0;
            int mergedValue = checked(hostValue + upgrade.Value);

            // Keep the original RPC payload intact in case another patch reads it.
            var mergedStats = new Hashtable();
            foreach (var entry in hostStats) mergedStats[entry.Key] = entry.Value;
            mergedStats[steamID] = mergedValue;
            data[statName] = mergedStats;
            log($"[LuckyUpgrades] Sync {upgrade.Key}: host={hostValue}, shared={upgrade.Value}, merged={mergedValue}");
        }
    }
}
