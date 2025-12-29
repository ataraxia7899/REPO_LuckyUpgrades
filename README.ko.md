<div align="center">

### 🌐 README Language : [English](README.md) | [한국어](README.ko.md)
<br>

# LuckyUpgrades

[![Language](https://img.shields.io/badge/Language-C%23-239120?logo=c-sharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Thunderstore Profile](https://img.shields.io/badge/THUNDERSTORE-PROFILE-blue?logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/ataraxia7899/)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/ataraxia7899/LuckyUpgrades?label=THUNDERSTORE&color=00AFEC&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/ataraxia7899/LuckyUpgrades/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/ataraxia7899/LuckyUpgrades?label=DOWNLOADS&color=00FF00&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/repo/p/ataraxia7899/LuckyUpgrades/)

[**R.E.P.O 업그레이드 공유 모드 (Thunderstore)**](https://thunderstore.io/c/repo/p/ataraxia7899/LuckyUpgrades/)

플레이어가 업그레이드 아이템을 획득하면 설정된 확률에 따라 **다른 모든 플레이어**도 동일한 업그레이드를 받습니다.

---
</div>

### 🛠 기술 스택

| 항목 | 설명 |
| :--- | :--- |
| **언어** | C# |
| **프레임워크** | .NET / BepInEx 5.4.x |
| **게임** | R.E.P.O. (Unity) |
| **라이브러리** | Harmony (패칭용) |

---

### ⚠️ 중요 안내

> **로비에 있는 모든 플레이어가 이 모드를 설치해야 정상적으로 작동합니다!**

---

### 🎬 간단 가이드

<div align="center">

![LuckyUpgrades 간단 가이드](https://github.com/user-attachments/assets/3a397b21-74c2-4944-a70a-9a4010520d2f)

</div>

---

### ✨ 주요 기능

* 🎲 **확률 기반 공유**: 각 업그레이드 유형별로 공유 확률 설정 가능
* ⚙️ **개별 업그레이드 설정**: 업그레이드마다 다른 확률 지정 가능
* 🔧 **13가지 업그레이드 지원**: 모든 플레이어 업그레이드 지원

---

### 📋 지원 업그레이드

| 업그레이드 | 설정 이름 | 기본값 |
| :--- | :--- | :--- |
| 체력 | `ChanceToActivatePlayerHealth` | 25% |
| 에너지 (스태미나) | `ChanceToActivatePlayerEnergy` | 25% |
| 달리기 속도 | `ChanceToActivatePlayerSprintSpeed` | 25% |
| 추가 점프 | `ChanceToActivatePlayerExtraJump` | 25% |
| 구르기 발사 | `ChanceToActivatePlayerTumbleLaunch` | 25% |
| 구르기 등반 | `ChanceToActivatePlayerTumbleClimb` | 25% |
| 구르기 날개 | `ChanceToActivatePlayerTumbleWings` | 25% |
| 웅크리기 휴식 | `ChanceToActivatePlayerCrouchRest` | 25% |
| 잡기 범위 | `ChanceToActivatePlayerGrabRange` | 25% |
| 잡기 힘 | `ChanceToActivatePlayerGrabStrength` | 25% |
| 던지기 힘 | `ChanceToActivatePlayerGrabThrow` | 25% |
| 지도 플레이어 수 | `ChanceToActivateMapPlayerCount` | 25% |
| 유령 배터리 | `ChanceToActivateDeathHeadBattery` | 25% |

---

### 📦 설치 방법

#### **Thunderstore 모드 매니저 (권장)**
1.  Thunderstore Mod Manager 설치
2.  **LuckyUpgrades** 검색 후 설치
3.  **로비의 모든 플레이어가 모드를 설치해야 합니다**

#### **수동 설치**
1.  BepInEx가 설치되어 있어야 합니다
2.  `LuckyUpgrades.dll`을 `BepInEx/plugins/` 폴더에 복사
3.  게임 실행
4.  **로비의 모든 플레이어와 모드를 공유하세요**

---

### ⚙️ 설정

게임을 실행하면 다음 경로에 설정 파일이 생성됩니다:
`BepInEx/config/LuckyUpgrades.cfg`

```ini
[Upgrades]

## 체력 업그레이드 공유 확률 (0-100%)
ChanceToActivatePlayerHealth = 25

## 에너지 업그레이드 공유 확률 (0-100%)
ChanceToActivatePlayerEnergy = 25

## ... (모든 업그레이드 기본값 25%)
```
