# Requirements Analysis Document for *The Elemelons*

## 1. Introduction

### 1.1 Purpose of the System
*The Elemelons* is a 2D dungeon crawler game where players explore elemental-themed dungeons, battle enemies, and retrieve magical artifacts called Elemelons. The goal is to restore balance to the kingdom by collecting all four Elemelons. The game emphasizes exploration, and tactical combat, providing players with an engaging fantasy experience.

### 1.2 Scope of the System
The game targets casual to midcore players who enjoy fantasy, dungeon crawlers, and pixel art. The target audience includes players aged 12 and above. *The Elemelons* will be developed for the PC platform using Unity version 2022.3.50f1.

### 1.3 Objectives
- Provide an immersive dungeon-crawling experience with a focus on elemental-themed environments.
- Implement a tactical combat system that emphasizes spell-casting and resource management.
- Develop procedurally generated dungeons to ensure replayability.
- Deliver a polished visual experience with 2D pixel art and fluid animations.

### 1.4 Definitions, Acronyms, and Abbreviations
- **Player**: The main character controlled by the user, capable of casting spells and navigating dungeons.
- **Spell**: A magical ability created by combining elemental powers, used for combat or interacting with the environment.
- **Elements**: Basic building blocks for spells; includes Fire, Water, Earth, and Wind.
- **Enemy**: Non-playable characters that oppose the player, often with elemental strength.
- **Boss**: A powerful enemy encountered at the end of a dungeon, representing the dungeon's element and guarding an Elemelon.
- **Dungeons**: Procedurally generated environments where the player encounters enemies and collects items.
- **NPC (Non-Player Character)**: Characters like the King who interact with the player but are not controlled by the player.
- **Altar**: An object found in dungeons that restores the player's health, increases his maximal health or mana, or grants a random buff when interacted with.
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
  - With fire, none: A floating, harmless flame appears that floats around and follows the player. Serves to illuminate darkness.
  - With fire, fire selected: casts a fireball that first charges up for a short while (< 1 second) and then flies into the given direction and has a longer spell lifetime (variable to be changed ~ 3 sec), explodes on impact (wall, enemy) and deals large amount of damage (variable to be changed ~ 2 hearts) in an AOE where the radius of effect is an edittable variable ~ 1.5 tiles
  - With wind, Wind selected: casts a speed buff spell that boosts the movement speed of the player by a factor (variable between 1 and 2) for a longer  time (~30 sec). Should delegate the buff creation to the Buff/Debuff Manager which in return delegates the icon creation and display to the UI Manager. The Buff/Debuff Manager should start the speed buff animation as long as the buff is applied (see Buff/Debuff Manager for details). The buff should not be stackable.
  - With fire, earth: cast stationary bomb that triggers an AOE explosion after ~3 seconds, damaging all non-staggered enemies in its range
  - With fire, wind: cast a fast fire tornado that deals small damage when it collides with enemies. Bounces off walls. Expires after medium time ~5s
  - With earth, wind: cast a circle of 6 shards that rotate around the player and individually break at impact with enemy and deal minor damage (~0.5hearts). If any shards are left at end of spell lifetime (~10s), they break
  - With water, earth: For some time (positive integer can be altered ~6s) the player leaves behind small puddles of a given radius (positive integer can be altered ~1.5 units) in which Enemies that touch the puddle get a slowness debuff (slowness factor can be altered ~0.3 and effect time very short ~ 1s) that is not stackable but gets reapplied as long as the Enemy touches the puddle. Debuff handling should be done in the BuffDebuff-Manager, the puddle should have a sprite that shows a muddy puddle.
  - With earth, earth: Raises a solid wall at an variable distance (~ 1-2 units) in the direction from the player to the place where the user is pointing (with the mouse). The wall is ~0.5 units wide and should have a collider that blocks all Enemies and spells. The flat part of the wall is approximately facing the player. Should disappear after around 7s.
  - With earth, none selected: casts a defense buff spell that boosts the defense factor of the player by a factor (variable between 1 and 2) for a longer time (~20 sec). Should delegate the buff creation to the Buff/Debuff Manager which in return delegates the icon creation and display to the UI Manager. The Buff/Debuff Manager should start the earth defense animation as long as the buff is applied (see Buff/Debuff Manager for details). The eath defense animation should show an earth layer that encompasses the player sprite. Player cannot move while the earth layer appearance animation is playing.
  - With wind, none: Player dashes forward an variable distance (~5 units) in the direction from the player to the place where the user is pointing (with the mouse). Should show a dash animation with trails behind the moving player sprite. If the player collides with a collider during the dash, the dash is stopped.
  - With water, none: water splash cleanses player of all buffs and debuffs instantly with a small animation that takes around 1 sec. Player cannot move during animation.
  - With water, water: a large (~3x5 units) tsunami wave is cast in the direction the player is aiming. The wavefront deals medium damage (~1.5 hearts). The wave disappears after around 2 secs.
  - With water, wind: A blizzard cloud that wobbles around randomly appears. Enemies that come in contact with it are frozen into ice crystals froma short time (2s) and cannot perform any kind of action while frozen. Diffuses after around 7 s.
  - With water, fire: A large (~8x8 units) cloud of water vapour appears. The player is hidden from enemies while inside the cloud and the enemies cannot locate the player while inside the cloud unless extremely close (~1 unit). Diffuses after around 13 s.
 
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

- **Generate Map**: Given a start point create all data necessary for the map data and return a MapData object. Should create floor, wall, monster spawn point, door, altar, darkness (for darkened rooms) positions as well as the different rooms
  - should call abstract methods for all the map positions which can later be implemented in the concrete map generators
  - positions should be saved in hash set objects
 
#### 2.2.3 Bipartitioning Map Generator

- **Create Floor**: Use recursive bipartitioning to partition the given rectangular dungeon with its height and width into rectangular rooms with a minimum height and width as well as an offset between the rooms and then connect the rooms
  - The rooms are connected by choosing a random starting room and then connecting the middle point of that room to the middle point of the other room and the corridors have width with a given corridorWidth
  - The rooms are added to the map data in order of the connections
  - The first room is denoted as room type START, the last as END, for the other rooms the first half is denoted as NORMAL and the other half as ADVANCED
- **Create walls**: Creates the wall positions by adding the outline of the floor to the wall data
  - Given a hash map of floor positions, the wall positions are the positions adjacent to the floor positions but not contained in them
- **Create doors**: Creates the door positions between rooms and corridors
  - Given room positions and corridor positions the door position is in the middle of the corridor adjacent to a room
- **Create monster spawnpoints**: For each room have a different pool of possible monsters depending on whether the room is of type NORMAL or ADVANCED, the ADVANCED rooms should have harder monsters. In each room select a random number of 0-5 floor positions, where the specific monster that is supposed to spawn there is denoted. The spawn positions have to be unique.
- **Create decoration positions**: Randomly select positions on the floor and wall to place decorations (solid and not solid)
- **Create altar positions**: With a variable probability between 0 and 40% for each room that is of type NORMAL or ADVANCED, spawn a shrine in the room at a random viable position if one is found. A position is viable if it is 3 tiles away from any wall and has a 3x3 field for the shrine without any monster positions.
- **Spawn Altars**: Given the shrine spawn points, instantiate altars at the given position.
- **Create dark rooms**: With a variable probability between 0 and 20% for each room that is of type NORMAL or ADVANCED, select rooms to be a dark room in which the player cannot see anything except darkness. The only way for the player to see, is to spawn the fire light around him using the according spell.

#### 2.2.4 Tilemap Visualization (abstract)

- **Paint tiles**: abstract method to paint all tiles given a MapData object, which can be implemented by the concrete visualizers for the different dungeon types
- **Clear**: abstract method to clear the tilemaps of the dungeon

#### 2.2.5 Earth Tilemap Visualizer

- **Paint tiles**: Paints floor, walls and decorations using the methods specified below
- **Paint floor**: Using an atlas tilemap with predefined floor patterns, place a rule tile for the rim tiles on the floor rims (adjacent to the walls) and map the atlas floor pattern to the rest of the floor
  - Starting point for the atlas should be seeded randomly in a way that the whole floor map fits into the selected atlas region
  - Rim of the floor should show the 12 possible rim tiles in the correct configuration (4 top corners, 4 bottom corners, 4 left, right, top, bottom straight rims)
  - The rim is either painted in sand colors for a tile closer to a NORMAL room or in grass colors for a tile closer to an ADVANCED room.
- **Paint walls**: Using the wall positions places different wall tiles depending on the position in the room
  - Walls on top of the room are two positions wide and contain the two tile high wall tiles
  - Walls on the left or right side of the room are one tile wide and contain the upper view of the wall
  - Walls on the bottom of the room are one black tile wide
  - Walls either use the sand textures for a tile closer to a NORMAL room or grass textures for a tile closer to an ADVANCED room.
- **Decorate dungeon**: Given the decoration positions, choose randomly one of the many possible decorations (distinguish between wall and floor decorations) and place the decorations, they should have the same collisions as the floor or wall before.
- **Clear**: Clears the floor and wall tilemap

#### 2.2.6 Monster Manager

- **Manage monsters** Keep references of all monsters in the dungeon. Close and open doors depending on where the player and monsters are
- **Spawn Monsters**: Given the monster spawn points with monster names of the map data, instantiate the respective monster prefabs with health = max health and mana = max mana, and initialize pathfinding and other dependencies on the player. Save references to the monsters, so the manager knows when all are defeated.
- **On all monsters defeated**: Emits an event once all the monsters of the dungeon are cleared/defeated, this can be used by the **Main Game** to progress the game.

#### 2.2.7 Altars:

- **Types**: Shrine can be golden or not, and can either contain blood or a pink fluid.
  - Case golden and blood: Spawns a golden collectable heart that increases the player’s max health by 1 on interact.
  - Case not golden and  blood: Spawns 1-3 collectable hearts that each heal the player’s health by 1 on interact.
  - Case golden and pink fluid: Spawns a golden collectable mana star that increases the player’s max mana by 1 on interact.
  - Case not golden and pink fluid: Spawns a collectable potion that grants the player a random buff (damage, defense or speed) that has a duration of ~2mins on interact.
- **On Interact**: Plays self-destruct animation and spawns loot depending on altar type. The “Collectable”-script component of the collectables is deactivated first.
- **DoMovement()**: Moves each spawned collectable around in a parabola shaped trajectory. The distance each collectable travels is measured and after a short random distance, the movement is interupted and their collectable scripts become enabled.
- After all spawned collectables have travelled their desired distance and their collectable scripts have been activated, Update() deactivates the altar’s altar script.

#### 2.2.8 Dark Rooms:

- **Shroud in Darkness**: The player should not be able to see anything except darkness inside a dark room, monsters and tiles are hidden like a dark filter above the room.
- **Allow light**: Using the fire none spell, the player has to be able to light a dark room around the circular light cone which exposes the actual contents of the room. Other fire based spells also create some light, but aren't very suited for illumination as they're are short lived or produce little light.

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

#### 2.3.2 Enemy Path Finding
- should be able to construct path to player by taking into consideration the collision objects (first walls)
- implementation should be seemless so that immediate direction where the enemy should walk to approach player should be returned/noted down
- should be reusable as module for all enemies
  
#### 2.3.3 Tree Log (Enemy)

- **Act on status**:
  - while status 'idle': If player moves into chase radius then status is updated to 'walking'
  - while status 'walk': If player inside attack radius then status is updated to 'attacking'; If player outside attack radius but inside chase radius then Tree Log pathfinds toward the player; If player outside chase radius and Tree Log not at home position, then Tree Log moves toward home position; If player outside chase raidius and Tree Log at home position, updates status to 'idle'
  - while status 'attacking': Tree log stands still and attempts to engage combat with the player depending on the BoxCollider of both objects, if collision, combat is handles by the Combat Manager
  - Animations should reflect the status and if 'walking' also the direction in which the Tree Log is headed
- **Change state**: Given the new state, check if state is possible (possibleStates in Tree Log) and then change the state to the new state if the new state is possible for Tree Log
- **Check health status**: If health is zero, the Tree Log Dies and triggers the death animation
- **Get Enemy Element Type**: Tree Log should be of element type EARTH

#### 2.3.4 Mole (Enemy)

- **Act on status**:
  - while status 'idle': If player moves into chase radius then status is updated to 'walking', the mole is dug under the earth for most of the idle time but every x amount of time digs up to show its face and then digs down again (x is a variable between 3 and 20 seconds); While the mole is underground it is invincible and cannot be attacked
  - while status 'walk': If player inside attack radius then status is updated to 'attacking'; If player outside attack radius but inside chase radius then the mole pathfinds toward the player while remaining under ground; If player outside chase radius, updates status to 'idle'
  - while status 'attacking': The mole digs up and attempts to engage combat with the player depending on the BoxCollider of the arms of the mole and the player, if collision, combat is handles by the Combat Manager
  - Animations should reflect the status and if 'walking' also the direction in which the Mole is digging
- **Get Damage**: The Mole is invincible while it is dug under ground, once it is visible it can be hit and should have a hit animation when it receives damage
- **Attack**: The mole attacks in status 'attacking' and uses its arms which should have an animation to attempt to hit the player at its sides, the damage should be variable between 0.5 and 2 hearts
- **Change state**: Given the new state, check if state is possible (possibleStates in Mole) and then change the state to the new state if the new state is possible for Mole
- **Check health status**: If health is zero, the Mole Dies and triggers the death animation
- **Get Enemy Element Type**: Tree Mole should be of element type EARTH

#### 2.3.5 Tiki (Enemy)

- **Act on status**:
  - while status 'idle': If player moves into chase radius then status is updated to 'walk',
  - while status ‘walk': tiki pathfinds to player until he reaches his attack range and status is updated to 'circle’
  - while status ‘circle': the tiki circles around the player at a given radius. If the tiki hits a wall, he changes direction. In order to avoid getting stuck, the circling range is varied randomly at each collision. after a given number of wall collisions, he switches back into ‘walk’.  If there is an enemy nearby that is low on health, tiki will update status to 'heal’. This is not possible if the head is detached.
  - while status ‘heal’ tiki pathfinds to wounded enemies and heals them by a set number of healthpoints.
- Animations should reflect whether tiki is walking or idle.
- if the tikis health is below 50%, its movespeed increases by x1.5
- Get Damage: The tikis head is invincible. The body is vulnerable. Killing the body will make the head disappear. The body is much more resistant when attached to the head.
- **Attack**: After circling the player for a certain time, the tikis head will start spinning and detach from the body to attack the player, then reattach to the body. The head cannot receive damage. If the distance and the tiki is shorter than the tikis range and the head is attached, the tiki will shoot the player with a weak spell.
- Act on health: If the tiki’s health is below 25% health  and the player is out of range for a flying head attack, tiki will shoot its spell in all directions.
- Change state: Given the new state, check if state is possible (possibleStates in Tiki) and then change the state to the new state if the new state is possible for Tiki
- Check health status: If health is zero, the Tiki Dies and triggers the death animation
- Get Enemy Element Type: Tiki should be of element type EARTH

#### 2.3.6 SpearShieldMan (Enemy)

- Has spear, shield or both: Tiki is randomly spawned in either of these configurations, which determine his attack and defense
- Act on status:
  - while status 'idle': If player moves into chase radius then status is updated to 'walk',
  - while status ‘walk': tiki pathfinds to player until he reaches his attack range and status is updated to 'attack’
  - while status ‘attack': Attack with spear, shield or either (randomly if it is equipped with both).
- Animations should reflect whether SpearShieldman is walking or idle.
- Get Damage: If the SpearShieldMan is equipped with a shield the defenseFactor is higher (~2), else ~1.
- Attack: Slashes forward and attempts to hit the player, if the attack was with a spear the damage is bigger (~1) but the knock time lower (~0.2 s), if the attack is with the shield, the attack is weaker (~0.5) but the knock time is longer (~1s).
- Act on health: If the SpearShieldMan’s health is below 25% health and it is equipped with a spear, and the player is in spear range, attempts to throw the spear at the player, which has high attack damage (~1.5) but then the spear is lost.
- Change state: Given the new state, check if state is possible (possibleStates in SpearShieldMan) and then change the state to the new state if the new state is possible for SpearShieldMan
- Check health status: If health is zero, the SpearShieldMan Dies and triggers the death animation
- Get Enemy Element Type: SpearShieldMan should be of element type EARTH

#### 2.3.7 Snek (Enemy):

- Attack coroutine: attack where it dashes towards the player. Conditions for this attack: Distance to player smaller than ~3 and cooldown from previous attack has run out.
- Cooldown coroutine: (~6s) cooldown for attack. triggered after each attack.
- **NewRayCast**: Short (~1 unit) raycasts in movement direction. hit is saved as global variable.
- **ActOnStatus**: Executes OnXXXStatus() method corresponding to the snake’s state.
- **OnDashStatus**: Dash status when snake is ~10 units or closer to player. Snake increases its speed towards player. However, if attackcooldown is still running, snake will chose the direction with which it can move away from the player. Animation reflects status.
- **OnCrawlStatus**: If snake is too far away, movement speed is decreased. Moves towards player if attack cooldown isnt running, else moves away.
- While cooldown is running, runs away from player.
- **OnAttackStatus**: moves in direction which was decided at the moment it switched into this state (towards player). Automatically switches out of state after given time (~2s).
- **OnIdleStatus**: no movement.
- **DoMovement**: If raycast from NewRayCast() detects a wall, movement vector is rotated by ~20 deg and new raycasts generated until the snake finds a free path or that movement been rotated by 360 deg. Executes chosen movement.
- **ActOnHealth**: If health is zero, the snake Dies and triggers the death animation.
- **UpdateAnimator**: Update animator to reflect snake’s current state and movement direction.
- **OnTriggerEnter2D**: Deals damage (~0.5 hearts) to the player if they collide.
- **In earthdungeon**: spawns in last room before boss.
- **Change state**: Given the new state change the state to the new state.
- **Get Enemy Element Type**: snake should be of element type EARTH

#### 2.3.8 Witch (Enemy):

- **ActOnStatus**: Called every frame. Executes OnXXXStatus() method corresponding to the witch’s state.
- **CalcRangeState**: Measures distance to player and updates current rangestate accordingly. ~8-12 units is in range, less is close, more is far. >50 units is out of range.
- **NewRayCast**: Called every frame. Short (~1 unit) raycasts in movement direction. hit is saved as global variable.
- **ThrowDice**: Rolls dice every few seconds (~3-9). Some die are rolled every 3s, some every 6s, some every 9s. An individual die used to decide on how the witch should behave or which ability she should use.
- **ReadDice**: Given a specific ability, checks its probability given the WitchMoveDictionary and the number its die is currently holding and returns whether the ability can currently be executed, not taking its cooldown in consideration.
- **WitchMoveDictionary**: A dictionary initialized at start containing information on all of the witch’s abilities, such as their cooldown time, probability, tied gameobject, time needed to cast, method that executes the ability, whether the cooldown is running and which die should be read.
- **Chose**: Given a list of abilities and taking their probability into account, randomly executes one of these abilities.
- **ExecuteMove**: Given an ability, executes it (via the dictionary)
- **UpdateAnimators**: Update animator for body, wand and hair to reflect witch’s current state and movement direction.
- **OnIdleStatus**: Is idle if player out of range. no movement.
- **OnWalkStatus**: Is in walkstatus if still has >~80%health. Witch randomly executes an ability from the following selection if their cooldown isnt running, the dice allow it and depending on her distance from the player: Greenpoof, RedPoof, GreenSpray, Dash, Boulder. Else she walks around randomly.
- **OnHoverStatus**:  Is in hoverstatus if still has ~80%>health >~30%. Witch randomly executes an ability from the following selection if their cooldown isnt running, the dice allow it and depending on her distance from the player: Greenpoof, RedPoof, GreenSpray, Dash, Boulder, Vortex, Medusa. Else she walks around randomly.
- **OnBaldStatus**: Is in baldstatus if  has <~30% health. Witch randomly executes an ability from the following selection if their cooldown isnt running, the dice allow it and depending on her distance from the player: Greenpoof, RedPoof, GreenSpray, Dash, Boulder, Vortex. Else she walks around randomly.
- **About the random Movement**: Will either move in straight lines or circle trajectory for random time depending on dice. If raycast detects wall, walk direction is inverted. After a random number of direction changes (~>4), may execute ability Ram Player.
- **DoMovement**: Executes chosen movement.
- **ActOnHealth**: Called every frame. If health is zero, the witch Dies and triggers the death animation. Also deactivates the bossbar and kills all other enemies in a 100 unit radius. Else, updates the int healthphase (depending on health percentage), read by other functions to determine parameters of attacks. Also updates state depending on healthphase. Executes the ‘Snek’ ability if transitions into bald status.
- **Change state**: Given the new state change the state to the new state.
- **Get Enemy Element Type**: witch should be of element type EARTH
- **DoCoolDown**: Conducts cooldown of given ability.
- **HandleCast**: Witch takes out wand and freezes during cast time. If spell to cast is offensive, show red sparkle effects around wand
### The abilities:
- **Dash**: Given a list of node positions, randomly selects a node in proximity and sets it as destination. Witch will ‘dash’ towards it with increased speed for a short time.
- **Medusa**: Instantiates the medusa effect and shows animation in witch’s hair. 
  - **Medusa** effect: Petrification animation that appears in front of the player sprite and prevents player input during its spell lifetime (~3s). Plays decomposition animation at end of spell life and gameobject is destroyed.
- **Snek**: Shows animation where the snake hair of the witch leave her and become 1 single independent snake. Witch is left bald. Instantiates a snake enemy.
- **Boulder**: Casts a few boulder spells at random locations inside of the bossroom. Number of boulders increases as the witch’s health becomes lower. 
  - **Boulder effect**: Boulder that free falls ~20 units and then shatters (on the ground) , dealing damage (~1heart) to the player on collision when it is ~2 units or closer to its shattering point. Drop shadow indicates where it will hit the ground. GameObject is destroyed after shatter animation.
- **Vortex**: Casts vortex spells. Number increases as the witch’s health becomes lower. 
  - **Vortex effect**: Vortex that moves in a curve away from its caster. Deals minor damage to player on collision (~0.1 hearts). Sucks player and enemies in that are in its range (~10 units). Force parameter controls strength. Also can randomly spawn red sparkles around itself. 
  - **Red Sparkle effect**: Red sparkle effect. Gameobject is destroyed after spelllifetime runs out ~2s. Deals a weakness or defense Debuff to the player on collision, chosen randomly. Can also do both.
- **Green Poof**: witch teleports to random node in proximity and leaves behind a green poof effect. 
  - **Green Poof effect**: Just a visual effect. Small green cloud. Gameobject is destroyed after spelllifetime runs out ~0.6s.
- **Red Poof**: witch casts a red poof effect and  teleports to random node in proximity and leaves behind a green poof effect.
  - **Red Poof effect**: Small Red cloud that disappears after ~0.6s, but then grows big and becomes a big red cloud that obscures sight for the player, as it is rendered above enemies, player, walls, floor etc. Gameobject is destroyed after spell lifetime runs out ~13s.
- **Green Spray**: Casts several green spells (same as tiki), that 'spray' at different angles. Number and angle coverage increases as the witch’s health becomes lower.
- **Ram Player**: Takes position of player at the moment the move was started and dashes towards it.

### 2.4 Combat System

#### 2.4.1 Combat Manager: Manages combat damage, health and mana calculations
- **Cast spells**: Given a spell type with predefined attributes (i.e position, rotation) spawn the given spell object
- **Add buff or debuff**: For a given object (e.g the player or an enemy), use the Manager to apply the given buff/debuff
- **Calculate Damage**: Using the spell/attack type and the type of the opponent the attack/spell hits, calculates the damage using the formula base_attack_damage * damage_factor(of attacker) / defense_factor(of defender) * attack_type_effectivity_modifier, the effectivity modifier is given as (attack type first - defender type second): Any - BASIC = 1, FIRE - EARTH = 1.25, EARTH - AIR = 1.25, AIR - WATER = 1.25, WATER - FIRE = 1.25, the modifiers should be specified in a dictionary so that they can be changed later.
- **Execute attack**: callback function for all attacks which are called by the participating objects, given the caller object (attacker) and the hit object (defender) calculate the attack damage, subtract the damage of the defender's health, set defender to status staggered and knock back the defender, if the defender is the player and the player dies, instruct the Game System to act on player death

#### 2.4.2 Buff/Debuff Manager: Manages buffs and debuffs for the player and enmies alike
- **Apply Buff or Debuff**: Start a timer of given length to apply the buff or debuff during the timer to any buffable object (Enemy or Player) and execute the effects. Then if the target of the buff/debuff is the player, delegate the UI Manager to create the buff/debuff effect in the UI, buffs and debuffs should have a name and a stackable attribute whether they can stack. If the buff/debuff is stackable a buffable object can have unlimited buffs/debuffs of the same name, if not only one of the name should be applied and if another is attempted to be applied, nothing happens.
- The different buffs are:
    - Speed buff: Player or Enemy have increased movement speed during the application
    - Strength buff: Player or Enemy have increased damage (using a damage factor)
    - Defense buff: Player or Enemy have increased defense (using a defense factor)
    - Mana regen buff: Player of Enemy have increased mana regen
    - Hidden: Buff that can only apply to the player. While this buff is active, the enemies cannot see the player and will walk around randomly.
 - The different debuffs are:
    - Slowness debuff: Player or Enemy have decreased movement speed during the application
    - Weakness debuff: Player or Enemy have decreased damage (using a damage factor)
    - Fragility debuff: Player or Enemy have decreased defense (using a defense factor)
    - Mana regen debuff: Player of Enemy have decreased mana regen
    - Freeze: Freezes the target into an ice crystal. While this debuff is active, the target cannot move, interact or attack.
    - Petrified: Turns the target into stone. While this debuff is active, the target cannot move, interact or attack.
    - Confused: Debuff that can only apply to enemies. While this debuff is active, the target walks around randomly.
  - Each buff has an associated duration and an effect factor, for the duration of the effect the respective attribute (movement speed, damage, etc.) is multiplied (buff) or divided (debuff) by the given factor (factors stack, i.e if two buffs with factor 2 are applied, the resulting factor is 4)
  - **Speed Buff** additional animations: If the speed buff is applied to the player, the UI Manager should continuously update the speed buff animation which shows dust particles in the opposite direction of the movement during the duration of the speed buff (see update and start speed buff animation in the UI Manager)
- **Freeze** additional animations: Ice crystal appears around target while debuff active
- **Confused** additional animation: question mark above target's head while debuff active
- **Petrified** additional animation for player: Target turned into stone while debuff active
- **Defense Buff** additional animation for player: Target covered in dirt/mud while buff active 
#### 2.4.3 Health

- **Subtract Health**: subtracts a floating input from the total health, if health drop below zero, health is set to zero
- **Add health**: adds a floating input to the total health, if health gets higher than max health, health is set to max health
- **Get health**: Returns the current total health value
- **Is zero**: Returns true if the current health is zero, false otherwise
- **Set new max health**: sets a new max health, should be positive interger

#### 2.4.4 Mana

- **Subtract mana**: subtracts a floating input from the total mana, if mana drop below zero, mana is set to zero
- **Add mana**: adds a floating input to the total mana, if mana gets higher than max mana, mana is set to max mana
- **Get mana**: Returns the current total mana value
- **Is zero**: Returns true if the current mana is zero, false otherwise
- - **Set new max mana**: sets a new max mana, should be positive interger

### 2.4.5 Collectables

- Abstract class for collectable items:
- **InitializeGeneral**: Initializes necessary components.
- **Movement**: Object is pulled towards player in in range (~2 units), else performs a hovering motion on a fixed location.
- **OnTriggerEnter2D**: Calls the abstract method OnCollect() if collision with the player detected.
- **Collectable items**:
- CollectableHeart heals the player by 1 health OnCollect
- CollectableMaxHeart increases the player maxhealth by 1 OnCollect
- CollectableMana fills up the player’s mana by 1
- CollectableMaxMana increases the player maxmana by 1 OnCollect
- CollectablePinkPotion** grants the player a random Buff for ~2mins with a factor of ~1.5 (Speed, Strength, Defense)
- All collectables play an animation OnCollect and then self-destruct

### 2.5 Game Progression and Save System

#### 2.5.1 The **GameManager** must control the overall game state, including dungeon transitions, combat phases, and player progression

- **Manage game progression**: For each game save there should be six different progression stages:
  1. (CASTLE_INTRODUCTION) Player is in the castle hub and has not yet retrieved the earth elemelon. The evil witch has stolen the elemelons. The king will tell the player that the elemelons have been stolen.
  2. (CASTLE_EARLY) Player is in the castle hub and has not yet retrieved the earth elemelon. The king has his usual rambly dialogue.
  3. (EARTH_EARLY) Player has first entered the earth dungeon (no earth elemelon has been retrieved yet). The player fights through the dungeon, defeating monsters and searching the boss room. After the Player has defeated all normal monsters in the earth dungeon and has then entered the door to the boss fight. The player stands now in the boss room.
  4. (CASTLE_END) Player has defeated the witch boss and received the elemelon. He returns to the castle to give it back to the king.
  5. (CASTLE_LATE) Player is in the castle hub and has retreived the earth elemelon.
  6. (EARTH_LATE) Player enters the earth dungeon after they have already retrieved the earth elemelon. They can fight all monsters and the boss, but no elemelon can be retrieved anymore.
- **Change Game Stage**: Enable the change of the game progression stage. Use the **GameSaveProvider** to save the updated stage of the player unless the stage is EARTH_EARLY or EARTH_LATE.
- **Start Main Game Stage**: When the game progression stage is updated, then the GameManager uses the **Main Game** to execute the progression stage game content.
- **Manage game status**: Game status should be saved as game status (enum for 'ingame', 'main menu', 'pause menu', 'dialogue', 'settings menu', 'death screen') and be able to switch between them through actions
- **On player death**: Change state to 'death screen' and open the death screen view in the **UI Manager**.
- **On go to main menu**: Use the **GameSaveProvider** to save the game progression stage and return to 'main menu' game status
- **On quit game**: Use the **GameSaveProvider** to save the game progression stage and quit the game
- **On pause menu**: Change state to 'pause game' open the pause menu view in **UI Manager** and pause the game (no monster movement, no attacking, no player movement)
- **On start dialogue**: Change state to 'dialogue' and pause the game (no monster movement, no attacking, no player movement), should be used if a dialogue is engaged.
- **On open options**: Change state to 'settings menu' and open the settings view in UI Manager.
- **On continue game**: Change state to 'ingame' and continue the game.
- **On new game**: Instruct the GameSaveProvider to reset the save file and start a new save and then start the game and set the status to 'ingame'
- **On continue saved game**: Instruct the GameSaveProvider to load the current saved game status and then start the game and set the status to 'ingame'

#### 2.5.2 The **GameSaveProvider** must support saving and loading game states, allowing players to resume their adventure seamlessly.
- **Manage one saved game**: The game should have the ability to save the progression for one single save game.
- **Save Game**: Save the progression stage of the player, but if the player is in progression stage EARTH_EARLY or EARTH_LATE, save the stage CASTLE_EARLY or CASTLE_LATE instead. Save the player's stats (max health and max mana) and the current button settings.
- **Load Game**: Load the current save using the saved scriptable object for stats, button settings, and progression stage. Set the respective stage in the **GameManager**.

#### 2.5.3 The **Main Game** enables the progression stage specific content of the game

- **In CASTLE_INTRODUCTION**:
  - Scene: Castle Hub Scene
  - Starts with a cut scene slide show dialog explaining what elemelons are and that the witch stole them
  - Then, the game is set to ingame and the player is spawned in the castle hub. The controls are explained to him via tooltips. He has to talk to the king.
  - The king has a special dialogue, telling the player who the evil witch is, that the player witch has to save him and retrieve the elemelons. The king also tells the player that for the first dungeon she will have to travel to an earth dungeon to get the first earth elemelon back.
  - The king asks the player to kill a monster in his bedroom. 
  - After this task is done and the player talks to the king again, the way towards the dungeon is unlocked and the game progresses to stage CASTLE_EARLY.
 
- **In CASTLE_EARLY**:
  - Scene: Castle Hub Scene
  - The king now has three repetitive dialogues where he greets the player, tells the player not to trust the evil witch, and that he counts on the player.
  - When the player enters the earth dungeon portal, the game progresses to the stage EARTH_EARLY.
 
- **In EARTH_EARLY**:
  - Scene: Earth Dungeon Scene
  - The earth dungeon is generated and populated with monsters (TreeStump, Mole, Tiki, SpearShieldsMan, Snek).
  - In the last room of the dungeon (denoted by Room Type END), there is a portal to the boss room, which is unlocked if the last normal monster is beaten (**Monster Manager** should have an event to trigger this).
  - If the player enters the boss room portal, they are teleported to the boss room where the evil witch is waiting with the earth elemelon sitting on a shrine.
  - Once the player has beaten the evil witch in the boss fight, the game progresses to stage CASTLE_END.
  - If the player dies during the dungeon playthrough, the game progression stage is reset to CASTLE_EARLY and the **Game Manager** is instructed to execute the player death screen.
 
- **In CASTLE_END**:
  - Scene: Castle Hub Scene
  - The player is spawned in the Castle where the king is waiting for her.
  - If the player engages dialogue with the king, the earth elemelon is retrieved to the king's room and the king is very thankful.
  - After the dialogue is concluded, the game progresses to stage CASTLE_LATE.
 
- **In CASTLE_LATE**:
  - Scene: Castle Hub Scene
  - The king now has three repetitive dialogues where he greets the player, tells the player not to trust the evil witch, and that he counts on the player.
  - The earth elemelon remains in the king's room.
  - When the player enters the earth door portal (door to the first dungeon), the game progresses to the stage EARTH_LATE.
 
- **In EARTH_LATE**:
  - Scene: Earth Dungeon Scene
  - The earth dungeon is generated and populated with monsters (TreeStump, Mole, Tiki, SpearShieldsMan).
  - In the last room of the dungeon (denoted by Room Type END), there is a door to the boss room, which is unlocked if the last normal monster is beaten (**Monster Manager** should have an event to trigger this).
  - If the player enters the boss room door, they are teleported to the boss room where the evil witch is waiting without any earth elemelon.
  - Once the player has beaten the evil witch in the boss fight, the game progresses to stage CASTLE_LATE.
  - If the player dies during the dungeon playthrough, the game progression stage is reset to CASTLE_LATE and the **Game Manager** is instructed to execute the player death screen.

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

- **Update health UI using player health**: display max health amount of (red) hearts of which player health are filled using steps of 0.25 of one full heart and rest are empty; the 0.25 of one heart is filled up if the according value is more than half or the required, e.g a health value of 1.127 has one full heart and as 0.127 is more than half of 0.25 we see one full heart and one quarter of a filled heart displayed; special case is for health values between 0 and 0.126 (exluding 0), here still the 0.25 of the first heart should be filled, and the fullness should only be zero when the player's health have reached zero.
- **Execute hurt animation**: If method is called this starts an animation where all hearts shortly light up white
- **Execute heal animation**: If method is called this starts an animation where all hearts shortly light up blue

#### 2.6.3 ManaBar

- **Update mana UI using player mana**: display max mana amount of (blue) stars of which player mana are filled using steps of 0.25 of one full mana star and rest are empty; the 0.25 of one star is filled up if the according value is more than half or the required, e.g a mana value of 1.127 has one full star and as 0.127 is more than half of 0.25 we see one full star and one quarter of a filled star displayed; special case is for mana values between 0 and 0.126 (exluding 0), here still the 0.25 of the first star should be filled, and the fullness should only be zero when the player's mana have reached zero.
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

#### 2.6.8 Bossbar:

- Given an attached healthscript, visualizes how much health compared to maxhealth the enemy/player etc of the script has.
-**Update** Updates how much health is shown according to the attached health script
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
- The game must support **keyboard** inputs, allowing players to choose their preferred control method.

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
- The project will primarily utilize **pixelart visuals** for 2D assets and animations.
- The game will primarily target **Windows** operating systems.
- Development tools like **Git** for version control, **Trello** for task management, and **Lucidchart** for diagram creation will be used to ensure efficient project tracking and collaboration.

## 5. Scenarios

1. **Starting the Game and Selecting New Game**
2. **Receiving the Mission from the King**
3. **Learning Controls and Spell-Casting**
4. **First Encounter with Enemies**
5. **Entering an Elemental Dungeon**
6. **Navigating Dungeon and Hazards**
7. **Combat with Elemental Enemies**
8. **Elemental Dungeon Boss Battle**
9. **Collecting the Elemental Elemelon and Returning to the King**
10. **Redoing the Dungeon**

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
