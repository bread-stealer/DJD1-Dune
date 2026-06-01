# Dune

A 2D survival platformer set on the desert planet Arrakis. Play as Paul Atreides, navigate deadly sand dunes, avoid sandworms, fend off Harkonnen soldiers, and harvest spice to survive.

Built with **Unity 6.3 LTS** and **C#**.

🎮 [Play on Itch.io](https://breadstealer.itch.io/dune)  
📁 [GitHub Repository](https://github.com/bread-stealer/DJD1-Dune.git)

---

## How to Play

| Key | Action |
|-----|--------|
| A / D | Move left and right |
| W / Space | Jump |
| Space (tap) | Light Attack |
| Space (hold 1.5s) | Heavy Attack |
| E | Toggle shield on and off |
| Hold F | Harvest spice at a Spice Collector |
| Esc | Pause the game |

**Objective:** Locate the Spice Collector, hold F to harvest spice, then return to the Ship to complete the level. Watch your water supply — if it runs out, your health will begin to drain.

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Camera/
│   │   ├── CameraSystem.cs
│   │   └── CameraShake.cs
│   ├── Enemy/
│   │   ├── Enemy.cs
│   │   ├── EnemyStats.cs
│   │   ├── Harkonnen.cs
│   │   ├── Sandworm.cs
│   │   ├── SandwormManager.cs
│   │   └── WormHead.cs
│   ├── Player/
│   │   ├── AnimEventBridge.cs
│   │   ├── AttackData.cs
│   │   ├── PlayerAttack.cs
│   │   ├── PlayerAudio.cs
│   │   ├── PlayerController.cs
│   │   ├── PlayerHealth.cs
│   │   ├── PlayerHitFlash.cs
│   │   ├── PlayerShield.cs
│   │   └── PlayerWater.cs
│   ├── Audio/
│   │   ├── AudioManager.cs
│   │   └── SFXClip.cs
│   ├── UI/
│   │   ├── DamageNumber.cs
│   │   ├── DamageNumberSpawner.cs
│   │   ├── GameOverUI.cs
│   │   ├── HealthBarUI.cs
│   │   ├── MainMenuUI.cs
│   │   ├── ShieldUI.cs
│   │   ├── SpiceUI.cs
│   │   ├── WaterGaugeUI.cs
│   │   └── WinUI.cs
│   └── Gameplay/
│       ├── HitStop.cs
│       ├── SceneRef.cs
│       ├── SecretZone.cs
│       ├── ScrollGallery.cs
│       ├── SortingLayerAttribute.cs
│       ├── SpiceExtractor.cs
│       ├── SpiceManager.cs
│       ├── WaterCollectable.cs
│       └── WinDoor.cs
└── Editor/
    ├── SceneRefDrawer.cs
    └── SortingLayerDrawer.cs
```

---

## UML Diagram

```mermaid
classDiagram

%% ─── DATA STRUCTURES ───────────────────────────────────────────────
class AttackData {
    +float Damage
    +bool IsShieldPenetrating
    +AttackData(float, bool)
}

class SFXClip {
    +AudioClip clip
    +float volume
}

class EnemyStats {
    -float maxHealth
    -float moveSpeed
    -float damage
    -float attackRange
    -float attackCooldown
    -float detectionRange
    -LayerMask playerLayer
    +float MaxHealth
    +float MoveSpeed
    +float Damage
    +float AttackRange
    +float AttackCooldown
    +float DetectionRange
    +LayerMask PlayerLayer
}

%% ─── PLAYER ────────────────────────────────────────────────────────
class PlayerController {
    -float moveSpeed
    -float airControlFactor
    -float jumpForce
    -float jumpCutMultiplier
    +TakeDamage(AttackData) void
    -HandleMove() void
    -HandleJump() void
    -HandleJumpCut() void
}

class PlayerHealth {
    -float maxHealth
    +float CurrentHealth
    +float MaxHealth
    +bool IsDead
    +event OnHealthChanged
    +event OnDeath
    +TakeDamage(float) void
    +Heal(float) void
}

class PlayerShield {
    -float maxStamina
    -float staminaDrainRate
    -float immunityWindowDuration
    -float cooldownDuration
    +bool IsShieldActive
    +float CurrentStamina
    +bool IsOnCooldown
    +event OnShieldActivated
    +event OnShieldDeactivated
    +event OnShieldBroken
    +event OnShieldBlocked
    +event OnShieldRecharged
    +TryBlock(AttackData) bool
}

class PlayerAttack {
    -float lightDamage
    -float heavyDamage
    -float heavyAttackThreshold
    -void OnAttackHit()
}

class PlayerWater {
    -float maxWater
    -float drainRate
    -float dehydrationDamage
    +float CurrentWater
    +bool IsDehydrated
    +event OnWaterChanged
    +event OnDehydrated
    +event OnWaterRestored
    +Replenish(float) void
}

class PlayerAudio {
    -SFXClip attackClip
    -SFXClip hitClip
    -SFXClip jumpClip
    -SFXClip landClip
    -SFXClip shieldOnClip
    -SFXClip shieldOffClip
    -SFXClip shieldBlockClip
    -SFXClip shieldBrokenClip
    -SFXClip sandFootstepClip
    -SFXClip rockFootstepClip
    +PlayAttackSFX() void
    +PlayJumpSFX() void
    +PlayLandSFX() void
}

class PlayerHitFlash {
    -float flashDuration
    -Color flashColor
}

class AnimEventBridge {
    +event OnAttackHitEvent
    +event OnFootstepEvent
    +OnAttackHit() void
    +OnFootstep() void
}

%% ─── ENEMY ─────────────────────────────────────────────────────────
class Enemy {
    <<abstract>>
    #EnemyStats stats
    #float currentHealth
    #bool isDead
    +TakeDamage(float, bool) void
    #EvaluateState()* void
    #OnDamaged() void
    #OnDeath() void
    #FacePlayer(Transform) void
}

class Harkonnen {
    -float heavyDamageMultiplier
    -float wallCheckDistance
    -float ledgeCheckDistance
    -float flipCooldown
    #EvaluateState() void
    -Chase() void
    -Attack() void
    -Patrol() void
}

class Sandworm {
    -float warningDuration
    -float burstDuration
    -float disappearDelay
    +bool IsIdle
    +Trigger(Vector3) void
    #EvaluateState() void
    -EmergeSequence(Vector3) IEnumerator
}

class WormHead {
    -bool _canDamage
    +SetActive(bool) void
}

class SandwormManager {
    +static Instance
    -float timeOnSandToTrigger
    -float cooldownAfterEmerge
    -PlayerOnSand() bool
    -PlayerOnSafeGround() bool
    -TriggerWorm() void
}

%% ─── AUDIO ─────────────────────────────────────────────────────────
class AudioManager {
    +static Instance
    -AudioSource sfxSource
    -AudioSource musicSource
    -float musicVolume
    +PlaySFX(AudioClip, float, bool) void
    +PlayMusicWithFadeIn(AudioClip) void
    +FadeOutMusic() void
    +SetMusicVolume(float) void
    +SetSFXVolume(float) void
}

%% ─── CAMERA ────────────────────────────────────────────────────────
class CameraSystem {
    -float trapWidth
    -float trapUp
    -float trapDown
    -float lookaheadAmount
    -float followSpeed
    +SetShakeOffset(Vector3) void
}

class CameraShake {
    +StartShake(float, float) void
}

%% ─── GAMEPLAY ───────────────────────────────────────────────────────
class SpiceManager {
    +static Instance
    +int TotalSpice
    +event OnSpiceChanged
    +AddSpice(int) void
}

class SpiceExtractor {
    -int spiceAmount
    -float harvestDuration
    -KeyCode harvestKey
}

class WaterCollectable {
    -float waterAmount
}

class WinDoor {
    -string lockedMessage
    -string openMessage
}

class HitStop {
    +static Instance
    +Stop(float) void
}

class DamageNumberSpawner {
    +static Instance
    +Spawn(float, bool, Vector3) void
}

class DamageNumber {
    +Setup(float, bool) void
}

%% ─── UI ────────────────────────────────────────────────────────────
class HealthBarUI {
    -float smoothSpeed
    -float ghostDelay
}

class ShieldUI {
    -float smoothSpeed
    -float ghostDelay
    -float pulseSpeed
}

class WaterGaugeUI {
    -Color normalColor
    -Color dehydratedColor
}

class SpiceUI {
    -UpdateDisplay(int) void
}

class GameOverUI {
    +OnRestartPressed() void
    +OnMainMenuPressed() void
}

class WinUI {
    +static event OnWin
    +ShowWinScreen() void
    +OnNextLevelPressed() void
    +OnMainMenuPressed() void
}

%% ─── RELATIONSHIPS ──────────────────────────────────────────────────

%% Inheritance
Enemy <|-- Harkonnen
Enemy <|-- Sandworm

%% Composition
PlayerController *-- PlayerShield
PlayerController *-- PlayerHealth
PlayerController *-- PlayerWater
PlayerController *-- PlayerAttack
PlayerController *-- PlayerAudio
PlayerController *-- PlayerHitFlash
Sandworm *-- WormHead

%% Aggregation
PlayerAttack o-- AnimEventBridge
PlayerAudio o-- AnimEventBridge
Enemy o-- EnemyStats
SandwormManager o-- Sandworm
CameraShake o-- CameraSystem

%% Dependency
PlayerController ..> AttackData
PlayerController ..> CameraShake
PlayerController ..> HitStop
PlayerAttack ..> AttackData
PlayerAttack ..> Enemy
PlayerShield ..> AttackData
Harkonnen ..> AttackData
Harkonnen ..> PlayerController
WormHead ..> PlayerController
WormHead ..> PlayerHealth
SpiceExtractor ..> SpiceManager
WinDoor ..> SpiceManager
WinDoor ..> WinUI
WaterCollectable ..> PlayerWater
DamageNumberSpawner ..> DamageNumber
AudioManager ..> PlayerHealth
AudioManager ..> WinUI
HealthBarUI ..> PlayerHealth
ShieldUI ..> PlayerShield
WaterGaugeUI ..> PlayerWater
SpiceUI ..> SpiceManager
GameOverUI ..> PlayerHealth
PlayerAudio ..> AudioManager
PlayerAudio ..> SFXClip
```

---

## Credits

Pixabay. (2024). *Nature: Strong desert wind* [Sound effect]. Pixabay. https://pixabay.com/sound-effects/nature-strong-desert-wind-155416/

Bfxr. (2025). *Bfxr: Make sound effects for your games* [Sound effect generator]. https://www.bfxr.net/

Pixabay. (2024). *Nature: Sandstorm* [Sound effect]. Pixabay. https://pixabay.com/sound-effects/nature-sandstorm-222741/

Pixabay. (2025). *Amurich Atma: Mysterious duduk* [Music]. Pixabay. https://pixabay.com/pt/music/mundo-amurich-atma-mysterious-duduk-337300/

Pixabay. (2024). *Film special effects: Sand walk* [Sound effect]. Pixabay. https://pixabay.com/sound-effects/film-special-effects-sand-walk-106366/

Pixabay. (2024). *Walking on rocks 02* [Sound effect]. Pixabay. https://pixabay.com/pt/sound-effects/filme-e-efeitos-especiais-walking-on-rocks-02-55515/
