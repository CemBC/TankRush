# Another TD Game

A strategy-focused tower defense game developed in Unity. Players place and upgrade specialized defensive units, manage their resources, and survive increasingly challenging enemy waves.

## About the Game

Another TD Game is a tower defense project built around strategic unit placement and progression. Different defensive structures serve different combat roles, encouraging players to adapt their strategy according to enemy types and level layouts.

The project also focuses on gameplay performance. Projectile behavior and target-selection systems were optimized to reduce unnecessary runtime calculations and improve performance during larger enemy waves.

## Features

- Strategic tower placement
- Multiple tower types and upgrade levels
- Wave-based enemy progression
- Different enemy types with configurable attributes
- Resource and economy management
- Gold-producing structures
- Drag-and-drop tower interaction
- Data-driven level, enemy, terrain, and tower configuration
- Main menu and level progression
- Health, progress, win, and lose interfaces
- Performance-optimized projectile systems
- Optimized target-selection mechanics

## Tower Types

The game includes several defensive and utility structures, such as:

- Cannons
- Catapults
- Turrets
- Gold Mines

Each structure has its own role, statistics, and upgrade progression.

## Technical Highlights

One of the main development challenges was projectile performance. Creating every projectile as an independent GameObject caused performance problems during larger waves.

To address this, projectile representations were converted into lightweight UI-based elements where appropriate. Target-selection behavior was also improved to avoid repeatedly switching targets and performing unnecessary range calculations.

## Technologies

- Unity
- C#
- Universal Render Pipeline
- Unity UI
- ScriptableObjects
- Prefab-based architecture
- Data-driven level design

## Project Structure

```text
Assets/
├── Datas/
│   ├── EnemyToken/
│   ├── TowerDatas/
│   ├── TerrainToken/
│   └── LevelDatas/
├── InternalAssets/
├── ExternalAssets/
└── Scripts/s