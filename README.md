# LuckyUpgrades

[**R.E.P.O Upgrade Sharing Mod**](https://thunderstore.io/c/repo/p/ataraxia7899/LuckyUpgrades/)

When a player picks up an upgrade item, there is a configurable chance that **ALL other players** will also receive the same upgrade.

---

## ⚠️ Important Notice

> **All players in the lobby MUST have this mod installed for it to work correctly!**

---

## Features

- 🎲 **Probability-based sharing**: Configurable share chance for each upgrade type
- ⚙️ **Per-upgrade settings**: Set different chances for each upgrade
- 🔧 **13 Upgrades supported**: All player upgrades are supported

---

## Supported Upgrades

| Upgrade | Config Name | Default |
|---------|-------------|---------|
| Health | ChanceToActivatePlayerHealth | 25% |
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
[Upgrades]

## % Chance to share the Health upgrade (0-100)
ChanceToActivatePlayerHealth = 25

## % Chance to share the Energy upgrade (0-100)
ChanceToActivatePlayerEnergy = 25

## ... (all other upgrades with default 25%)
```

---

## How It Works

1. Player A picks up an upgrade item
2. **All clients** detect this via Harmony patches on `PunManager.UpgradeXXX()`
3. Each client rolls against the configured chance
4. If successful, the client applies the upgrade to **themselves**

---

## Compatibility

- **R.E.P.O** v0.3.2+
- **BepInEx 5.4.x**
- **All players must have the mod installed**
- May conflict with other upgrade sync mods

---

## Troubleshooting

### ⚠️ Upgrading from v1.0.x

**Delete your old config file** and let the mod create a new one:
1. Delete `BepInEx/config/LuckyUpgrades.cfg`
2. Launch the game to generate a new config

### Mod not working
1. Verify BepInEx is installed correctly
2. Check `BepInEx/LogOutput.log` for `[LuckyUpgrades]` entries
3. **Ensure ALL players have the mod installed**

### Upgrade not shared
- Ensure probability values are between 0-100
- **Make sure all players have the same mod version**

---

## Changelog

### v1.1.1
- **Bug Fix**: Shared upgrades now persist across level changes
- Added upgrade tracking system that reapplies shared upgrades after level transitions

### v1.1.0
- **Breaking Change**: ALL players must now install the mod
- Simplified config: Removed global settings, only per-upgrade chances remain
- Changed synchronization method: Each client applies upgrades locally

### v1.0.5
- Changed patching method: Now patches `PunManager.UpgradeXXX()` directly
- Added 4 new upgrade types

---

## License

MIT License

---

## Credits

- Developed by: **ataraxia7899**
- Contact: ataraxia7899@gmail.com (Please contact me if any issues arise.)
