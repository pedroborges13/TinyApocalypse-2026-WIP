# Tiny Apocalypse (Work in Progress)
Tiny Apocalypse is a personal project currently under development. It is a 3D Top-Down Shooter with Tower Defense mechanics, where players must survive infinite waves of enemies by building defenses and upgrading their arsenal.

Project Started: Late January 2026.

README Last Updated: April 24, 2026.

<img width="400" height="170" alt="TinyClip2" src="https://github.com/user-attachments/assets/8f76c554-8c65-4567-9ec7-50cb5fcef456" />
<img width="400" height="170" alt="TinyClip1" src="https://github.com/user-attachments/assets/ac3a193c-f0b6-4d8e-b09b-0586e5043a5c" />

---
## 🎮 Game Overview
- Tech Stack: Unity, C#, Github
- Art: Pre-made assets from Quaternious bundle
- Platform: PC
---
## 🧠 Development Focus and Game Concept
The objective of this project was to develop a game with a larger scope and more challenging mechanics, while maintaining a robust architecture that is easy to scale and extend. I chose to implement important concepts that were new to my workflow or rarely used in previous projects, specifically focusing on Design Patterns, Object Pooling, State Machines, and ScriptableObjects.

### Game Concept
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
# Mainly Features
## Scriptable Objects
To optimize performance, and ensure a decoupled architecture, I utilized ScriptableObjects. This approach allows for a Data-Driven workflow, where gameplay parameters are separated from the logic, making the project scalable.
- [EntityStats](Assets/Scripts/ScriptableObjects/EntityStatsData.cs) - Acts as a template for all living entities (Player and Enemies). It centralizes attributes like Max HP, Move Speed, and Base Damage, enabling rapid difficulty tuning without modifying prefabs.
- [WeaponData](Assets/Scripts/ScriptableObjects/WeaponData.cs) - Defines unique behaviors for the arsenal, including damage, fire rate, knockback force, projectile speed, price, fire sound, magazine size, projetile behaviour and reload timings.
- [BuildingData](Assets/Scripts/ScriptableObjects/BuildingData.cs) - Handles the properties of various traps and barricades, such as construction costs, prefabs and preview prefabs.
- [SoundLibrary](Assets/Scripts/ScriptableObjects/SoundLibraryData.cs) - A central place to organize all game sounds. It uses a list of Enums to find and play specific sound effects quickly. When the game starts, it converts the list into a Dictionary. This makes finding sounds much faster, ensuring the game doesn't lag during combat.
- [SoundEffectData](Assets/Scripts/ScriptableObjects/SoundEffectData.cs) - Encapsulates individual audio logic, supporting multiple audio clips for the same action. It features built-in randomized pitch and volume modulation, ensuring that repetitive sounds (like weapon fire or zombie moans) remain organic and less fatiguing for the player.

## Object Pooling
To prevent memory spikes and garbage collection stutters during combat, I implemented a Object Pooling system. This system manages:
- Enemies: [WaveManager](Assets/Scripts/Managers/WaveManager.cs) and [PooledEnemy](Assets/Scripts/Pooled/PooledEnemy.cs)
- VFX: [VFXManager](Assets/Scripts/Managers/VFXManager.cs) and [PooledVFX](Assets/Scripts/Pooled/PooledVFX.cs)
- SFX: [SoundManager](Assets/Scripts/Managers/AudioManager.cs).
- Projectiles: [Weapon](Assets/Scripts/Weapon.cs) and [Projectile](Assets/Scripts/Projectile.cs)

## Enemy AI
The artificial intelligence was built upon Unity's NavMesh system, specifically optimized to handle large enemy hordes efficiently.
- State Machine Management: I implemented an Enum-based State Machine to govern enemy behaviors such as Chasing, Attacking, Knockback, and Dead. This approach ensures clean, readable code and prevents logical bugs during state transitions.
- Smart Pathfinding & Decision Making: Enemies actively detect if the direct path to the player is clear or obstructed. If the path is blocked, the AI evaluates whether to calculate an alternative route or attack the obstacle to clear the way.
- Recalculation Intervals: To maximize performance, pathfinding is not calculated every frame. Instead, I used fixed intervals with slight individual offsets for each unit, effectively reducing the "hive mind" effect where all enemies react simultaneously.
- Dynamic Behavioral Variety: Upon spawning, each enemy receives randomized parameter adjustments, including Speed, Acceleration, Stopping Distance, and Avoidance Priority. This creates more organic, less artificial movement patterns and significantly improves the overall Game Feel.

## Game Management & Dual State Machine
To ensure a scalable and bug-free game loop, I implemented a Game Manager utilizing a Dual State Machine architecture. This allows the game to independently track the player's progress and the application's current status.
- [GamePhase Machine](Assets/Scripts/Managers/GameManager.cs): Manages the core gameplay loop between Preparation and Combat. This separation ensures that building mechanics and enemy spawning logic only execute during their respective phases, preventing unintended interactions.
- [GameStatus Machine](Assets/Scripts/Managers/GameManager.cs): Handles the states, such as Playing, Paused, and GameOver. By decoupling this from the gameplay phases, the "Pause" functionality remains consistent regardless of whether the player is in the middle of a wave or building defenses.

## Override Animation 
The game uses 4 humanoid characters, all from the same asset pack (Quaternius). Because of this, I decided to use Animator Override Controllers, which simplifies the animation process by allowing a base controller to be reused across all characters.
- [Character Animation Controller](Assets/Scripts/CharacterAnimationController.cs)
---
## 🧩 Core Systems Implemented
### Infinite Wave System
- The waves were programmed to be progressively more difficult, increasing the number of enemies and the groups spawned during each round, along with improvements to enemy stats and control over the spawn chance for each type. [WaveManager](Assets/Scripts/Managers/WaveManager.cs)

### Building System
- [Building Manager](Assets/Scripts/Managers/BuildManager.cs) - The building system allows players to strategically place defenses during the Preparation Phase. It features a grid-based alignment system and real-time visual feedback.
- [Shop Manager](Assets/Scripts/Managers/ShopManager.cs) - Acts as the central hub for the game's economy, handling transactions for both weapons and structures. It ensures that players can only progress when they have sufficient resources.
- Grid-less placement system using raycasts on the ground. - The system uses Camera.main.ScreenPointToRay combined with Physics.Raycast to translate 2D mouse coordinates into 3D world space. I utilized LayerMasks, allowing the ray to focus only on valid buildable surfaces.
- Preview-based placement with validation logic. - Ensures that structures are perfectly aligned, which is crucial for building effective barricades and traps
- Designed to support future expansion (blocking paths, traps, NavMesh obstacles).

### Combat & Weapons
[Weapon](Assets/Scripts/Weapon.cs):
- Multiple weapon types implemented.
- Data-driven weapon configuration using ScriptableObjects.
- Designed to support inventory switching and grenade systems.
  
### Player Architecture
The player’s architecture was split into in some scripts to avoid accumulating too many responsibilities
- [PlayerController](Assets/Scripts/Player/PlayerController.cs) - Handles input, movement, and emits gameplay events
- [Wallet](Assets/Scripts/Player/PlayerWallet.cs) - Handles the player’s money and triggers events to notify any changes in the value.
- [Inventory](Assets/Scripts/Inventory.cs) - Handles weapon management, including adding, equipping, and unequipping weapons.

### Other Managers
I utilised Manager scripts to organise the core systems (Audio, Build, Game, Settings, Shop, UI, and VFX), converting them into Singletons when necessary. Below, I will cover only those that haven't been explained in other sections.
- [Audio Manager](Assets/Scripts/Managers/AudioManager.cs) - Handles all game audio through a centralised Singleton, ensuring sound continuity across scene transitions. The system utilises ScriptableObjects to define sound data. It automatically saves and loads player volume preferences using PlayerPrefs.
- [UI Manager](Assets/Scripts/Managers/UIManager.cs) - Handles the visual of the game. It was built using an Event-Driven architecture to ensure the interface remains decoupled from the core gameplay systems
- [Settings Manager](Assets/Scripts/Managers/SettingsManager.cs) - Handles hardware configurations, ensuring that user preferences for display and resolution are applied and saved across game sessions. I utilized the DontDestroyOnLoad to ensure the Settings Manager remains active across different scenes
- [VFX Manager](Assets/Scripts/Managers/VFXManager.cs) - VFX Manager is responsible for all particle effects and visual feedback in the game. To maintain high performance during intense combat with multiple simultaneous explosions.
---
## Future Implementation
The project is still in development. The next steps include:
- Additional mechanics: grenades, traps and towers.
- Visual polish (Game feel).
- Save system: JSON-based.
- A bit about UI implementation (Menu, wave's info).
- Further balancing of difficulty curves and economy.
---
## Note:
This repository is public to showcase my development process and architectural decisions.
Once the project reaches a more complete state, this README will be expanded.
