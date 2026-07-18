# Another TD Game

A strategy-focused tower defense game developed in Unity. Players place and upgrade specialized defensive units, manage resources, and survive increasingly challenging enemy waves.

## Features

- Strategic tower placement and upgrades
- Multiple defensive and utility structures
- Wave-based enemy progression
- Configurable enemies, towers, terrain, and levels
- Resource management and gold-producing structures
- Drag-and-drop tower interaction
- Win, lose, health, and progress interfaces
- Performance-focused projectile and targeting systems

## Technical Highlights

A key development challenge was the cost of representing every projectile as an independent GameObject. The projectile system was redesigned to use lighter-weight visual elements where appropriate, reducing runtime overhead during larger waves. Target-selection logic was also adjusted to avoid unnecessary target switching and repeated range calculations.

## Technologies

- Unity
- C#
- Universal Render Pipeline
- ScriptableObjects
- Unity UI
- Prefab-based and data-driven architecture

## Running the Project

1. Clone the repository.
2. Open Unity Hub.
3. Select **Add project from disk**.
4. Open the cloned project folder.
5. Open the main menu scene and press Play.

## Author

**Cem Başar Ceylani**

- GitHub: [CemBC](https://github.com/CemBC)
- Portfolio: [cembc.github.io/CemBasarCeylani](https://cembc.github.io/CemBasarCeylani)
