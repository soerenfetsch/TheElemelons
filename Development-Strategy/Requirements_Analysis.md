# Requirements Analysis Document for *The Elemelons*

## 1. Introduction

### 1.1 Purpose of the System
*The Elemelons* is a 2D dungeon crawler game where players explore elemental-themed dungeons, battle enemies, and retrieve magical artifacts called Elemelons. The goal is to restore balance to the kingdom by collecting all four Elemelons. The game emphasizes exploration, puzzle-solving, and tactical combat, providing players with an engaging fantasy experience.

### 1.2 Scope of the System
The game targets casual to midcore players who enjoy fantasy, dungeon crawlers, and pixel art. The target audience includes players aged 12 and above. *The Elemelons* will be developed for the PC platform using Unity version 2022.3.50f1.

### 1.3 Objectives
- Provide an immersive dungeon-crawling experience with a focus on elemental-themed environments.
- Implement a tactical combat system that emphasizes spell-casting and resource management.
- Develop procedurally generated dungeons to ensure replayability.
- Deliver a polished visual experience with 2D pixel art and fluid animations.
- Create engaging puzzles that require players to combine elemental abilities.

## 2. Functional Requirements

### 2.1 Gameplay Features

#### 2.1.1 Dungeon Generation
- The system must procedurally generate dungeons using **DungeonGenerator** and **MapGenerator** modules to ensure unique layouts each playthrough.
- Each dungeon must include floors, walls, and decorative elements managed by **TilemapVisualizer** and **TilemapManagement**.
- The **MapData** module must handle the data structure for dungeon layouts and ensure efficient loading and saving of map states.
- The dungeon generation process must follow a modular approach to allow easy adjustments for different themes and levels of difficulty.

#### 2.1.2 Combat System
- Players must be able to cast spells using different elemental types, which are managed by **SpellCasting** and **Mana** modules.
- The **CombatManager** module must manage interactions between **Player** and **Enemy** entities, including health deduction, status updates, and enemy attacks.
- The system must support multiple enemy types, each with unique attributes and behaviors to provide varied challenges.
- Players should receive visual and audio feedback for successful attacks, mana usage, and damage taken.

#### 2.1.3 Player Interaction
- The **InputManager** must handle player inputs, allowing character movement, combat, and interaction with environmental objects.
- The **UIManager** must present **HealthBar**, **ManaBar**, and other critical information to the player, ensuring they have a clear understanding of their status.
- Players must be able to interact with **NPCs** through a **DialogueSystem**, enabling quest progression and lore discovery.

#### 2.1.4 Game Progression and Save System
- The **GameManager** must control the overall game state, including dungeon transitions, combat phases, and player progression.
- The **GameSaveProvider** must support saving and loading game states, allowing players to resume their adventure seamlessly.
- Players should be able to save at specific checkpoints and have multiple save slots for different playthroughs.

### 2.2 User Stories
- **As a player**, I want to explore dungeons with elemental puzzles so that I can retrieve magical Elemelons and progress the story.
- **As a player**, I want to engage in strategic combat with enemies using elemental spells to overcome different challenges.
- **As a player**, I want to interact with NPCs to learn more about the lore and receive quests that guide my adventure.

## 3. Non-Functional Requirements

### 3.1 Performance
- The game must run at a consistent **60 FPS** on standard PC hardware with minimal frame drops, even when multiple enemies are present on-screen.
- **Combat latency** must be less than **100 ms** to ensure a responsive and immersive experience.
- Procedural dungeon generation must complete in under **5 seconds** to avoid delays during gameplay transitions.

### 3.2 Modularity and Maintainability
- The codebase must follow a **modular structure** where components like **DungeonGenerator**, **CombatManager**, and **UIManager** are decoupled, allowing for independent updates or replacements.
- Each core gameplay module must have **unit tests** to ensure functionality remains intact during code changes.
- **Branch naming conventions** must follow the format `feature-{featurename}-{developer}`, and commit messages should use **infinitive form** to maintain consistency.

### 3.3 Test-Driven Development
- Development must follow **test-driven design** principles, where unit tests are written before implementing core features.
- Manual **integration tests** must be conducted before merging feature branches, focusing on the interaction between major modules (e.g., **CombatManager** and **Health**).
- **Automated testing** should cover critical areas like procedural generation, combat interactions, and save/load functionality.

### 3.4 User Interface and Experience
- The **user interface** must be intuitive and accessible, with clear indicators for health, mana, and player objectives.
- **Visual effects** for spells, damage, and status changes must be designed to provide immediate feedback to the player, enhancing engagement.
- The game must support **keyboard and controller** inputs, allowing players to choose their preferred control method.

### 3.5 Resource Management
- **Memory usage** must be optimized to ensure stable performance, especially during dungeon generation and combat sequences.
- **Audio and visual assets** must be loaded dynamically to minimize initial load times and ensure smooth transitions between scenes.

### 3.6 Coding Standards
- The code must follow **consistent naming conventions**, with clear and descriptive variable and function names.
- **Comments** must be added to explain complex logic, particularly in procedural generation and combat algorithms.
- All **scripts** must be written to allow **unit testing**, ensuring that each function performs as expected independently.

## 4. Assumptions and Dependencies
- The game will be developed using **Unity version 2022.3.50f1**.
- The project will utilize **Pixelart** for creating 2D assets and animations for the visuals.
- The game will primarily target **Windows** operating systems.
- Development tools like **Git** for version control, **Trello** for task management, and **Lucidchart** for diagram creation will be used to ensure efficient project tracking and collaboration.

## 5. Glossary
- **Elemelons**: Magical artifacts representing elemental powers that players must collect.
- **DungeonGenerator**: A core module responsible for procedural dungeon layout generation.
- **CombatManager**: A module that handles combat interactions between players and enemies.
- **Mana**: A resource used by players to cast spells, managed by the Mana module.
- **NPC**: Non-playable characters that provide lore and quests to the player.
- **Procedural Generation**: The algorithmic creation of game content, such as dungeon layouts, to ensure a unique experience each playthrough.

## 6. Diagrams and Visual Aids
- **Sequence Diagrams**: Refer to the sequence diagrams for **dungeon generation** and **combat** to understand the workflow and interactions between modules, created using **Lucidchart**.
- **Class Diagrams**: The **class diagram** provides a high-level view of the relationships between core components like **Player**, **Enemy**, **MapGenerator**, and **GameManager**, created using **Lucidchart**.
