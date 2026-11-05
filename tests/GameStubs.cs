// Minimal managed stand-ins let the real plugin hooks run without starting Unity.
using LuckyUpgrades;
namespace ExitGames.Client.Photon {
    public class Hashtable : Dictionary<object, object> {
        public new object this[object key] {
            get => TryGetValue(key, out var value) ? value : null;
            set => base[key] = value;
        }
    }
}
namespace Photon.Pun {
    public struct PhotonMessageInfo { public object Sender { get; set; } }
    public static class PhotonNetwork {
        public static bool InRoom = true;
        public static bool IsMasterClient;
        public static object MasterClient = new object();
    }
}
namespace BepInEx.Logging {
    public class ManualLogSource {
        public void LogInfo(object message) { }
        public void LogError(object message) => throw new Exception(message.ToString());
    }
}
namespace BepInEx {
    public class BepInPlugin : Attribute { public BepInPlugin(string a, string b, string c) { } }
    public class BepInProcess : Attribute { public BepInProcess(string a) { } }
    public class BaseUnityPlugin {
        public BepInEx.Logging.ManualLogSource Logger = new();
        public object Config = new();
    }
}
namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string method) { } }
    public class HarmonyPrefix : Attribute { }
    public class HarmonyPostfix : Attribute { }
    public class Harmony { public Harmony(string id) { } public void PatchAll(Type type) { } }
}
namespace UnityEngine {
    public class Object {
        public static void DontDestroyOnLoad(object value) { }
        public static implicit operator bool(Object value) => value != null;
    }
    public class MonoBehaviour : Object { }
    public enum HideFlags { HideAndDontSave }
    public class GameObject : Object {
        public GameObject(string name) { }
        public T AddComponent<T>() where T : new() => new T();
        public HideFlags hideFlags;
    }
}
namespace LuckyUpgrades {
    public class UpgradeConfig {
        public static int Chance = 100;
        public UpgradeConfig(object config) { }
        public int GetShareChance(string type) => Chance;
    }
    public class MyPluginInfo {
        public const string PLUGIN_GUID = "LuckyUpgrades";
        public const string PLUGIN_NAME = "LuckyUpgrades";
        public const string PLUGIN_VERSION = "test";
    }
}
public class PlayerAvatar { public string SteamID = "guest"; }
public static class SemiFunc {
    public static PlayerAvatar Local = new();
    public static PlayerAvatar PlayerAvatarLocal() => Local;
    public static string PlayerGetSteamID(PlayerAvatar player) => player.SteamID;
}
public class Level { public string name; }
public class RunManager {
    public static RunManager instance = new();
    public Level levelCurrent;
}
public class StatsManager {
    public static StatsManager instance = new();
    public Dictionary<string, Dictionary<string, int>> dictionaryOfDictionaries =
        SharedUpgradeSync.StatNames.Values.ToDictionary(name => name, name => new Dictionary<string, int>());
}
public class PunManager {
    public static PunManager instance = new();
    private int Apply(string upgrade, string id, int value, Action<string, int> postfix) {
        var stats = StatsManager.instance.dictionaryOfDictionaries[SharedUpgradeSync.StatNames[upgrade]];
        stats[id] = Math.Max(0, stats.GetValueOrDefault(id) + value);
        postfix(id, value);
        return stats[id];
    }
    public int UpgradePlayerHealth(string id, int value = 1) => Apply("Health", id, value, Plugin.UpgradePlayerHealth_Postfix);
    public int UpgradePlayerEnergy(string id, int value = 1) => Apply("Energy", id, value, Plugin.UpgradePlayerEnergy_Postfix);
    public int UpgradePlayerExtraJump(string id, int value = 1) => Apply("ExtraJump", id, value, Plugin.UpgradePlayerExtraJump_Postfix);
    public int UpgradePlayerGrabRange(string id, int value = 1) => Apply("GrabRange", id, value, Plugin.UpgradePlayerGrabRange_Postfix);
    public int UpgradePlayerGrabStrength(string id, int value = 1) => Apply("GrabStrength", id, value, Plugin.UpgradePlayerGrabStrength_Postfix);
    public int UpgradePlayerThrowStrength(string id, int value = 1) => Apply("GrabThrow", id, value, Plugin.UpgradePlayerThrowStrength_Postfix);
    public int UpgradePlayerSprintSpeed(string id, int value = 1) => Apply("SprintSpeed", id, value, Plugin.UpgradePlayerSprintSpeed_Postfix);
    public int UpgradePlayerTumbleLaunch(string id, int value = 1) => Apply("TumbleLaunch", id, value, Plugin.UpgradePlayerTumbleLaunch_Postfix);
    public int UpgradeMapPlayerCount(string id, int value = 1) => Apply("MapPlayerCount", id, value, Plugin.UpgradeMapPlayerCount_Postfix);
    public int UpgradePlayerTumbleClimb(string id, int value = 1) => Apply("TumbleClimb", id, value, Plugin.UpgradePlayerTumbleClimb_Postfix);
    public int UpgradePlayerTumbleWings(string id, int value = 1) => Apply("TumbleWings", id, value, Plugin.UpgradePlayerTumbleWings_Postfix);
    public int UpgradePlayerCrouchRest(string id, int value = 1) => Apply("CrouchRest", id, value, Plugin.UpgradePlayerCrouchRest_Postfix);
    public int UpgradeDeathHeadBattery(string id, int value = 1) => Apply("DeathHeadBattery", id, value, Plugin.UpgradeDeathHeadBattery_Postfix);
    public void ReceiveSyncData(ExitGames.Client.Photon.Hashtable data, bool finalChunk, Photon.Pun.PhotonMessageInfo info) {
        Plugin.ReceiveSyncData_Prefix(ref data, info, out bool merged);
        foreach (var entry in data) {
            if (entry.Key is string name && entry.Value is ExitGames.Client.Photon.Hashtable values &&
                StatsManager.instance.dictionaryOfDictionaries.TryGetValue(name, out var stats)) {
                stats.Clear();
                foreach (var value in values) stats[(string)value.Key] = (int)value.Value;
            }
        }
        Plugin.ReceiveSyncData_Postfix(data, merged);
    }
}
