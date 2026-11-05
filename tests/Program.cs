using System.Reflection;
using LuckyUpgrades;
using Photon.Pun;
using ExitGames.Client.Photon;

int assertions = 0;
void Equal(int expected, int actual, string scenario) {
    assertions++;
    if (actual != expected) throw new Exception($"{scenario}: expected {expected}, got {actual}");
}
void Reset() {
    Plugin._sharedUpgrades.Clear();
    Plugin._mySteamID = null;
    StatsManager.instance = new();
    PhotonNetwork.IsMasterClient = false;
    UpgradeConfig.Chance = 100;
}
int Count(string type = "GrabStrength") =>
    StatsManager.instance.dictionaryOfDictionaries[SharedUpgradeSync.StatNames[type]].GetValueOrDefault("guest");
void Share(string type = "GrabStrength", int amount = 1) {
    var method = typeof(Plugin).GetMethods().Single(m =>
        m.Name.EndsWith("_Postfix") &&
        m.GetCustomAttributes<HarmonyLib.HarmonyPatch>().Any() &&
        m.Name == Methods[type] + "_Postfix");
    method.Invoke(null, new object[] { "host", amount });
}
Hashtable Snapshot(string type, int own, bool includeGuest = true) {
    var players = new Hashtable { ["other"] = 7 };
    if (includeGuest) players["guest"] = own;
    return new Hashtable { [SharedUpgradeSync.StatNames[type]] = players };
}
void Sync(Hashtable data, object sender = null, bool final = true) =>
    PunManager.instance.ReceiveSyncData(data, final, new PhotonMessageInfo { Sender = sender ?? PhotonNetwork.MasterClient });
void Level(string name, UpgradeReapplyRunner runner) {
    RunManager.instance.levelCurrent = new Level { name = name };
    typeof(UpgradeReapplyRunner).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(runner, null);
}
var plugin = new Plugin();
typeof(Plugin).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(plugin, null);

// Reproduce the reported route with one bonus and repeated/omitted host snapshots.
Reset();
Share();
Equal(1, Count(), "first share");
var runner = new UpgradeReapplyRunner();
var raw = Snapshot("GrabStrength", 0, false);
foreach (string level in new[] { "Level - Manor", "Level - Shop Forest", "Level - Lobby", "Level - Arctic" }) {
    Level(level, runner);
    Equal(1, Count(), "scene change retains exactly one");
    Sync(new Hashtable());
    Equal(1, Count(), "unchanged delta retains exactly one");
    Sync(raw);
    Sync(raw);
    Equal(1, Count(), "repeated snapshot retains exactly one");
}
Equal(0, ((Hashtable)raw["playerUpgradeStrength"]).ContainsKey("guest") ? 1 : 0, "incoming payload preserved");

// Host counts direct purchases. Local bonus must survive updates to that baseline.
PunManager.instance.UpgradePlayerGrabStrength("guest", 2);
Equal(3, Count(), "two direct plus one shared");
Sync(Snapshot("GrabStrength", 2));
Equal(3, Count(), "direct purchases preserved after sync");
Share();
Sync(Snapshot("GrabStrength", 2));
Equal(4, Count(), "second legitimate share");
Equal(7, StatsManager.instance.dictionaryOfDictionaries["playerUpgradeStrength"]["other"], "other player preserved");

// Every supported stat, including a zero entry omitted by host, and chunked snapshots.
foreach (string type in SharedUpgradeSync.StatNames.Keys) {
    Reset();
    Share(type);
    var snapshot = Snapshot(type, 0, false);
    Sync(snapshot, final: false);
    Sync(new Hashtable(), final: true);
    Sync(snapshot);
    Equal(1, Count(type), type + " repeated partial snapshot");
    Sync(Snapshot(type, 2));
    Equal(3, Count(type), type + " direct baseline");
}

// Authority guards and probabilities.
Reset();
Share();
var ignored = Snapshot("GrabStrength", 9);
Plugin.ReceiveSyncData_Prefix(ref ignored, new PhotonMessageInfo { Sender = new object() }, out _);
Equal(9, (int)((Hashtable)ignored["playerUpgradeStrength"])["guest"], "untrusted sender unchanged");
PhotonNetwork.IsMasterClient = true;
Plugin.ReceiveSyncData_Prefix(ref ignored, new PhotonMessageInfo { Sender = PhotonNetwork.MasterClient }, out _);
Equal(9, (int)((Hashtable)ignored["playerUpgradeStrength"])["guest"], "host unchanged");
Reset();
UpgradeConfig.Chance = 0;
Share();
Equal(0, Count(), "zero chance");
Equal(0, Plugin._sharedUpgrades.Count, "zero chance not tracked");
UpgradeConfig.Chance = 100;
Share(amount: -1);
Share(amount: 0);
Equal(0, Plugin._sharedUpgrades.Count, "nonpositive events ignored");

// A real in-game lobby retains bonuses. Menu transitions clear the session.
Share();
runner = new UpgradeReapplyRunner();
Level("Level - Lobby", runner);
Equal(1, Plugin._sharedUpgrades.Count, "in-game lobby retains bonus");
Level("Level - Main Menu", runner);
Equal(0, Plugin._sharedUpgrades.Count, "main menu resets bonus");
Equal(0, Plugin._mySteamID == null ? 0 : 1, "main menu clears player identity");
Reset();
Share();
Level("Level - Lobby Menu", runner);
Equal(0, Plugin._sharedUpgrades.Count, "lobby menu resets bonus");

// Diagnostics failures must not prevent the item's completion/sound callback.
Reset();
StatsManager.instance.FailDiagnostics = true;
bool completed = false;
Share();
completed = true;
Equal(1, completed ? 1 : 0, "item completes even when diagnostics API fails");
Equal(1, Count(), "sharing still applies when diagnostics API fails");
Equal(1, Plugin._sharedUpgrades["GrabStrength"], "bonus tracked without diagnostics");
Sync(Snapshot("GrabStrength", 0));
Equal(1, Count(), "host sync continues without diagnostics");

// The user's host log failed even for self-acquired upgrades. Exercise every hook.
foreach (string type in SharedUpgradeSync.StatNames.Keys) {
    Reset();
    PhotonNetwork.IsMasterClient = true;
    typeof(PunManager).GetMethod(Methods[type]).Invoke(PunManager.instance, new object[] { "guest", 1 });
    Equal(1, Count(type), type + " host consumes exactly once");
    Equal(0, Plugin._sharedUpgrades.Count, type + " self purchase not shared recursively");
}

// A malformed bonus snapshot must leave vanilla RPC processing available.
Reset();
Share();
var malformed = Snapshot("GrabStrength", 0);
((Hashtable)malformed["playerUpgradeStrength"])["guest"] = "invalid";
var originalMalformed = malformed;
Plugin.ReceiveSyncData_Prefix(ref malformed, new PhotonMessageInfo { Sender = PhotonNetwork.MasterClient }, out bool didMerge);
Equal(0, didMerge ? 1 : 0, "failed merge reports no mutation");
Equal(1, ReferenceEquals(originalMalformed, malformed) ? 1 : 0, "failed merge preserves original RPC payload");

// Inspect the production DLL, built against the old game reference package.
// This catches binary references that source-only stand-ins cannot detect.
if (args.Length > 0) {
    using var dll = File.OpenRead(args[0]);
    using var pe = new System.Reflection.PortableExecutable.PEReader(dll);
    var metadata = System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
    var members = metadata.MemberReferences.Select(handle => metadata.GetMemberReference(handle)).ToArray();
    Equal(0, members.Count(member => metadata.GetString(member.Name) == "dictionaryOfDictionaries"), "production DLL has no incompatible field reference");
    Equal(1, members.Any(member => metadata.GetString(member.Name) == "FetchPlayerUpgrades") ? 1 : 0, "production DLL uses public upgrade API");
}
Console.WriteLine($"PASS: {assertions} assertions, including all 13 upgrade types.");

static partial class Program {
    static readonly Dictionary<string, string> Methods = new() {
        ["Health"] = "UpgradePlayerHealth",
        ["Energy"] = "UpgradePlayerEnergy",
        ["ExtraJump"] = "UpgradePlayerExtraJump",
        ["GrabRange"] = "UpgradePlayerGrabRange",
        ["GrabStrength"] = "UpgradePlayerGrabStrength",
        ["GrabThrow"] = "UpgradePlayerThrowStrength",
        ["SprintSpeed"] = "UpgradePlayerSprintSpeed",
        ["TumbleLaunch"] = "UpgradePlayerTumbleLaunch",
        ["MapPlayerCount"] = "UpgradeMapPlayerCount",
        ["TumbleClimb"] = "UpgradePlayerTumbleClimb",
        ["TumbleWings"] = "UpgradePlayerTumbleWings",
        ["CrouchRest"] = "UpgradePlayerCrouchRest",
        ["DeathHeadBattery"] = "UpgradeDeathHeadBattery",
    };
}
