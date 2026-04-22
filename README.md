# Tiny Apocalypse (Work in Progress)
This is a personal project currently under development. It is a 3D Top-Down Shooter with Tower Defense mechanics, where players must survive infinite waves of enemies by building defenses and upgrading their arsenal.

Project Started: Late January 2026
README Last Updated: February 5, 2026
---
## 🎮 Game Overview
- Tech Stack: Unity, C#, Github
- Art: Pre-made assets from Quaternious bundle
- Platform: PC

## 🧠 Development Focus
The objective of this project was to develop a game with a larger scope and more challenging mechanics, while maintaining a robust architecture that is easy to scale and extend.
- 

## Scritable Objects

- [EntityStats](Assets/Scripts/ScriptableObjects/EntityStatsData.cs) - Acts as a template for all living entities (Player and Enemies). It centralizes attributes like Max HP, Move Speed, and Base Damage, enabling rapid difficulty tuning without modifying prefabs.
- [WeaponData](Assets/Scripts/ScriptableObjects/WeaponData.cs) - Defines unique behaviors for the arsenal, including damage, fire rate, knockback force, projectile speed, price, fire sound, magazine size, projetile behaviour and reload timings.
- [BuildingData](Assets/Scripts/ScriptableObjects/BuildingData.cs) - Handles the properties of various traps and barricades, such as construction costs, prefabs and preview prefabs.
- [SoundLibrary](Assets/Scripts/ScriptableObjects/SoundLibraryData.cs) - A central place to organize all game sounds. It uses a list of Enums to find and play specific sound effects quickly. When the game starts, it converts the list into a Dictionary. This makes finding sounds much faster, ensuring the game doesn't lag during combat.
- [SoundEffectData](Assets/Scripts/ScriptableObjects/SoundEffectData.cs) - Encapsulates individual audio logic, supporting multiple audio clips for the same action. It features built-in randomized pitch and volume modulation, ensuring that repetitive sounds (like weapon fire or zombie moans) remain organic and less fatiguing for the player.

## Object Pooling
To prevent memory spikes and garbage collection stutters during combat, I implemented a Object Pooling system. This system manages:
- Enemies: [] [PooledEnemy](Assets/Scripts/Pooled/PooledEnemy.cs)
- VFX: [VFXManager](Assets/Scripts/Managers/VFXManager.cs), [PooledVFX](Assets/Scripts/Pooled/PooledVFX.cs)

## 🧩 Core Systems Implemented
### Infinite Wave System
- The waves were programmed to be progressively more difficult, increasing the number of enemies and the groups spawned during each round, along with improvements to enemy stats and control over the spawn chance for each type. [WaveManager](Assets/Scripts/Managers/WaveManager.cs)

### Player Architecture
- [PlayerController](Assets/Scripts/Player/PlayerController.cs) - Handles input, movement, and emits gameplay events
- [PlayerWallet](Assets/Scripts/Player/PlayerWallet.cs) - Handles the player’s money and triggers events to notify any changes in the value.
  
## Game Concept
The gameplay is divided into two main phases managed by a custom Game State system:
**Preparation Phase**
- The player can build towers, traps, and obstacles.
- Weapons and upgrades can be acquired.
- The player decides when to start the next wave (or waits for an auto-start timer).

**Combat Phase**
- Enemies spawn in infinite waves with increasing difficulty.
- Enemy stats and compositions scale over time.
- The goal is to survive as many waves as possible.
---
## Current Progress
This project was created with the goal of practicing gameplay architecture and system design, rather than focusing only on visual polish.
Key systems implemented so far:
### Game Manager
- Central authority using a State-based approach.
- Controls transitions between Preparation and Combat phases.
- Decoupled from UI and gameplay systems via events.
### Wave System
- Infinite wave progression.
- Enemy composition and difficulty scale dynamically over time.
- Supports grouped enemy spawns per wave instead of single-unit spawns.
### Building System
- Grid-less placement system using raycasts on the ground.
- Preview-based placement with validation logic.
- Designed to support future expansion (blocking paths, traps, NavMesh obstacles).
### Combat & Weapons
- Multiple weapon types implemented.
- Data-driven weapon configuration using ScriptableObjects.
- Designed to support inventory switching and future grenade systems.
### Assets
- Third-party assets used for characters and environments.
- Custom animation controllers shared between player and enemies using Animator Override Controllers.
---
## Future Implementation
The project is still a few weeks away from completion. The next steps include:
- UI/UX implementation (shop, HUD, menu, wave info).
- Additional mechanics (grenades, traps, boss enemy).
- Visual polish (VFX, game feel, audio).
- Save system (JSON-based) for progression.
- Further balancing of difficulty curves and economy.
---
## Note:
This repository is public to showcase my development process and architectural decisions.
Once the project reaches a more complete state, this README will be expanded with:
- Detailed system breakdowns.
- Code structure explanations.
- Design decisions.
