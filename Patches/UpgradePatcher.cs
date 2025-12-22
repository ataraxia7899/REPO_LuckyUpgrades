using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LuckyUpgrades.Patches
{
    /// <summary>
    /// 게임 내 다양한 업그레이드 클래스에 대해 Harmony 패치를 적용하고 
    /// 업그레이드 획득 시 공유 로직을 실행하는 클래스입니다.
    /// </summary>
    public static class UpgradePatcher
    {
        /// <summary>
        /// 확률 계산을 위한 랜덤 객체입니다.
        /// </summary>
        private static readonly System.Random _random = new System.Random();

        /// <summary>
        /// 모든 관련 업그레이드 클래스를 탐색하여 수동으로 Postfix 패치를 적용합니다.
        /// </summary>
        /// <param name="harmony">패치를 적용할 Harmony 인스턴스</param>
        public static void PatchAll(Harmony harmony)
        {
            // 게임 내에 존재하는 것으로 추정되거나 확인된 업그레이드 클래스 목록
            string[] upgradeClasses = new string[]
            {
                "ItemUpgradePlayerHealth",
                "ItemUpgradePlayerEnergy",
                "ItemUpgradePlayerSprintSpeed",
                "ItemUpgradePlayerExtraJump",
                "ItemUpgradePlayerTumbleLaunch",
                "ItemUpgradePlayerGrabRange",
                "ItemUpgradePlayerGrabStrength",
                "ItemUpgradePlayerGrabThrow",
                "ItemUpgradeMapPlayerCount"
            };

            int patchedCount = 0;
            MethodInfo postfix = typeof(UpgradePatcher).GetMethod(nameof(Postfix), BindingFlags.Static | BindingFlags.Public);

            foreach (string className in upgradeClasses)
            {
                Type type = AccessTools.TypeByName(className);
                if (type == null) continue;

                // 로그 분석을 통해 확인된 'Upgrade' 메서드를 우선 탐색
                MethodInfo targetMethod = AccessTools.Method(type, "Upgrade") 
                                         ?? AccessTools.Method(type, "ApplyUpgrade")
                                         ?? AccessTools.Method(type, "Apply");

                if (targetMethod != null)
                {
                    harmony.Patch(targetMethod, postfix: new HarmonyMethod(postfix));
                    Plugin.Log.LogInfo($"[LuckyUpgrades] Patch applied to: {className}.{targetMethod.Name}");
                    patchedCount++;
                }
            }

            // 개별 클래스를 찾지 못한 경우 일반적인 ItemUpgrade 클래스 시도
            if (patchedCount == 0)
            {
                Type genericType = AccessTools.TypeByName("ItemUpgrade");
                if (genericType != null)
                {
                    MethodInfo genericMethod = AccessTools.Method(genericType, "Upgrade") 
                                             ?? AccessTools.Method(genericType, "ApplyUpgrade");
                    if (genericMethod != null)
                    {
                        harmony.Patch(genericMethod, postfix: new HarmonyMethod(postfix));
                        Plugin.Log.LogInfo($"[LuckyUpgrades] Patch applied to generic class: {genericType.Name}");
                    }
                }
            }
        }

        /// <summary>
        /// 업그레이드가 적용된 직후 호출되어 공유 로직을 수행하는 Postfix 메서드입니다.
        /// </summary>
        /// <param name="__instance">업그레이드가 적용된 인스턴스 객체</param>
        public static void Postfix(object __instance)
        {
            try
            {
                if (!Plugin.UpgradeConfiguration.ModEnabled.Value) return;
                
                // 호스트 전용 모드인 경우 호스트인지 확인
                if (Plugin.UpgradeConfiguration.HostOnly.Value && !IsHost()) return;
                
                // 업그레이드 소멸 확률 체크
                if (Plugin.UpgradeConfiguration.ShouldWasteUpgrade())
                {
                    Plugin.Log.LogInfo("[LuckyUpgrades] Upgrade chance failed - Upgrade wasted!");
                    return;
                }

                string upgradeType = GetUpgradeType(__instance);
                object sourcePlayer = GetSourcePlayer(__instance);

                if (string.IsNullOrEmpty(upgradeType) || sourcePlayer == null) return;

                // 다른 플레이어에게 업그레이드 공유 시도
                ApplyUpgradeToOtherPlayers(upgradeType, sourcePlayer);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"[LuckyUpgrades] Error in Postfix patch: {ex.Message}");
            }
        }

        /// <summary>
        /// 현재 플레이어가 방의 호스트(마스터 클라이언트)인지 확인합니다.
        /// </summary>
        private static bool IsHost()
        {
            try
            {
                // Photon 네트워크 상태 확인
                Type photonNetworkType = AccessTools.TypeByName("Photon.Pun.PhotonNetwork");
                if (photonNetworkType != null)
                {
                    PropertyInfo isMasterClientProp = photonNetworkType.GetProperty("IsMasterClient");
                    if (isMasterClientProp != null) return (bool)isMasterClientProp.GetValue(null);
                }

                // GameManager의 호스트 여부 확인
                Type gameManagerType = AccessTools.TypeByName("GameManager");
                if (gameManagerType != null)
                {
                    object instance = AccessTools.Property(gameManagerType, "Instance")?.GetValue(null) 
                                     ?? AccessTools.Field(gameManagerType, "instance")?.GetValue(null);
                    if (instance != null)
                    {
                        return (bool)(AccessTools.Property(gameManagerType, "IsHost")?.GetValue(instance) 
                                     ?? AccessTools.Field(gameManagerType, "isHost")?.GetValue(instance) ?? true);
                    }
                }
                return true;
            }
            catch { return true; }
        }

        /// <summary>
        /// 인스턴스 타입을 분석하여 업그레이드 종류를 문자열로 반환합니다.
        /// </summary>
        private static string GetUpgradeType(object instance)
        {
            string name = instance.GetType().Name;
            if (name.Contains("Health")) return "Health";
            if (name.Contains("Energy") || name.Contains("Stamina")) return "Energy";
            if (name.Contains("Sprint") || name.Contains("Speed")) return "SprintSpeed";
            if (name.Contains("Jump")) return "ExtraJump";
            if (name.Contains("Tumble") || name.Contains("Launch")) return "TumbleLaunch";
            if (name.Contains("Range")) return "GrabRange";
            if (name.Contains("Strength")) return "GrabStrength";
            if (name.Contains("Throw")) return "GrabThrow";
            if (name.Contains("Map") || name.Contains("Count")) return "MapPlayerCount";
            return name;
        }

        /// <summary>
        /// 업그레이드 인스턴스에서 원본 플레이어 객체를 추출합니다.
        /// </summary>
        private static object GetSourcePlayer(object instance)
        {
            Type type = instance.GetType();
            string[] fields = { "player", "Player", "playerController", "PlayerController", "owner", "Owner", "playerAvatar", "PlayerAvatar" };
            foreach (var f in fields)
            {
                var val = AccessTools.Property(type, f)?.GetValue(instance) ?? AccessTools.Field(type, f)?.GetValue(instance);
                if (val != null) return val;
            }
            return null;
        }

        /// <summary>
        /// 원본 플레이어를 제외한 다른 모든 플레이어에게 확률적으로 업그레이드를 배포합니다.
        /// </summary>
        private static void ApplyUpgradeToOtherPlayers(string upgradeType, object sourcePlayer)
        {
            int shareChance = Plugin.UpgradeConfiguration.GetShareChance(upgradeType);
            List<object> allPlayers = GetAllPlayers();

            foreach (object player in allPlayers)
            {
                if (player == null || player.Equals(sourcePlayer)) continue;

                // 확률 주사위 굴리기
                if (_random.Next(0, 100) < shareChance)
                {
                    if (ApplyUpgradeToPlayer(player, upgradeType))
                    {
                        Plugin.Log.LogInfo($"[LuckyUpgrades] Success! {upgradeType} upgrade shared with another player (Chance: {shareChance}%)");
                    }
                }
            }
        }

        /// <summary>
        /// 게임 내 활성화된 모든 플레이어 목록을 가져옵니다.
        /// </summary>
        private static List<object> GetAllPlayers()
        {
            List<object> players = new List<object>();
            Type pcType = AccessTools.TypeByName("PlayerController");
            if (pcType != null)
            {
                var result = AccessTools.Method(pcType, "GetAllPlayers")?.Invoke(null, null) 
                             ?? AccessTools.Method(pcType, "GetAll")?.Invoke(null, null);
                if (result is System.Collections.IEnumerable en) { foreach (var p in en) players.Add(p); return players; }
            }

            Type gmType = AccessTools.TypeByName("GameManager");
            if (gmType != null)
            {
                object instance = AccessTools.Property(gmType, "Instance")?.GetValue(null) ?? AccessTools.Field(gmType, "instance")?.GetValue(null);
                if (instance != null)
                {
                    var result = AccessTools.Property(gmType, "players")?.GetValue(instance) ?? AccessTools.Field(gmType, "players")?.GetValue(instance);
                    if (result is System.Collections.IEnumerable en) { foreach (var p in en) players.Add(p); return players; }
                }
            }

            // 최후의 수단으로 FindObjectsOfType 사용
            if (pcType != null)
            {
                foreach (var p in UnityEngine.Object.FindObjectsOfType(pcType)) players.Add(p);
            }
            return players;
        }

        /// <summary>
        /// 특정 플레이어에게 지정된 업그레이드 효과를 적용합니다.
        /// </summary>
        private static bool ApplyUpgradeToPlayer(object player, string upgradeType)
        {
            try
            {
                Type smType = AccessTools.TypeByName("StatsManager");
                if (smType != null)
                {
                    // 플레이어 객체에서 StatsManager 인스턴스 추출 시도
                    object sm = AccessTools.Property(player.GetType(), "statsManager")?.GetValue(player) 
                               ?? AccessTools.Field(player.GetType(), "statsManager")?.GetValue(player)
                               ?? (player as Component)?.GetComponent(smType);
                    
                    if (sm != null)
                    {
                        // 업그레이드 적용 메서드 호출
                        MethodInfo addMethod = AccessTools.Method(smType, "AddUpgrade") 
                                              ?? AccessTools.Method(smType, "ApplyUpgrade")
                                              ?? AccessTools.Method(smType, "IncreaseLevel");
                        if (addMethod != null)
                        {
                            addMethod.Invoke(sm, new object[] { upgradeType });
                            return true;
                        }
                    }
                }
                return false;
            }
            catch { return false; }
        }
    }
}
