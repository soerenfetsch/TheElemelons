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

### 1.4 Definitions, Acronyms, and Abbreviations
- **Player**: The main character controlled by the user, capable of casting spells and navigating dungeons.
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

#### 2.1.1 Player Movement

- OnInputMove(): given a movement in x and/or y direction move the character along the given axes using the Player object's velocity
  - Velocity should be normalized so that diagonal movement is as fast and horizontal/vertical
  - Velocity should be calculated by a baseSpeed times a speedMultiplier which can be altered in other methods
  - Animation direction (front, back, left and right) reflects the direction the player is moving

- OnKnockbackMove(): given a direction a knockbackVelocity and a knockTime the player receives a knock in the given direction and with the given velocity and is unable to act within the given time frame
  - Receiving knockback triggers a coroutine in which the player is set to a state where they cannot act
  - A staggering animation is triggered to show that the knockback is received during the coroutine
  - Animation direction (front, back, left and right) does not reflect the direction the player is moving
 
#### 2.1.2 Spell Casting

- OnChargeSelected(): given a ChargeType enum object (fire, wind, water, earth) selects a new charge for the player if any of the two possible charge slots are empty
  - Instructs the UIManager to show the new charge sprite above the player
  - If both slots are occupied, nothing happens

- ClearCharges(): clears all selected charges of the player
  - Any selected charge is gone and both charge slots are empty again

- CastSpell(): Given the currently selected charges, determines the appropriate spell of the spell combination and then casts the spell in the direction the player is facing, given the player has enough mana to cast the spell
  - Direction of cast is determined by the mouse position given by the inputManager (direction from the character)
  - After casting all charges are cleared
  - If the player has sufficient mana and the spell is cast then the mana of the player is subtracted by the spell cost
  - Order of selected charges does not matter
 
  - With no charges selected: cast a basic attack spell that moves through the map during a small spell lifetime, has small damage value and only small knockback on enemies
  - With fire, fire selected casts a fireball that first charges up for a short while (< 1 second) and then flies into the given direction and has a longer spell lifetime, explodes on impact (wall, enemy)
 
#### 2.1.3 Input Manager

- OnKeyPressed: Input Manager uses its internal key to type mapping to trigger the appropriate key method in the update (see below) for the given game status (enum for 'ingame', 'main menu', 'settings menu', etc..)
- UpdateKey(): for a given event and new key, update the key to method binding of the 'ingame' methods
- GetAimDirection(): given the current mouse position calculates and returns the angle around the player
- Update(): uses the following methods to map the key inputs to the appropriate actions
  - UpdateMovementFromKeys(): Uses the internal movement keys for up, down, left, right to trigger the player movement in the given directions
  - UpdateMousePosition(): Updates the current mouse position
  - UpdateChargeSelection(): Uses the internal charge selection keys to call the SpellCasting.onChargeSelected() with the given charge if an appropriate key is pressed
  - UpdateCastButton(): If the cast button is pressed, instruct the SpellCasting object of the player to trigger the CastSpell() method
  - UpdateClearButton(): If the clear button is pressed, instruct the SpellCasting object of the player to trigger the ClearCharges() method

### 2.2 Dungeon Generation

#### 2.2.1 Dungeon Generator

- GeneratorDungeon(): Given a start position and using the selected TilemapVisualizer and MapGenerator generates the map data using the MapGenerator and then visualizes the map using the TilemapVisualizer

#### 2.2.2 Map Generator (abstract)

- GenerateMap(): Given a start point create all data necessary for the map data and return a MapData object. Should create floor, wall, deco, monster spawn point, door, shrine and trap positions as well as the different rooms
  - should call abstract methods for all the map positions which can later be implemented in the concrete map generators
  - positions should be saved in hash set objects
 
#### 2.2.3 Bipartitioning Map Generator

- CreateFloor(): Uses recursive bipartitioning to partition the given rectangular dungeon with its height and width into rectangular rooms with a minimum height and width as well as an offset between the rooms and then connect the rooms
  - The rooms are connected by choosing a random starting room and then connecting the middle point of that room to the middle point of the other room and the corridors have width with a given corridorWidth
  - The rooms are added to the map data in order of the connections
- CreateWalls(): Creates the wall positions by adding the outline of the floor to the wall data
  - Given a hash map of floor positions, the wall positions are the positions adjacent to the floor positions but not contained in them
- TO BE DEFINED: CreateMonsterSpawnPoints(): Randomly select positions on the floor on which monsters can spawn
- TO BE DEFINED: CreateDecorationPositions(): Randomly select positions on the floor and wall to place decorations (solid and not solid)

#### 2.2.4 Tilemap Visualization (abstract)

- PaintTiles(): abstract method to paint all tiles given a MapData object, which can be implemented by the concrete visualizers for the different dungeon types
- Clear(): abstract method to clear the tilemaps of the dungeon

#### 2.2.5 Earth Tilemap Visualizer

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

### 2.3 Enemy System

#### 2.3.1 Enemy (abstract)

- ChangeState(): abstract method to change the current state/objective of the Enemy
  - Uses a EnemyStatus enum which is defined for all enemies the same
- ActOnStatus(): abstract method to act in concretely implemented methods given the current status
- SubtractHealth(): method to subtract health of the internal Health object
- SubtractMana(): method to subtract mana of the internal Mana object
- CheckHealthStatus(): abstract method to act given the current health
- CheckManaStatus(): abstract method to act given the current mana

#### 2.3.2 Tree Log (Enemy)

- ActOnStatus():
  - while status 'idle': If player moves into chase radius then status is updated to 'walking'
  - while status 'walk': If player inside attack radius then status is updated to 'attacking'; If player outside attack radius but inside chase radius then Tree Log pathfinds toward the player; If player outside chase radius and Tree Log not at home position, then Tree Log moves toward home position; If player outside chase raidius and Tree Log at home position, updates status to 'idle'
  - while status 'attacking': Tree log stands still and attempts to engage combat with the player depending on the BoxCollider of both objects, if collision, combat is handles by the Combat Manager (TO BE DEFINED)
  - Animations should reflect the status and if 'walking' also the direction in which the Tree Log is headed
- ChangeState(): Given the new state, check if state is possible (possibleStates in Tree Log) and then change the state to the new state if the new state is possible for Tree Log
- CheckHealthStatus(): If health is zero, the Tree Log Dies and triggers the death animation

### 2.4 Combat System

#### TO BE DEFINED 2.4.1 Combat Manager: Manages combat damage, health and mana calculations

#### 2.4.2 Health

- SubtractHealth(): subtracts a floating input from the total health, if health drop below zero, health is set to zero
- AddHealth(): adds a floating input to the total health, if health gets higher than max health, health is set to max health
- GetHealth(): Returns the current total health value
- IsZero(): Returns true if the current health is zero, false otherwise

#### 2.4.3 Mana

- SubtractMana(): subtracts a floating input from the total mana, if mana drop below zero, mana is set to zero
- AddMana(): adds a floating input to the total mana, if mana gets higher than max mana, mana is set to max mana
- GetMana(): Returns the current total mana value
- IsZero(): Returns true if the current mana is zero, false otherwise

### TO BE DEFINED CONCRETELY 2.5 Game Progression and Save System
- The **GameManager** must control the overall game state, including dungeon transitions, combat phases, and player progression.
- The **GameSaveProvider** must support saving and loading game states, allowing players to resume their adventure seamlessly.
- Players should be able to save at specific checkpoints and have multiple save slots for different playthroughs.

### 2.6 UI System

#### 2.6.1 UI Manager

- **UIManager** should manage UI objects to display and change the UI elements to reflect the game state, should manage healthbar, manabar, spell charges, aiming direction and main menu widgets
- update health: call the HealthBar to continuously update the health ui using the player health
- show hurt: call the HealthBar to show the hurt animation
- update mana: call the ManaBar to continuously update the mana ui using the player mana
- show hurt: call the ManaBar to show the hurt animation
- update aiming arrow: using the input manager get the current mouse aiming direction of player and show an arrow around the player sprite in that direction
- clear spell charge UI: clear all spell charges from the canvas so that no charges are displayed
- load charge UI: given a charge sprite type, check how many charge UIs are displayed and then if none are, display the charge on top of the player at position 1, if one is displayed then show the charge at position 2, else do nothing

#### 2.6.2 HealthBar

- Update health UI using player health: display max health amount of (red) hearts of which player health are filled and rest are empty
- Execute hurt animation: If method is called this starts an animation where all hearts shortly light up white
- Execute heal animation: If method is called this starts an animation where all hearts shortly light up blue

#### 2.6.2 ManaBar

- Update mana UI using player mana: display max mana amount of (blue) stars of which player mana are filled and rest are empty
- Execute hurt animation: If method is called this starts an animation where all stars shortly light up white
- Execute heal animation: If method is called this starts an animation where all stars shortly light up violet

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
- **Class Diagrams**: The **class diagram** provides a high-level view of the relationships between core components like **Player**, **Enemy**, **MapGenerator**, and **GameManager**, created using **Lucidchart**.
