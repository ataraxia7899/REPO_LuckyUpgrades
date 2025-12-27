<div align="center">

# LuckyUpgrades

[![Language](https://img.shields.io/badge/Language-C%23-239120?logo=c-sharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Thunderstore Profile](https://img.shields.io/badge/THUNDERSTORE-PROFILE-blue?logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/ataraxia7899/)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/ataraxia7899/LuckyUpgrades?label=THUNDERSTORE&color=00AFEC&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/ataraxia7899/LuckyUpgrades/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/ataraxia7899/LuckyUpgrades?label=DOWNLOADS&color=00FF00&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/ataraxia7899/LuckyUpgrades/)

[**R.E.P.O Upgrade Sharing Mod (Thunderstore)**](https://thunderstore.io/c/repo/p/ataraxia7899/LuckyUpgrades/)

When a player picks up an upgrade item, there is a configurable chance that **ALL other players** will also receive the same upgrade.

---
</div>

### 🛠 Tech Stack

| Item | Description |
| :--- | :--- |
| **Language** | C# |
| **Framework** | .NET / BepInEx 5.4.x |
| **Game** | R.E.P.O. (Unity) |
| **Library** | Harmony (for patching) |

---

### ⚠️ Important Notice

> **All players in the lobby MUST have this mod installed for it to work correctly!**

---

### ✨ Features

* 🎲 **Probability-based sharing**: Configurable share chance for each upgrade type
* ⚙️ **Per-upgrade settings**: Set different chances for each upgrade
* 🔧 **13 Upgrades supported**: All player upgrades are supported

---

### 📋 Supported Upgrades

| Upgrade | Config Name | Default |
| :--- | :--- | :--- |
| Health | `ChanceToActivatePlayerHealth` | 25% |
| Energy (Stamina) | `ChanceToActivatePlayerEnergy` | 25% |
| Sprint Speed | `ChanceToActivatePlayerSprintSpeed` | 25% |
| Extra Jump | `ChanceToActivatePlayerExtraJump` | 25% |
| Tumble Launch | `ChanceToActivatePlayerTumbleLaunch` | 25% |
| Tumble Climb | `ChanceToActivatePlayerTumbleClimb` | 25% |
| Tumble Wings | `ChanceToActivatePlayerTumbleWings` | 25% |
| Crouch Rest | `ChanceToActivatePlayerCrouchRest` | 25% |
| Grab Range | `ChanceToActivatePlayerGrabRange` | 25% |
| Grab Strength | `ChanceToActivatePlayerGrabStrength` | 25% |
| Grab Throw | `ChanceToActivatePlayerGrabThrow` | 25% |
| Map Player Count | `ChanceToActivateMapPlayerCount` | 25% |
| Death Head Battery | `ChanceToActivateDeathHeadBattery` | 25% |

---

### 📦 Installation

#### **Thunderstore Mod Manager (Recommended)**
1.  Install Thunderstore Mod Manager
2.  Search for **LuckyUpgrades** and install
3.  **Ensure all players in your lobby install the mod**

#### **Manual Installation**
1.  BepInEx must be installed
2.  Copy `LuckyUpgrades.dll` to `BepInEx/plugins/` folder
3.  Launch the game
4.  **Share the mod with all players in your lobby**

---

### ⚙️ Configuration

After launching the game, a config file will be created at:
`BepInEx/config/LuckyUpgrades.cfg`

```ini
[Upgrades]

## % Chance to share the Health upgrade (0-100)
ChanceToActivatePlayerHealth = 25

## % Chance to share the Energy upgrade (0-100)
ChanceToActivatePlayerEnergy = 25

## ... (all other upgrades with default 25%)
