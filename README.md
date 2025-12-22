# LuckyUpgrades

**R.E.P.O Upgrade Sharing Mod**

When a player picks up an upgrade item, there is a configurable chance that other players will also receive the same upgrade.

---

## Features

- 🎲 **Probability-based sharing**: Configurable share chance for each upgrade type
- ⚙️ **Flexible settings**: Global or per-upgrade probability settings
- 🏠 **Host-only**: Only the host needs to install the mod
- 🎯 **Waste chance**: Optional chance to waste upgrades (no one gets it)

---

## Supported Upgrades

| Upgrade | Config Name | Default |
|---------|-------------|---------|
| Health | ChanceToActivatePlayerHealth | 50% |
| Energy (Stamina) | ChanceToActivatePlayerEnergy | 25% |
| Sprint Speed | ChanceToActivatePlayerSprintSpeed | 25% |
| Extra Jump | ChanceToActivatePlayerExtraJump | 25% |
| Tumble Launch | ChanceToActivatePlayerTumbleLaunch | 25% |
| Grab Range | ChanceToActivatePlayerGrabRange | 25% |
| Grab Strength | ChanceToActivatePlayerGrabStrength | 25% |
| Grab Throw | ChanceToActivatePlayerGrabThrow | 25% |
| Map Player Count | ChanceToActivateMapPlayerCount | 25% |

---

## Installation

### Thunderstore Mod Manager (Recommended)
1. Install Thunderstore Mod Manager
2. Search for LuckyUpgrades and install

### Manual Installation
1. BepInEx must be installed
2. Copy `LuckyUpgrades.dll` to `BepInEx/plugins/` folder
3. Launch the game

---

## Configuration

After launching the game, a config file will be created at `BepInEx/config/com.reposharemod.luckyupgrades.cfg`

```ini
[Global]

## Enable or disable the mod
ModEnabled = true

## Only the host will run the mod logic (recommended: true)
HostOnly = true

## If true, the GlobalChanceToActivate will be used for all upgrades
UseOneChanceForAll = true

## % Chance to activate the upgrade for every player
GlobalChanceToActivate = 25

## % Chance to waste the upgrade and activate it for nobody
ChanceToWasteUpgrade = 0

[SpecificUpgrades]

## % Chance to activate the Health upgrade for every player
ChanceToActivatePlayerHealth = 50

## % Chance to activate the Energy upgrade for every player
ChanceToActivatePlayerEnergy = 25

## ... (other upgrades)
```

---

## How It Works

1. Player A picks up an upgrade item
2. The mod checks if the upgrade should be wasted (ChanceToWasteUpgrade)
3. For each other player, the mod rolls against the configured chance
4. If successful, the upgrade is applied to that player as well

---

## Compatibility

- **R.E.P.O** latest version
- **BepInEx 5.4.x**
- May conflict with other upgrade sync mods

---

## Troubleshooting

### Mod not working
1. Verify BepInEx is installed correctly
2. Check `BepInEx/LogOutput.log` for errors
3. Ensure the host has the mod installed

### Probability not applied
- Check `ModEnabled = true` in config
- Ensure probability values are 0-100

---

## License

MIT License

---

## Credits

- Developed by: ataraxia7899
- Inspired by: SharedUpgrades mod
