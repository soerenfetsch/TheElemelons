# Requirements Analysis Document for *The Elemelons*

## 1. Introduction

### 1.1 Purpose of the System
*The Elemelons* is a 2D dungeon crawler game where players explore elemental-themed dungeons, battle enemies, and retrieve magical artifacts called Elemelons. The goal is to restore balance to the kingdom by collecting all four Elemelons. The game emphasizes exploration, puzzle-solving, and tactical combat, providing players with an engaging fantasy experience.

### 1.2 Scope of the System
The Elemelons targets casual to midcore players aged 12 and above who enjoy fantasy settings, dungeon crawlers, and pixel art. The game is designed for single-player gameplay and will support keyboard and mouse input on PC platforms. Developed using Unity (version 2022.3.50f1), The Elemelons will feature replayable, procedurally generated dungeons, emphasizing both tactical combat and environmental puzzles. Additional expansions (such as new dungeons or elements) may be considered in future iterations.

### 1.3 Objectives
1. **Gameplay Experience**:
   - Delier an immersive dungeon-crawling experience with procedurally generated, elemental-themed dungeons.
   - Develop a tactical combat system emphasizing spell-casting, recource management, and elemental interplay.
2. **Replayability**:
   - Ensure high replay value through procedural generation and diverse puzzle mechanics.
3. **Vusual Quality**:
   - Deliver a polished visual experience with 2D pixel art and fluid animations.
4. **Engaging Gameplay**:
   - Create engaging puzzles that require players to combine elemental abilities.
   - Introduce challenging Boss battles that integrate elemental strengths and weaknesses.

### 1.4 Definitions, Acronyms, and Abbreviations
- **Player**: The main character controlled by the user, capable of casting spells, interacting with NPCs, and exploring dungeons.
- **Spell**: A magical ability created by combining elemental powers, used for combat or interacting with the environment.
- **Elements**: Basic building blocks for spells; includes Fire, Water, Earth, and Wind.
- **Enemy**: Non-playable characters that oppose the player, often with elemental strength.
- **Boss**: A powerful enemy encountered at the end of each dungeon, representing the dungeon's element and guarding an Elemelon.
- **Dungeons**: Procedurally generated environments where the player encounters enemies, solves puzzles, and collects items.
- **NPC (Non-Player Character)**: Characters like the King who interact with the player but are not controlled by the player.
- **Traps**:Environmental hazards that harm or hinder the player, such as spikes or fire pits.
- **Altar**: An object found in dungeons that restores the player's health when interacted with.
- **Health**: A resource representing the player's life, displayed as hearts in the HUD.

## 2. Functional Requirements

### 2.1 Player System

### 2.1.1 Player Movement

- The player character can move in four cardinal directions using keyboard inputs.

- **Movement Mechanics**:
  - Velocity will be normalized to ensure consistent speed during diagonal movement.
  - Speed is calculated using a baseSpeed multiplied by a speedMultiplier, adjustable by game states (e.g., buffs, debuffs).
- **Animation**:
  - The player's direction (front, back, left, right) will reflect movement visually.
  - When knocked back, animations will trigger to indicate the player's temporary loss of control.
- **Knockback System**:
  - Upon receiving knockback, the player is temporarily stunned, unable to act for a defined duration.
  - Knockback will trigger staggering animations.

### 2.1.2 Spell Casting

- **Charge Selection**:
  - Players can select up to two charges from the four elemental types (Fire, Wind, Water, Earth).
  - Selected charges are visually displayed above the player character.
- **Spell Casting**:
  - Combines selected charges into unique spells (e.g., Fire + Fire = Fireball).
  - Casting spells consumes mana. If mana is insufficient, the spell fails.
  - Spells interact with the environment (e.g., Fireball buruns wooden obstaclse).
- **Edge Cases**:
  - If no charges are selected, the player casts a basic attack.
  - Charge order does not affect spell type.
 
### 2.1.3 Input Manager

- Mps keyboard inputs to player actions, including movement, spell casting, and interacting with the environment.
- **Features**:
  - Key rebinding for movement, spell casting, and actions.
  - Updates mouse position to calculate spell direction.
  - Separate input states for gameplay and menus (e.g., ingame, main menu, settings).
 
### 2.1.3 Player Progression System

- **Elemental Proficiency**:
  - Unlock elemental-specific synergies:
    - Fire spells gain burn effects (damage over time).
    - Water spells create puddles for freezing or electricity combos.
    - Earth spells raise defensive walls for tactical retreats.
  - Upgrades expand tactial options but do not make the player "stronger".
- **Artifacts and Relics**:
  - Discover situational artifacts tied to the kingdom's lore, such as faster mana regeneration or immunity to specific traps.
  - Player can equip one artifact per dungeon per dungeon run, adding a layer of strategic planning.
- **Environmental Mastery**:
  - Wind currents to move platforms.
  - Earth spikes to seal traps.
  - Water bridges over lava.

### 2.2 Dungeon Generation

### 2.2.1 Dungeon Generator

- **Core Functionality**:
  - Generates dungeon layouts using procedural techniques.
  - Visualizes layouts with TilemapVisualizer.
- **Dungeon Features**:
  - Includes rooms, corridors, traps, shrines, and treasure chests.
  - Balances challenge by scaling enemy difficulty with dungeon depth.

### 2.2.2 Map Generator (abstract)

- Abstract class for procedural map creation.
- **Responsibilities**:
  - Create floor, wall, and decoration positions.
  - Define spawn points for enemies, treasures, and traps.
  - Return a MapData object with all positional information.
 
### 2.2.3 Bipartitioning Map Generator

- Uses recursive partitioning to create rectangular rooms and connect them with corridors.
- Ensures room spacing and dimensions adhere to predefined rules.

### 2.2.4 Tilemap Visualization (abstract)

- **PaintTiles()**: Paints floor, wall, and decoration tiles based on MapData.
- **Clear()**: Clears dungeon visuals when transitioning between dungeons.

### 2.2.5 Earth Tilemap Visualizer

- PaintTiles(): Paints floor, walls and decorations using the methods specified below
- PaintFloor(): Using an atlas tilemap with predefined floor patterns, place a rule tile for the rim tiles on the floor rims (adjacent to the walls) and map the atlas floor pattern to the rest of the floor
  - Starting point for the atlas should be seeded randomly in a way that the whole floor map fits into the selected atlas region
  - Rim of the floor should show the 12 possible rim tiles in the correct configuration (4 top corners, 4 bottom corners, 4 left, right, top, bottom straight rims)
- PaintWalls(): Using the wall positions places different wall tiles depending on the position in the room
  - Walls on top of the room are two positions wide and contain the two tile high wall tiles
  - Walls on the left or right side of the room are one tile wide and contain the upper view of the wall
  - Walls on the bottom of the room are one black tile wide
- TO BE DEFINED: DecorateDungeon(): ...
- Clear(): Clears the floor and wall tilemap

## 2.3 Enemy System

### 2.3.1 Enemy-(AI) (abstract)

- Enemies are driven by state-based AI:
  - **Idle**: Enemy patrols or remains stationary.
  - **Chase**: Enemy moves towards the player when within detection radius.
  - **Attack**: Enemy executes an attack sequence if within attack range.
  - **Retreat**: Enemy retreats when health is low.
- **Scaling**:
  - Enemy stats (health, damage, AI complexity) scale with dungeon depth.

#### 2.3.1.1 Tree Log (Enemy)

- ActOnStatus():
  - while status 'idle': If player moves into chase radius then status is updated to 'walking'
  - while status 'walk': If player inside attack radius then status is updated to 'attacking'; If player outside attack radius but inside chase radius then Tree Log pathfinds toward the player; If player outside chase radius and Tree Log not at home position, then Tree Log moves toward home position; If player outside chase raidius and Tree Log at home position, updates status to 'idle'
  - while status 'attacking': Tree log stands still and attempts to engage combat with the player depending on the BoxCollider of both objects, if collision, combat is handles by the Combat Manager (TO BE DEFINED)
  - Animations should reflect the status and if 'walking' also the direction in which the Tree Log is headed
- ChangeState(): Given the new state, check if state is possible (possibleStates in Tree Log) and then change the state to the new state if the new state is possible for Tree Log
- CheckHealthStatus(): If health is zero, the Tree Log Dies and triggers the death animation

### 2.3.2 Boss Mechanics

- Bosses represent dungeon themes and challenge the player with unique abilites.
- Each boss guards an Elemelon artifact.
- Boss fights include multiple phases to increase difficulty.

## 2.4 Combat System

### 2.4.1 Combat Manager

- Handles player-enemy interactions, including damage calculations and health/mana updates.
- Supports environmental interactions, like traps damaging enemies and players.

### 2.4.2 Health and Mana Systems

- **Health**:
  - Reduces when damage is received. If health reaches zero, the player dies.
  - Restores via health pickups or interacting with altars.
 - **Mana**:
   - Depletes with each spell cast.
   - Restores gradually over time or through mana pickups.

#### TO BE DEFINED CONCRETELY 2.5 Game Progression and Save System (might need to remove this!)
- The **GameManager** must control the overall game state, including dungeon transitions, combat phases, and player progression.
- The **GameSaveProvider** must support saving and loading game states, allowing players to resume their adventure seamlessly.
- Players should be able to save at specific checkpoints and have multiple save slots for different playthroughs.


### 2.5 Reward System

- **Lore Fragments**: Discover scrolls or murals revealing the kingdom's history and the Elemelons' origin.
- **Elemental Keys**: Open hidden areas with additional puzzles or challenges.
- **Unique Spells**: Reward players with situational spells for solving puzzles or combat scenarios (e.g., time-freezing or stealth spells)
- **Puzzle Fragments**: Collect pieces leading to a mega-puzzle unlocking secret content.

### 2.6 Metrics and Feedback Loops

- **Scoreboard**:
  - Track dungeon completion time, enemy defeats, and puzzles solved.
  - Display combo multipliers for consecutive kills or puzzle-solving streaks.
- **Player Dashboard**:
  - Visualize player progress, such as collected Elemelons, discovered relics, and completed dungeons.
- **Post-Dungeon Insights**:
  - Provide performance summaries after each dungeon (e.g., most-used spells, damage dealt, damage received).

## 3. Non-Functional Requirements

### 3.1 Performance
- The game must run at a consistent **60 FPS** on standard PC hardware with minimal frame drops, even when multiple enemies are present on-screen.
- **Combat latency** must be less than **100 ms** to ensure a responsive and immersive experience.
- Procedural dungeon generation must complete in under **5 seconds** to avoid delays during gameplay transitions.

### 3.2 Modularity and Maintainability
- The codebase must follow a **modular structure** where components like **DungeonGenerator**, **CombatManager**, and **UIManager** are decoupled, allowing for independent updates or replacements.
- Each core gameplay module must have **unit tests** to ensure functionality remains intact during code changes.
- **Branch naming conventions** must follow the format `feature-{featurename}-{developer}`, and commit messages should use **infinitive form** to maintain consistency.
- To merge changes coming from a feature branch, every developer has to create a merge request for the given feature into the main branch

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
- For our code we follow the standard C# coding guidelines for .NET development (https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- The code must follow **consistent naming conventions**, with clear and descriptive variable and function names.
- We use Pascal Case for the naming of our methods and class and Camel Case for our variables
- **Comments** must be added to explain complex logic, particularly in procedural generation and combat algorithms.
- All **scripts** must be written to allow **unit testing**, ensuring that each function performs as expected independently.
- We use the C# default formatter for the C# VSCode extension, this formatter has to be used on every document we commit

## 4. Assumptions and Dependencies
- The game will be developed using **Unity version 2022.3.50f1**.
- The project will utilize **Pixelart** for creating 2D assets and animations for the visuals.
- The game will primarily target **Windows** operating systems.
- Development tools like **Git** for version control, **Trello** for task management, and **Lucidchart** for diagram creation will be used to ensure efficient project tracking and collaboration.

## 5. Scenarios

1. **Starting the Game and Selecting New Game**
2. **Receiving the Mission from the King**
3. **Learning Controls and Spell-Casting**
4. **First Encounter with Obstacles**
5. **Entering an Elemental Dungeon**
6. **Navigating Dungeon Traps and Hazards**
7. **Combat with Elemental Enemies**
8. **Elemental Dungeon Boss Battle**
9. **Collecting the Elemental Elemelon and Returning to the King**
10. **Exploring Other Elemental Dungeons**
11. **Final Retrieval of All Four Elemelons**
12. **Entering the Final Battle**

## 6. Use Case Model

1. **Starting a New Game**: Player selects "New Game" from the main menu.
2. **Basic Controls and Spell-Casting Tutorial**: Player learns movement, interaction, and spell-casting.
3. **Clearing Obstacles with Spells**: Player uses spells to remove obstacles.
4. **Accessing Menus**: Player can access settings, spell information, and HUD.

### 6.1 Object Model

- **Player**: Health, Mana, Inventory, Position.
- **Spell**: Element1, Element2, ManaCost, Effect.
- **Enemy**: Health, Type, Position.
- **Boss**: Health, Type, SpecialAbilities.
- **Dungeon**: Theme, Layout, Enemies, Traps.
- **King (NPC)**: Position, Dialogue.

### 6.2 Dynamic Models

1. **Spell-Casting Sequence**: Selecting elements, casting, and applying effects.
2. **Dungeon Exploration and Enemy Encounter**: Navigating, encountering, and battling enemies.
3. **Boss Battle Sequence**: Defeating the dungeon boss and obtaining the Elemelon.
4. **Returning Elemelons to the King**: Player returns Elemelon, receives a reward, and is prepared for further exploration.

### 6.3 User Interface - Navigational Paths and Screen Mock-Ups

- **Main Menu**: Continue, New Game, Settings, End Game.
- **In-Game HUD**: Displays health, mana, and elemental slots.
- **Pause Menu**: Access to settings, grimoire, and quit.
- **Settings Menu**: Adjust sound, controls, and visual effects.
- **Grimoire (Spellbook)**: Review spell combinations and effects.
- **Interaction with King**: Dialogue with king when returning Elemelons.

## 7. Glossary
- **Elemelons**: Magical artifacts representing elemental powers that players must collect.
- **DungeonGenerator**: A core module responsible for procedural dungeon layout generation.
- **CombatManager**: A module that handles combat interactions between players and enemies.
- **Mana**: A resource used by players to cast spells, managed by the Mana module.
- **NPC**: Non-playable characters that provide lore and quests to the player.
- **Procedural Generation**: The algorithmic creation of game content, such as dungeon layouts, to ensure a unique experience each playthrough.

## 8. Diagrams and Visual Aids
- **Sequence Diagrams**: Refer to the sequence diagrams for **dungeon generation** and **combat** to understand the workflow and interactions between modules, created using **Lucidchart**.
- **Class Diagrams**: The **class diagram** provides a high-level view of the relationships between core components like **Player**, **Enemy**, **MapGenerator**, and **GameManager**, created using **Lucidchart** and **draw.io**.
