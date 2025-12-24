# LuckyUpgrades

**R.E.P.O Upgrade Sharing Mod**

When a player picks up an upgrade item, there is a configurable chance that **ALL other players** will also receive the same upgrade.

---

## ⚠️ Important Notice

> **All players in the lobby MUST have this mod installed for it to work correctly!**
> 
> This mod works by having each player's game detect when others pick up upgrades, then apply the shared upgrade locally.

---

## Features

- 🎲 **Probability-based sharing**: Configurable share chance for each upgrade type
- ⚙️ **Flexible settings**: Global or per-upgrade probability settings
-  **Waste chance**: Optional chance to waste upgrades (no one gets it)
- 🔧 **13 Upgrades supported**: All player upgrades are supported

---

## Supported Upgrades

| Upgrade | Config Name | Default |
|---------|-------------|---------|
| Health | ChanceToActivatePlayerHealth | 50% |
| Energy (Stamina) | ChanceToActivatePlayerEnergy | 25% |
| Sprint Speed | ChanceToActivatePlayerSprintSpeed | 25% |
| Extra Jump | ChanceToActivatePlayerExtraJump | 25% |
| Tumble Launch | ChanceToActivatePlayerTumbleLaunch | 25% |
| Tumble Climb | ChanceToActivatePlayerTumbleClimb | 25% |
| Tumble Wings | ChanceToActivatePlayerTumbleWings | 25% |
| Crouch Rest | ChanceToActivatePlayerCrouchRest | 25% |
| Grab Range | ChanceToActivatePlayerGrabRange | 25% |
| Grab Strength | ChanceToActivatePlayerGrabStrength | 25% |
| Grab Throw | ChanceToActivatePlayerGrabThrow | 25% |
| Map Player Count | ChanceToActivateMapPlayerCount | 25% |
| Death Head Battery | ChanceToActivateDeathHeadBattery | 25% |

---

## Installation

### Thunderstore Mod Manager (Recommended)
1. Install Thunderstore Mod Manager
2. Search for **LuckyUpgrades** and install
3. **Ensure all players in your lobby install the mod**

### Manual Installation
1. BepInEx must be installed
2. Copy `LuckyUpgrades.dll` to `BepInEx/plugins/` folder
3. Launch the game
4. **Share the mod with all players in your lobby**

---

## Configuration

After launching the game, a config file will be created at:
`BepInEx/config/LuckyUpgrades.cfg`

```ini
[Global]

## Enable or disable the mod
ModEnabled = true

## If true, the GlobalChanceToActivate will be used for all upgrades
UseOneChanceForAll = true

## % Chance to activate the upgrade for every player (0-100)
GlobalChanceToActivate = 25

## % Chance to waste the upgrade and activate it for nobody (0-100)
ChanceToWasteUpgrade = 0

[SpecificUpgrades]

## Individual upgrade chances (used when UseOneChanceForAll = false)
ChanceToActivatePlayerHealth = 50
ChanceToActivatePlayerEnergy = 25
# ... (other upgrades with default 25%)
```

---

## How It Works

1. Player A picks up an upgrade item
2. **All clients** detect this via Harmony patches on `PunManager.UpgradeXXX()`
3. Each client checks if the upgrade should be wasted (ChanceToWasteUpgrade)
4. Each client rolls against the configured chance
5. If successful, the client applies the upgrade to **themselves**

---

## Compatibility

- **R.E.P.O** v0.3.2+
- **BepInEx 5.4.x**
- **All players must have the mod installed**
- May conflict with other upgrade sync mods (e.g., SharedUpgrades)

---

## Troubleshooting

### ⚠️ Upgrading from v1.0.x

If you are upgrading from v1.0.x, **delete your old config file** and let the mod create a new one:
1. Delete `BepInEx/config/LuckyUpgrades.cfg`
2. Launch the game to generate a new config

This is required because the `HostOnly` setting has been removed in v1.1.0.

### Mod not working
1. Verify BepInEx is installed correctly
2. Check `BepInEx/LogOutput.log` for `[LuckyUpgrades]` entries
3. **Ensure ALL players have the mod installed**

### Upgrade not shared
- Check `ModEnabled = true` in config
- Ensure probability values are between 0-100
- If using `UseOneChanceForAll = false`, check individual upgrade settings
- **Make sure all players have the same mod version**

### Log Messages
Look for these log entries:
```
[LuckyUpgrades] ★ 다른 플레이어 업그레이드 감지! Health: [SteamID] +1
[LuckyUpgrades] 확률 체크: 25 < 100?
[LuckyUpgrades] ★★ 나에게 Health +1 공유 적용됨!
```

---

## Changelog

### v1.1.0
- **Breaking Change**: Removed `HostOnly` setting - ALL players must now install the mod
- Changed synchronization method: Each client applies upgrades locally when detecting others' upgrades
- Improved network compatibility
- Delete old config file after upgrading

### v1.0.5
- Changed patching method: Now patches `PunManager.UpgradeXXX()` directly
- Added 4 new upgrade types: TumbleClimb, TumbleWings, CrouchRest, DeathHeadBattery
- Improved error handling and null checks
- Fixed Random instance issue for consistent probability

---

## License

MIT License

---

## Credits

- Developed by: **ataraxia7899**
- Inspired by: SharedUpgrades mod
