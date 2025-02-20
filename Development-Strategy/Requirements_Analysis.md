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

#### 2.1.1 Player

- **Get knocked**: given a direction a knockTime (float between 0 and 10 seconds) the player is "knocked" and unable to act within the given time frame
  - Receiving knockback triggers a coroutine in which the player is set to a state where they cannot act
  - A staggering animation is triggered to show that the knockback is received during the coroutine
  - Animation direction (front, back, left and right) does not reflect the direction the player is moving

#### 2.1.2 Player Movement

- **Move on input**: given a two-dimensional movement vector in any direction, move the character along the direction using the Player object's velocity times a unity delta time (Time.fixedDeltaTime)
  - Movement direction should be normalized so that diagonal movement is as fast as horizontal/vertical
  - Velocity should be calculated by a baseSpeed times a speedMultiplier both of which can be altered in other methods (changeable attributes)
  - Animation direction (front, back, left and right) reflects the direction the player is moving (character faces left when moving left etc.)
 
#### 2.1.3 Spell Casting

- **Select charge**: given a ChargeType enum object (fire, wind, water, earth) selects a new charge for the player if any of two possible charge slots are empty
  - For the player two open slots for charges are given, which can each be occupied by either ChargyType enum
  - The slots are both initialized as empty
  - Upon charge selection, if either of the slots is empty, the first empty slot is filled with the charge
  - If both slots are occupied, nothing happens
  - If a slot was filled with the charge, instructs the UI Manager to show a charge sprite at the occupied slot above the player head (left if first slot, right if second slot), which represents the charge (Fire symbol for fire charge etc.)

- **Clear charges**: clears all charges of the player
  - All selected charges are cleared and both charge slots are empty again
  - Instructs the UI Manager to remove all shown charge sprites (no sprites shown on top of the player's head)

- **Cast spell**: Given the player's charge selection cast a corresponding spell.
  - Given a combination from 0 to 2 charges, determine the spell corresponding to the charge combination (e.g Charge1 Fire, Charge2 Earth --> Bomb Spell). The corresponding spells are shown below.
  - If the player's current mana amount (float) is greater or equal to the spells mana cost (float) the spell is cast and the mana of the player is subtracted by the spell cost.
  - Then cast the spell at the Casting Point of the player (given by the aim arrow defined in the UI Manager) in the direction from the player position to the user's mouse position.
  - Direction of cast is determined through the mouse position in a method given by the Input Manager
  - After casting all charges are cleared
  - Order of selected charges does not matter
  - Use the combat manager to instantiate the spell game object
 
  - With no charges selected: cast a basic attack spell that is stationary at the casting point. It lingers at the point for a small time (variable to be changed ~ 1 sec) and if it collides with an enemy, deals a small amount of damage (variable to be changed ~ 0.5 hearts)
  - With fire, fire selected: casts a fireball that first charges up for a short while (< 1 second) and then flies into the given direction and has a longer spell lifetime (variable to be changed ~ 3 sec), explodes on impact (wall, enemy) and deals large amount of damage (variable to be changed ~ 2 hearts) in an AOE where the radius of effect is an edittable variable ~ 1.5 tiles
  - With wind, none selected: casts a speed buff spell that boosts the movement speed of the player by a factor (variable between 1 and 2) for a variable amount of time (3-30 sec). Should delegate the buff creation to the Buff/Debuff Manager which in return delegates the icon creation and display to the UI Manager. The Buff/Debuff Manager should start the speed buff animation as long as the buff is applied (see Buff/Debuff Manager for details)
 
#### 2.1.4 Input Manager

- **Act on key pressed**: Input Manager uses its internal key to type mapping to trigger the appropriate key method in the update (see below) for the given game status (enum for 'ingame', 'main menu', 'settings menu', etc..)
- **Update key mapping**: for a given event and new key, update the key to method binding of the 'ingame' methods
- **Get aim direction**: given the current mouse position calculates and returns the angle around the player for a given pivot position on the player (variable, can be changed)
- **Update**: uses the following functionalities to map the key inputs to the appropriate actions
  - **Update movement from keys**: Uses the internal movement keys for up, down, left, right to trigger the player movement in the given directions
  - **Update mouse position**: Updates the current mouse position
  - **Update charge selection**: Uses the internal charge selection keys to call the **Select charge** of Spell Casting with the given charge if an appropriate key is pressed
  - **Update cast**: If the cast button is pressed, instruct the SpellCasting object of the player to trigger the **Cast spell** functionality
  - **Update clear charges**: If the clear button is pressed, instruct the SpellCasting object of the player to trigger the **Clear charges** functionality
  - **Update interaction**: Uses the internal interact key to call the **Interact with closest interactable** method of the Interaction System of the player

### 2.2 Dungeon Generation

#### 2.2.1 Dungeon Generator

- **Generate Dungeon**: Given a start position and using the selected TilemapVisualizer and MapGenerator generates the map data using the MapGenerator and then visualizes the map using the TilemapVisualizer

#### 2.2.2 Map Generator (abstract)

- **Generate Map**: Given a start point create all data necessary for the map data and return a MapData object. Should create floor, wall, deco, monster spawn point, door, shrine and trap positions as well as the different rooms
  - should call abstract methods for all the map positions which can later be implemented in the concrete map generators
  - positions should be saved in hash set objects
 
#### 2.2.3 Bipartitioning Map Generator

- **Create Floor**: Use recursive bipartitioning to partition the given rectangular dungeon with its height and width into rectangular rooms with a minimum height and width as well as an offset between the rooms and then connect the rooms
  - The rooms are connected by choosing a random starting room and then connecting the middle point of that room to the middle point of the other room and the corridors have width with a given corridorWidth
  - The rooms are added to the map data in order of the connections
- **Create walls**: Creates the wall positions by adding the outline of the floor to the wall data
  - Given a hash map of floor positions, the wall positions are the positions adjacent to the floor positions but not contained in them
- **Create doors**: Creates the door positions between rooms and corridors
  - Given room positions and corridor positions the door position is in the middle of the corridor adjacent to a room
- TO BE DEFINED: **Create monster spawnpoints**: Randomly select positions on the floor on which monsters can spawn
- TO BE DEFINED: **Create decoration positions**: Randomly select positions on the floor and wall to place decorations (solid and not solid)
- TO BE DEFINED: **Create shrine positions**: Randomly select positions on the floor to place shrines

#### 2.2.4 Tilemap Visualization (abstract)

- **Paint tiles**: abstract method to paint all tiles given a MapData object, which can be implemented by the concrete visualizers for the different dungeon types
- **Clear**: abstract method to clear the tilemaps of the dungeon

#### 2.2.5 Earth Tilemap Visualizer

- **Paint tiles**: Paints floor, walls and decorations using the methods specified below
- **Paint floor**: Using an atlas tilemap with predefined floor patterns, place a rule tile for the rim tiles on the floor rims (adjacent to the walls) and map the atlas floor pattern to the rest of the floor
  - Starting point for the atlas should be seeded randomly in a way that the whole floor map fits into the selected atlas region
  - Rim of the floor should show the 12 possible rim tiles in the correct configuration (4 top corners, 4 bottom corners, 4 left, right, top, bottom straight rims)
- **Paint walls**: Using the wall positions places different wall tiles depending on the position in the room
  - Walls on top of the room are two positions wide and contain the two tile high wall tiles
  - Walls on the left or right side of the room are one tile wide and contain the upper view of the wall
  - Walls on the bottom of the room are one black tile wide
- TO BE DEFINED: **Decorate dungeon**: ...
- **Clear**: Clears the floor and wall tilemap

### 2.3 Enemy System

#### 2.3.1 Enemy (abstract)

- **Change state**: abstract method to change the current state/objective of the Enemy
  - Uses a EnemyStatus enum which is defined for all enemies the same
- **Act on status**: abstract method to act in concretely implemented methods given the current status
- **Subtract Health**: method to subtract health of the internal Health object
- **Subtract Mana**: method to subtract mana of the internal Mana object
- **Check health status**: abstract method to act given the current health
- **Check mana status**: abstract method to act given the current mana
- **Get Enemy Element Type**: each enemy is associated with a elemental type, which should be returned here

#### 2.3.2 Tree Log (Enemy)

- **Act on status**:
  - while status 'idle': If player moves into chase radius then status is updated to 'walking'
  - while status 'walk': If player inside attack radius then status is updated to 'attacking'; If player outside attack radius but inside chase radius then Tree Log pathfinds toward the player; If player outside chase radius and Tree Log not at home position, then Tree Log moves toward home position; If player outside chase raidius and Tree Log at home position, updates status to 'idle'
  - while status 'attacking': Tree log stands still and attempts to engage combat with the player depending on the BoxCollider of both objects, if collision, combat is handles by the Combat Manager (TO BE DEFINED)
  - Animations should reflect the status and if 'walking' also the direction in which the Tree Log is headed
- **Change state**: Given the new state, check if state is possible (possibleStates in Tree Log) and then change the state to the new state if the new state is possible for Tree Log
- **Check health status**: If health is zero, the Tree Log Dies and triggers the death animation
- **Get Enemy Element Type**: Tree Log should be of element type EARTH

#### 2.3.3 Enemy Path Finding
- should be able to construct path to player by taking into consideration the collision objects (first walls)
- implementation should be seemless so that immediate direction where the enemy should walk to approach player should be returned/noted down
- should be reusable as module for all enemies

### 2.4 Combat System

#### 2.4.1 Combat Manager: Manages combat damage, health and mana calculations
- **Cast spells**: Given a spell type with predefined attributes (i.e position, rotation) spawn the given spell object
- **Add buff or debuff**: For a given object (e.g the player or an enemy), use the Manager to apply the given buff/debuff
- **Calculate Damage**: Using the spell/attack type and the type of the opponent the attack/spell hits, calculates the damage using the formula base_attack_damage * damage_factor(of attacker) * defense_factor(of defender) * attack_type_effectivity_modifier, the effectivity modifier is given as (attack type first - defender type second): Any - BASIC = 1, FIRE - EARTH = 1.25, EARTH - AIR = 1.25, AIR - WATER = 1.25, WATER - FIRE = 1.25, the modifiers should be specified in a dictionary so that they can be changed later.
- **Execute attack**: callback function for all attacks which are called by the participating objects, given the caller object (attacker) and the hit object (defender) calculate the attack damage, subtract the damage of the defender's health, set defender to status staggered and knock back the defender, if the defender is the player and the player dies, instruct the Game System to act on player death

#### 2.4.2 Buff/Debuff Manager: Manages buffs and debuffs for the player and enmies alike
- **Apply Buff or Debuff**: Start a timer of given length to apply the buff or debuff during the timer to any buffable object (Enemy or Player) and execute the effects. Then if the target of the buff/debuff is the player, delegate the UI Manager to create the buff/debuff effect in the UI, buffs and debuffs should be additive so they can stack on the object (handle a list of them)
- The different buffs are:
    - Speed buff: Player or Enemy have increased movement speed during the application
    - Damage buff: Player or Enemy have increased damage (using a damage factor)
    - Defense buff: Player or Enemy have increased defense (using a defense factor)
    - Mana regen buff: Player of Enemy have increased mana regen
 - The different debuffs are:
    - Slowness debuff: Player or Enemy have decreased movement speed during the application
    - Weakness debuff: Player or Enemy have decreased damage (using a damage factor)
    - Fragility debuff: Player or Enemy have decreased defense (using a defense factor)
    - Mana regen debuff: Player of Enemy have decreased mana regen
  - Each buff has an associated duration and an effect factor, for the duration of the effect the respective attribute (movement speed, damage, etc.) is multiplied (buff) or divided (debuff) by the given factor (factors stack, i.e if two buffs with factor 2 are applied, the resulting factor is 4)
  - **Speed Buff** additional animations: If the speed buff is applied to the player, the UI Manager should continuously update the speed buff animation which shows dust particles in the opposite direction of the movement during the duration of the speed buff (see update and start speed buff animation in the UI Manager)

#### 2.4.3 Health

- **Subtract Health**: subtracts a floating input from the total health, if health drop below zero, health is set to zero
- **Add health**: adds a floating input to the total health, if health gets higher than max health, health is set to max health
- **Get health**: Returns the current total health value
- **Is zero**: Returns true if the current health is zero, false otherwise

#### 2.4.4 Mana

- **Subtract mana**: subtracts a floating input from the total mana, if mana drop below zero, mana is set to zero
- **Add mana**: adds a floating input to the total mana, if mana gets higher than max mana, mana is set to max mana
- **Get mana**: Returns the current total mana value
- **Is zero**: Returns true if the current mana is zero, false otherwise

### 2.5 Game Progression and Save System

#### 2.5.1 The **GameManager** must control the overall game state, including dungeon transitions, combat phases, and player progression

- **Manage game progression**: save where the player is in the game progression (which dungeons/levels) and which dialogs and boss fights were successfully finished and allow the access to the current game progression level, saving should be handled by the GameSaveProvider
- **Manage game status**: Game status should be saved as game status (enum for 'ingame', 'main menu', 'settings menu', etc..) and be able to switch between them through actions
- **On go to main menu**: Save the game state and return to 'main menu' game status
- **On quit game**: Save the game state and quit the game
- **On click options**: Change state to 'settings menu' and open the settings view in UI Manager
- **On continue game**: Change state to 'ingame' and continue the game
- **On pause game**: Change state to 'pause menu' open the pause menu view in UI Manager and pause the game
- **On new game**: Instruct the GameSaveProvider to reset the save file and start a new save and then start the game and set the status to 'ingame'
- **On continue game**: Instruct the GameSaveProvider to load the current saved game status and then start the game and set the status to 'ingame'

#### 2.5.2 The **GameSaveProvider** must support saving and loading game states, allowing players to resume their adventure seamlessly.
- **Manage save slot** Players should be able to save at specific checkpoints (collected elemelons = completed dungeons) and have one save slot for one playthrough
- **Save Game**: Save the collected elemelons and stats (max health and max mana) in a scriptable object
- **Load Game**: Load the current save using the saved scriptable object for stats and collected elemelons

### 2.6 UI System

#### 2.6.1 UI Manager

- **UIManager** should manage UI objects to display and change the UI elements to reflect the game state, should manage healthbar, manabar, spell charges, aiming direction and main menu widgets
- **Update health**: call the HealthBar to continuously update the health ui using the player health
- **Show hurt**: call the HealthBar to show the hurt animation
- **Update mana**: call the ManaBar to continuously update the mana ui using the player mana
- **Show hurt**: call the ManaBar to show the hurt animation
- **Update aiming arrow**: using the input manager get the current mouse aiming direction of player and show an arrow around the player sprite in that direction
- **Clear spell charge UI**: clear all spell charges from the canvas so that no charges are displayed
- **Load charge UI**: given a charge sprite type, check how many charge UIs are displayed and then if none are, display the charge on top of the player at position 1, if one is displayed then show the charge at position 2, else do nothing
- **Update speed buff animation**: Continuously update the horizontal and vertical direction vector of the speed buff animator by getting the direction of the player movement. The animation is not be displayed by this but just updates the necessary variables
- **Start/Stop speed buff animation**: Start or stop the animation which displays blue dust particles at the players feet flying in the opposite direction of the player's movement
- **Update Buff/Debuff Icon Bar**: call the Buff/Debuff Icon Bar to continuously update the icon bar ui using the player's active buffs

#### 2.6.2 HealthBar

- **Update health UI using player health**: display max health amount of (red) hearts of which player health are filled and rest are empty
- **Execute hurt animation**: If method is called this starts an animation where all hearts shortly light up white
- **Execute heal animation**: If method is called this starts an animation where all hearts shortly light up blue

#### 2.6.3 ManaBar

- **Update mana UI using player mana**: display max mana amount of (blue) stars of which player mana are filled and rest are empty
- **Execute hurt animation**: If method is called this starts an animation where all stars shortly light up white
- **Execute heal animation**: If method is called this starts an animation where all stars shortly light up violet

#### 2.6.4 Camera Movement

- **Update position using player**: Should be centered at player transform position
- **Update resolution**: Should have method to adjust the size of the camera view, currently: only sensible constant value allowed

#### 2.6.5 Menu UI

- **Manage menu views**: Allow the display of different menu views: Start game / Main Menu view, pause game view, settings view (and subviews)
- **Display start game view**: Displays the following widgets: 'New Game', 'Continue Game', 'Options', 'Quit Game'
- **Display pause game view**: Displays the following widgets: 'Continue Game', 'Options', 'Quit to Main Menu'
- **Display settings view**: Displays the following widgets: 'Control Settings'
- **Display Control Settings view**: Show key mapping between action and controls (change keys by clicking on keys and pressing another, handled by input manager)
- All buttons should have a callback method which can handle the click event (callbacks in Game Manager, except settings which are handled by Input Manager)

#### 2.6.6 Dialogue UI

- **Display Dialogue**: Given a name and dialogue text: dialogue should be as a rectangular dialogue box, text should be written into the box within the given space. When one box is full, the dialogue should wait for continuation and only on continue should further text be displayed. Above the dialog box display the name and portrait of the speaker.

#### 2.6.7 Buff/Debuff Icon Bar

- **Add buff**: Add a buff type to the player's buffs, player can have unlimited buffs and buffs of the same type.
- **Remove buff**: Remove a buff type from the player's buffs, if player has multiple of this type, only one is removed.
- **Update and display icon bar**: Given a fixed list of images of fixed length 'L' (length can be altered in the GUI) where buff icons can be displayed, select the sprite corresponding to the first L buffs of the player and display them in the images.

### 2.7 Interaction and Dialogue System

#### 2.7.1 Dialogue Manager

- **Manage internal game state**: Keep track of the current game progression (e.g through the Game Manager) to keep track of current possible dialogs
- **Manage Dialog Document**: Should be structured by Participating object, current progression level, dialog text
- **Load Dialog from Document**: Efficiently load the document at appropriate timings to make dialogue accessible at runtime
- **Execute dialog**: Given a Participating object and the current progression level, get the appropriate dialog text and use the UI manager to display the dialog, dialogue should be displayed until player presses the continue button (add to Input manager), during the dialog the rest of the game should be paused (use Game Manager)

#### 2.7.2 Interactable Object (abstract)

- **Interact Method**: Abstract interaction method which can be implemented by the concrete classes
- **Interaction Radius**: Has a float value between 0 and 10 (default 2f), in which radius the player can interact with the object
- **Collider and Layer Mask**: All interactable objects have to be places in the "Interactable" Layer Mask and should have a collider2D for detection
- **Is Able to Interact Check**: Check method that returns True, if the player is able to interact with the object; The default implementation should take a transform (player) as input and return true if the transform is within the interaction radius distance.
- **Interactable Layer**: All interactable objects should have a layer mask "Interactable Layer" with which the Interaction System can determine interchangeable objects

#### 2.7.3 NPC (abstract)

- **Change state**: abstract method to change the current state/objective of the NPC
  - Uses a NPC Status enum which is defined for all NPCs the same
- **Act on status**: abstract method to act in concretely implemented methods given the current status

#### 2.7.4 King: Implements NPC, Interactable Object

- **On walk status**: Given a number of positions (at least 1) King should find a somewhat straight path to the positions and walk to each one in order, dialog should not be allowed
- **On idle status**: King should remain in place and should allow interaction to start a dialogue
- **Interact method**: On interact execute the currently viable dialogue in the current game progression

#### 2.7.5 Interaction System

- **Interaction Radius**: Has a float value between 0 and 10 (default 2f), in which radius the player can attempt interaction with interactable objects
- **Player Component**: The interaction system is a component of the player and should have an internal reference to the player's transform object to check all interactable objects within the interaction radius
- **Interact with closest interactable**: Method that first finds all interactable objects within the interaction radius where the **Is Able to Interact Check** is True and then executes the **Interaction Method** of the closest of those to the player transform

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
