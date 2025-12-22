using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LuckyUpgrades.Patches
{
    /// <summary>
    /// Harmony patch that applies upgrades to other players based on probability
    /// 
    /// Note: Actual game class/method names may need adjustment after dnSpy analysis
    /// Currently using discovered method: ItemUpgradePlayerHealth.Upgrade
    /// </summary>
    [HarmonyPatch]
    public class UpgradePatch
    {
        /// <summary>
        /// Random instance for probability calculation
        /// </summary>
        private static readonly System.Random _random = new System.Random();

        /// <summary>
        /// Dynamically specify the patch target method
        /// </summary>
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            // List of possible type names to search
            string[] possibleTypeNames = new string[]
            {
                "ItemUpgradePlayerHealth",
                "ItemUpgradePlayerEnergy",
                "ItemUpgradePlayerSprintSpeed",
                "ItemUpgradePlayerExtraJump",
                "ItemUpgradePlayerTumbleLaunch",
                "ItemUpgradePlayerGrabRange",
                "ItemUpgradePlayerGrabStrength",
                "ItemUpgradePlayerGrabThrow",
                "ItemUpgradeMapPlayerCount",
                "ItemUpgrade",
                "UpgradeItem",
                "PlayerUpgrade",
                "StatsManager"
            };

            // List of possible method names to search
            string[] possibleMethodNames = new string[]
            {
                "Upgrade",
                "ApplyUpgrade",
                "Apply",
                "AddUpgrade",
                "PickUp",
                "OnPickup"
            };

            foreach (string typeName in possibleTypeNames)
            {
                Type type = AccessTools.TypeByName(typeName);
                if (type == null) continue;

                foreach (string methodName in possibleMethodNames)
                {
                    MethodInfo method = AccessTools.Method(type, methodName);
                    if (method != null)
                    {
                        Plugin.Log.LogInfo($"[LuckyUpgrades] Patch target found: {typeName}.{methodName}");
                        return method;
                    }
                }
            }

            Plugin.Log.LogWarning("[LuckyUpgrades] Could not find patch target method. Manual configuration required.");
            return null;
        }

        /// <summary>
        /// Postfix patch executed after upgrade is applied
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(object __instance)
        {
            try
            {
                // Skip if mod is disabled
                if (!Plugin.UpgradeConfiguration.ModEnabled.Value)
                    return;

                // Skip if host-only mode and not host
                if (Plugin.UpgradeConfiguration.HostOnly.Value && !IsHost())
                    return;

                // Check if upgrade should be wasted
                if (Plugin.UpgradeConfiguration.ShouldWasteUpgrade())
                {
                    Plugin.Log.LogInfo("[LuckyUpgrades] Upgrade wasted - no one gets it!");
                    return;
                }

                // Get upgrade type
                string upgradeType = GetUpgradeType(__instance);
                if (string.IsNullOrEmpty(upgradeType))
                {
                    Plugin.Log.LogWarning("[LuckyUpgrades] Could not determine upgrade type.");
                    return;
                }

                // Get source player
                object sourcePlayer = GetSourcePlayer(__instance);
                if (sourcePlayer == null)
                {
                    Plugin.Log.LogWarning("[LuckyUpgrades] Could not determine source player.");
                    return;
                }

                // Apply upgrade to other players based on probability
                ApplyUpgradeToOtherPlayers(upgradeType, sourcePlayer);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[LuckyUpgrades] Error in Postfix: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if current client is the host
        /// </summary>
        private static bool IsHost()
        {
            try
            {
                // Check Photon PUN
                Type photonNetworkType = AccessTools.TypeByName("Photon.Pun.PhotonNetwork");
                if (photonNetworkType != null)
                {
                    PropertyInfo isMasterClientProp = photonNetworkType.GetProperty("IsMasterClient");
                    if (isMasterClientProp != null)
                    {
                        return (bool)isMasterClientProp.GetValue(null);
                    }
                }

                // Check GameManager
                Type gameManagerType = AccessTools.TypeByName("GameManager");
                if (gameManagerType != null)
                {
                    PropertyInfo isHostProp = gameManagerType.GetProperty("IsHost") 
                                            ?? gameManagerType.GetProperty("isHost");
                    if (isHostProp != null)
                    {
                        object instance = GetGameManagerInstance();
                        if (instance != null)
                            return (bool)isHostProp.GetValue(instance);
                    }
                }

                // Default: assume host
                return true;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Get GameManager instance
        /// </summary>
        private static object GetGameManagerInstance()
        {
            try
            {
                Type gameManagerType = AccessTools.TypeByName("GameManager");
                if (gameManagerType == null) return null;

                // Find singleton instance
                PropertyInfo instanceProp = gameManagerType.GetProperty("Instance") 
                                           ?? gameManagerType.GetProperty("instance");
                if (instanceProp != null)
                    return instanceProp.GetValue(null);

                FieldInfo instanceField = gameManagerType.GetField("Instance") 
                                         ?? gameManagerType.GetField("instance");
                if (instanceField != null)
                    return instanceField.GetValue(null);

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get upgrade type name from instance
        /// </summary>
        private static string GetUpgradeType(object instance)
        {
            if (instance == null) return null;

            try
            {
                Type type = instance.GetType();

                // Extract from class name
                string typeName = type.Name;
                if (typeName.Contains("Health")) return "Health";
                if (typeName.Contains("Energy") || typeName.Contains("Stamina")) return "Energy";
                if (typeName.Contains("Sprint") || typeName.Contains("Speed")) return "SprintSpeed";
                if (typeName.Contains("Jump")) return "ExtraJump";
                if (typeName.Contains("Tumble") || typeName.Contains("Launch")) return "TumbleLaunch";
                if (typeName.Contains("Range")) return "GrabRange";
                if (typeName.Contains("Strength")) return "GrabStrength";
                if (typeName.Contains("Throw")) return "GrabThrow";
                if (typeName.Contains("Map") || typeName.Contains("Count")) return "MapPlayerCount";

                // Try to get from upgradeType field/property
                PropertyInfo upgradeTypeProp = type.GetProperty("upgradeType") 
                                              ?? type.GetProperty("UpgradeType");
                if (upgradeTypeProp != null)
                    return upgradeTypeProp.GetValue(instance)?.ToString();

                FieldInfo upgradeTypeField = type.GetField("upgradeType") 
                                            ?? type.GetField("UpgradeType");
                if (upgradeTypeField != null)
                    return upgradeTypeField.GetValue(instance)?.ToString();

                return typeName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get the player who picked up the upgrade
        /// </summary>
        private static object GetSourcePlayer(object instance)
        {
            if (instance == null) return null;

            try
            {
                Type type = instance.GetType();

                // Look for player-related fields/properties
                string[] playerFieldNames = { "player", "Player", "playerController", "PlayerController", "owner", "Owner", "playerAvatar", "PlayerAvatar" };
                
                foreach (string fieldName in playerFieldNames)
                {
                    PropertyInfo prop = type.GetProperty(fieldName);
                    if (prop != null)
                        return prop.GetValue(instance);

                    FieldInfo field = type.GetField(fieldName);
                    if (field != null)
                        return field.GetValue(instance);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Apply upgrade to other players based on probability
        /// </summary>
        private static void ApplyUpgradeToOtherPlayers(string upgradeType, object sourcePlayer)
        {
            try
            {
                // Get share chance (0-100)
                int shareChance = Plugin.UpgradeConfiguration.GetShareChance(upgradeType);

                // Get all players
                List<object> allPlayers = GetAllPlayers();
                if (allPlayers == null || allPlayers.Count <= 1)
                {
                    Plugin.Log.LogDebug("[LuckyUpgrades] No other players found.");
                    return;
                }

                int sharedCount = 0;
                foreach (object player in allPlayers)
                {
                    // Skip source player
                    if (IsSamePlayer(player, sourcePlayer))
                        continue;

                    // Probability check (0-100)
                    int roll = _random.Next(0, 100);
                    if (roll < shareChance)
                    {
                        // Apply upgrade
                        bool success = ApplyUpgradeToPlayer(player, upgradeType);
                        if (success)
                        {
                            sharedCount++;
                            Plugin.Log.LogInfo($"[LuckyUpgrades] {upgradeType} upgrade shared to player (chance: {shareChance}%)");
                        }
                    }
                    else
                    {
                        Plugin.Log.LogDebug($"[LuckyUpgrades] {upgradeType} upgrade not shared (roll: {roll}, chance: {shareChance})");
                    }
                }

                if (sharedCount > 0)
                {
                    Plugin.Log.LogInfo($"[LuckyUpgrades] {upgradeType} upgrade shared to {sharedCount} players total");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[LuckyUpgrades] Error sharing upgrade: {ex.Message}");
            }
        }

        /// <summary>
        /// Get list of all players
        /// </summary>
        private static List<object> GetAllPlayers()
        {
            try
            {
                List<object> players = new List<object>();

                // Try PlayerController.GetAllPlayers()
                Type playerControllerType = AccessTools.TypeByName("PlayerController");
                if (playerControllerType != null)
                {
                    MethodInfo getAllMethod = playerControllerType.GetMethod("GetAllPlayers") 
                                             ?? playerControllerType.GetMethod("GetAll");
                    if (getAllMethod != null && getAllMethod.IsStatic)
                    {
                        var result = getAllMethod.Invoke(null, null);
                        if (result is System.Collections.IEnumerable enumerable)
                        {
                            foreach (var player in enumerable)
                                players.Add(player);
                            return players;
                        }
                    }
                }

                // Try GameManager.players
                object gameManager = GetGameManagerInstance();
                if (gameManager != null)
                {
                    Type gmType = gameManager.GetType();
                    PropertyInfo playersProp = gmType.GetProperty("players") 
                                              ?? gmType.GetProperty("Players");
                    if (playersProp != null)
                    {
                        var result = playersProp.GetValue(gameManager);
                        if (result is System.Collections.IEnumerable enumerable)
                        {
                            foreach (var player in enumerable)
                                players.Add(player);
                            return players;
                        }
                    }
                }

                // Try FindObjectsOfType
                if (playerControllerType != null)
                {
                    var foundPlayers = UnityEngine.Object.FindObjectsOfType(playerControllerType);
                    foreach (var player in foundPlayers)
                        players.Add(player);
                    return players;
                }

                // Try PlayerAvatar
                Type playerAvatarType = AccessTools.TypeByName("PlayerAvatar");
                if (playerAvatarType != null)
                {
                    var foundPlayers = UnityEngine.Object.FindObjectsOfType(playerAvatarType);
                    foreach (var player in foundPlayers)
                        players.Add(player);
                    return players;
                }

                return players;
            }
            catch
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Check if two players are the same
        /// </summary>
        private static bool IsSamePlayer(object player1, object player2)
        {
            if (player1 == null || player2 == null) return false;
            return player1.Equals(player2) || ReferenceEquals(player1, player2);
        }

        /// <summary>
        /// Apply upgrade to a specific player
        /// </summary>
        private static bool ApplyUpgradeToPlayer(object player, string upgradeType)
        {
            try
            {
                // Try StatsManager
                Type statsManagerType = AccessTools.TypeByName("StatsManager");
                if (statsManagerType != null)
                {
                    object statsManager = GetStatsManagerForPlayer(player);
                    if (statsManager != null)
                    {
                        MethodInfo addMethod = statsManagerType.GetMethod("AddUpgrade") 
                                              ?? statsManagerType.GetMethod("ApplyUpgrade")
                                              ?? statsManagerType.GetMethod("IncreaseLevel");
                        if (addMethod != null)
                        {
                            addMethod.Invoke(statsManager, new object[] { upgradeType });
                            return true;
                        }
                    }
                }

                // Try direct stat modification
                switch (upgradeType.ToLower())
                {
                    case "health":
                        return ModifyPlayerStat(player, "maxHealth", 20f, true);
                    case "energy":
                        return ModifyPlayerStat(player, "maxEnergy", 10f, true);
                    case "sprintspeed":
                        return ModifyPlayerStat(player, "sprintSpeed", 0.1f, true);
                    case "extrajump":
                        return ModifyPlayerStat(player, "extraJumps", 1f, true);
                    case "tumblelaunch":
                        return ModifyPlayerStat(player, "tumbleLaunchLevel", 1f, true);
                    case "grabrange":
                        return ModifyPlayerStat(player, "grabRange", 0.5f, true);
                    case "grabstrength":
                        return ModifyPlayerStat(player, "grabStrength", 1f, true);
                    case "grabthrow":
                        return ModifyPlayerStat(player, "throwStrength", 1f, true);
                }

                return false;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[LuckyUpgrades] Failed to apply upgrade to player: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get StatsManager for a player
        /// </summary>
        private static object GetStatsManagerForPlayer(object player)
        {
            if (player == null) return null;

            try
            {
                Type playerType = player.GetType();

                // Look for statsManager field/property
                PropertyInfo prop = playerType.GetProperty("statsManager") 
                                   ?? playerType.GetProperty("StatsManager")
                                   ?? playerType.GetProperty("stats");
                if (prop != null)
                    return prop.GetValue(player);

                FieldInfo field = playerType.GetField("statsManager") 
                                 ?? playerType.GetField("StatsManager")
                                 ?? playerType.GetField("stats");
                if (field != null)
                    return field.GetValue(player);

                // Try GetComponent
                if (player is Component component)
                {
                    Type statsManagerType = AccessTools.TypeByName("StatsManager");
                    if (statsManagerType != null)
                    {
                        MethodInfo getComponentMethod = typeof(Component).GetMethod("GetComponent", Type.EmptyTypes)
                            ?.MakeGenericMethod(statsManagerType);
                        if (getComponentMethod != null)
                            return getComponentMethod.Invoke(component, null);
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Modify a player stat
        /// </summary>
        private static bool ModifyPlayerStat(object player, string statName, float amount, bool isAdditive)
        {
            if (player == null) return false;

            try
            {
                Type playerType = player.GetType();

                // Find property
                PropertyInfo prop = playerType.GetProperty(statName);
                if (prop != null && prop.CanWrite)
                {
                    float currentValue = Convert.ToSingle(prop.GetValue(player));
                    float newValue = isAdditive ? currentValue + amount : currentValue * amount;
                    prop.SetValue(player, newValue);
                    return true;
                }

                // Find field
                FieldInfo field = playerType.GetField(statName);
                if (field != null)
                {
                    float currentValue = Convert.ToSingle(field.GetValue(player));
                    float newValue = isAdditive ? currentValue + amount : currentValue * amount;
                    field.SetValue(player, newValue);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
