# Game Design Document (GDD)

## 1. Overview
- **Game Title**: The Elemelons
- **Game Genre**: Real-time Dungeon Crawler with Puzzle Elements
- **Target Audience**: Fans of fantasy, dungeon crawlers, and 2D pixel art games. Targeted at ages 12 and above, with casual to midcore players in mind.
- **Platform**: PC
- **Project Scope**: The game is a 2D dungeon crawler where the player controls a witch or wizard tasked with defeating an evil witch and retrieving the Elemelons. The primary focus is on elemental spell combinations, procedural dungeons, and monster-slaying combat.
- **Core Concept**: Elemelon stands out due to its elemental spell combination system. Players use a wand to combine four elements—fire, water, earth, and wind—to create unique offensive and utility spells. The strategic use of these combinations is key to progressing through the procedurally generated dungeons and retrieving the stolen Elemelons.

---

## 2. Game Mechanics

### Core Gameplay Loop
1. **Explore Dungeons**: The player navigates procedurally generated dungeon levels filled with enemies, traps, puzzles, and collectibles.
2. **Battle Monsters**: Use elemental spell combinations to fight off monsters, each with its own elemental affinity and attack style.
3. **Collect the Elemelon**: At the end of each level, the player retrieves one of the king's stolen Elemelons.
4. **Return to the King**: After collecting all the Elemelons, the game is completed, and the player returns victorious.

### Player Controls
- **Movement**: WASD for movement.
- **Elemental Spell Creation**: Two slots for combining elemental powers. Players select elements with number keys (1-4), casts spell with mouse left-click and dispells charges with right-click
- **Elemental Spell Casting**: Every combination of two elements yields a unique spell. 
- **Mana & Health Management**: The player’s spells cost mana, which regenerates over time. Health can be restored at altars found in the dungeon.
- **Interact**: Use "E" to interact with objects or NPCs 

### Combat System
- **Real-Time Combat**: The game features real-time combat where the player must balance offensive and defensive spell combinations based on the situation.
- **Element Properties**: Each element has its own properties, resulting in differently styled spell combinations depending on the elements provided
  - **Fire**: Fire creates high risk and high damage spells, that offer an offensive play style
  - **Water**: Water is a source of health and finesse and creates accurate, efficient and healing spells
  - **Earth**: Earth provides a defensive and slow play style, spells create stationary effects, creating protection spells and strong but slow attacks
  - **Wind**: Wind is for swift and intelligent attacks, providing speed and agility to the player, but also allowing clever attacks
-**Element Hierarchy**: Fire is strong against earth is strong against wind is strong against water
Soeren suggestions:
- **Attack Spells**: 
  - **Fire-Fire**: High damage fire blast, burning every enemy in its path.
  - **Fire-Earth**: Fire meteor, that on impact creates huge damage in an AOE
- **Utility Spells**:
  - **Earth-Earth**: Create an earth barrier around your character, shielding from some damage of other monsters
  - **Wind-Wind**: Use this spell to quickly dash forward in the direction you are looking
Amanda Suggestions:
-**Fire**: Light
-**Fire fire**: Fireball that poofs when hitting target
-**Fire water**: pleasantly warm water vapor, weakens ice enemies , obstructs sight (meaning it reduces aggro range)
-**Fire earth**: Bomb dropped 
-**Fire air**: small fire tornado with seeking
-**Water**: cleansing removes debuffs, “stay hydrated “
-**Water water**: cone tsunami, knocks back+dmg
-**Water air**: blizzard freezes enemies
-**Water earth**: leave puddles of mud while walking crawl style, slows enemies
-**Air**: speed buff
-**Air air**: beyblade of wind around you, AoE
-**Air earth**: shard shroud: lingering floating shards around player that deal damage and disappear uppon impact
-**Earth**: shield of stone
-**Earth earth**: lingering spike area
  
### Level Design
- **Procedural Dungeon Generation**: Each level is procedurally generated, with different layouts, rooms, traps, and enemy spawns. Dungeon should be built in a room-to-room manner, meaning no endlessly long tight corridors. The rooms can vary in size and shape.
- **Element-Themed Levels**: Each dungeon corresponds to an element (Fire, Water, Earth, Wind), affecting the environment and the types of monsters encountered. The optimal strategy against monsters should be different for each dungeon type. For example:
  - **Fire Dungeon**: Lava pits, fire-themed monsters that are immune to fire spells but weak to water spells.
  - **Wind Dungeon**: Gusty environments where wind enhances mobility but also makes ranged attacks harder to control.
    Enemies are fast and try to dodge your attacks/escape you and keep a safe distance
  - **Water Dungeon**: Watery and wet map that makes fire attacks less powerful, but allows player to cleverly use wind and water combination to swiftly move across and through the water
  - **Earth Dungen** Dry and robust dungeon, in which rocks and earth can be used to alter the terrain and attack enemies using the ground. Monsters are typically more slow and robust, have good defense. 
- **Traps and Obstacles**: Include pitfalls, spikes, and elemental hazards that the player must avoid or disable using spells or strategy, teleportataion traps, traps that spawn enemies around player, traps that turn you into a frog for 15s

### Progression System
- **Elemelon Collection**: The goal is to collect all the 4 Elemelons, one in each dungeon, and return them to the king. The Elemelons can be found in boss rooms (1 per dungeon) and are dropped once the boss and all other enemies in this room have been defeated.
- **Character Progression**: Players don’t level up traditionally. The progression should mainly show in the players increasing skillfulness at using spells. The player's maximum mana and health is increased after each retrieval of an elemelon.
  
fire beats earth beats wind beats water beats fire
### NPCs/Enemies (Soeren suggestion)
- **Enemies**: Element-themed monsters with varying resistances and vulnerabilities. Each dungeon level introduces new enemies with elemental affinities that impact combat strategies.
  - **Fire Monsters**: Resist earth spells but are weak to water-based spells.
  - **Water Monsters**: Utilize water-based attacks and are vulnerable to wind spells.
  - **Earth Monsters**: Slow monsters with high HP, use ground attacks and melee combat, vulnerable to earth attacks.
  - **Wind Monsters**: Fast monsters, that are weak to fire attacks.
- **Bosses**: At the end of each dungeon level, there’s a boss fight that requires mastery of the spell system to defeat.
- **The Evil Witch**: The final boss, leader of the monsters, fight unlocked when the last Elemelon is returned to the king.
---
### NPCs/Enemies (Amanda suggestion)
-**fire beats earth beats wind beats water beats fire**
-**Boss Rooms** contain boss monster of the dungeon and some helpers. Location is random, but shouldn't be close to the start of the dungeon
-**Earth Monsters** 
  -**Stone Golem** Boss of the earth dungeon, tanky and slow, hits rarely but deals a lot of damage at once, protects his       minions with earth wakll and can heal those next to him
  -**Mini Golem** Small Version of Stone Golem, help him out. Can deal medium to minor damage by bumping or charging? player.   seek Stone Golem's protection when health is low.
  -**Log** Tree Log sleeps at home coordinates, wakes up and walks to player when in aggro range, walks back to home            coordinates when outside of aggro range. Deals damage by walking against player. Is not supposed to be challenging alone,     meant more as a first enemy.
  -**Shield-and Spearsman** Usually spawn in pairs, shieldsman mostly blocks attacks with his shield and rams player if too     close. Spearsman stays behind shieldsman and occasionally pierces through with spear to hist player. Shieldsman follows       spearsman as long as alive. (Idea in case this is too complicated: combine the 2 units into one)
  -**Mole** Can't be hit while underground, peeks out of ground to attack player

-**Water Monsters**
  -???**Crab** can only walk sideways, deals damage by running into player
-**Wind Monsters**

-**Fire Monsters**
  


## 3. Story and World Building

### Storyline/Premise
You play as a witch summoned through a magic mirror by a desperate king whose castle has been overrun by an army of monsters led by an evil witch. The monsters have stolen the king’s most prized possessions — the Elemelons, magical fruits that represent the elements and are the source of his power. The player's mission is to recover the Elemelons by battling through dungeons filled with elemental creatures, (very simple) puzzles, and traps.

### World/Setting
The game is set in the king’s castle, which has been transformed by the evil witch into a series of dangerous elemental dungeons. Each dungeon represents one of the four classical elements — Fire, Water, Earth, and Wind — where elemental monsters reside, and environmental hazards reflect the dungeon’s theme.

### Main Characters
- **The Player (Witch)**: A powerful spellcaster with the unique ability to combine elemental forces. 
- **The King**: Summons the player to retrieve the Elemelons and restore order.
- **The Evil Witch**: The main antagonist, responsible for the chaos and leader of the monsters.
- **Monsters**: Element-themed creatures with varying abilities and strengths.

### Dialogue and Narrative Structure
- **Speech Bubbles**: Simple pixel-art portraits of characters appear with speech bubbles for dialogue. Conversations happen when the player interacts with the king, special boss monsters, or other NPCs in the dungeon.
- **Linear Story Progression**: The story unfolds as the player retrieves each Elemelon, with a final confrontation with the evil witch.

---

## 4. Art and Visuals

### Art Style
- **Pixel Art**: The game will use a simple, yet detailed, pixel art style. Characters and environments are created with a retro aesthetic, featuring vibrant colors and clear distinctions between elements.
- **2D Grid-Based World**: The game world is viewed top-down on a grid, allowing the player to move through dungeon rooms.

### Character Design
- **Witch/Wizard**: The main character will be represented by a pixel sprite, with simple animations for walking, casting spells, and interacting with objects.
- **Enemies**: Each enemy type is designed to reflect its elemental affiliation (e.g., fire monsters will have fiery textures, water monsters will have flowing or liquid-like animations, etc.).

### Environment Design
- **Dungeons**: Each dungeon is visually themed around one element, with environmental hazards like lava, water pools, rock formations, or gusts of wind.
- **Simple Effects**: Spellcasting will include visual effects like fireballs, water splashes, and swirling winds, all done in pixel art.

### UI/UX Design
- **HUD**: Displays the player’s health and mana bars, equipped elements, selected spells and small inventory bar.
- **Dialogue**: When characters speak, pixel art portraits appear with simple dialogue in speech bubbles.
- **Menus**: Inventory and settings menus will follow a pixelated theme, designed to be intuitive and easy to navigate.

### Animations
- **Spellcasting**: Each spell has a simple animation sequence, like fireballs shooting, wind gusts pushing enemies, and earth barriers forming.

---

## 5. Sound and Music

### Sound Design
- **Elemental Sounds**: Each spell has its own sound effect, such as crackling fire, rushing water, and gusts of wind.
- **Ambient Dungeon Sounds**: Environmental sounds match each dungeon (e.g., bubbling lava in the Fire Dungeon, howling wind in the Wind Dungeon).
- **Combat Effects**: Sounds for spells hitting enemies, health potions being used, and enemies being defeated.

### Music
- **Dynamic Music**: Each dungeon has its own elemental theme, with music that adapts to the part of the map. For example, fast-paced music in boss fight combat and calmer tunes while exploring.

---

## 6. Technical Requirements
- **Game Engine**: Unity.
- **Platform**: PC.
- **Procedural Generation**: Dungeons are procedurally generated using Unity’s dungeon generation tools.

---

## 7. Level Design

### Procedural Generation
Each level is procedurally generated, ensuring a unique experience each time the player enters a dungeon. The dungeons will vary in size and structure but will always include:
- **Enemies**: Rooms filled with monsters that the player must defeat to progress.
- **Trap Corridors**: Sections with environmental hazards such as spikes, fire traps, or other element-based obstacles.
- **Loot**: Randomly generated power ups or other loot such as mana and health potions, or gear upgrades scattered around the dungeon, dropped by monsters or in chests.
- **Final Room**: Contains the dungeon's boss monster, guarding one of the stolen Elemelons.

### Key Locations
- **The Castle Hub**: (Royal Bedchamber or Throne Room) Where the player starts and returns after completing each dungeon. The King provides updates and storyline progression here. 
- **Elemental Dungeons**: Four dungeons, each themed around an element (Fire, Water, Earth, Wind), with unique visual aesthetics, enemies, and environmental hazards. All accessible from the start, but king recommends the easiest yet uncleared dungeon. Entrance accessible from castle hub. Blocked by obstacles like some ice or a big rock that can be removed for example with a bomb spell.
- **Boss Rooms**: In each dungeon, the player faces an elemental boss that represents the element of that dungeon (e.g., a fire-themed boss in the Fire Dungeon). The boss spawns with some helper monsters. 

### Pacing and Progression
- **Increasing Difficulty**: Each dungeon becomes progressively more challenging, with stronger enemies, more complex traps, and tougher bosses.
- **Exploration vs. Combat**: Balance between exploring the dungeon, solving environmental puzzles, and battling enemies.

---

## 8. User Interface (UI) and User Experience (UX)

### UI Design
- **HUD**: The Heads-Up Display will show the following:
  - **Hearts**: Displays the player’s remaining health.
  - **Mana Stars**: Displays the player’s mana, which regenerates over time.
  - **Elemental Slots**: Two icons showing the currently selected elemental charges.
  - **Spells**: spell currently selected, represented by a unique icon (Amanda: Idk about this one, Prefer to just show charges)
 
- **Dialogue UI**: Simple pixel art portraits of speaking characters will appear with text dialogs at the bottom for dialogue.
  
- **Menus**:
  -**Starting Menu**: Allows the player to continue the game or to start a new run by clicking "new game". Has fancy            background art.
  - **Settings Menu**: Allows the player to adjust sound, music, keybindings, and other game options.
  - **Spell Tome** Player can look up a spell's effect by pressing the keys to cast the spell he/she wants to select
### UX Considerations
- **Easy to Navigate**: The player can quickly the menu without disrupting the gameplay experience.
- **Element Selection**: Players can select elemental combinations using hotkeys (number keys for fire, water, earth, and wind), allowing for fluid spell-casting during combat. 
- **Feedback System**: Visual and audio feedback for spell effects, health, and mana status to help players make informed decisions in real time.

---

## 9. Appendix

### References
- Inspiration drawn from classic 2D pixel art dungeon crawlers like *The Binding of Isaac*, *Crawl* and *Enter the Gungeon*.
- Research into elemental spellcasting systems from *Magicka* and *Element TD*.
  
### Glossary
- **Elemelon**: Magical fruits representing elemental powers (Fire, Water, Earth, Wind).
- **Mana**: The resource used for casting spells, which regenerates over time.
- **Procedural Generation**: A method of creating dungeons that are randomized in structure and layout with each playthrough.

---

## Additional Notes
- **Iterative Design**: The game will be developed in phases with feedback loops for refinement after internal playtesting.
- **Expandability**: Potential future expansions could introduce new elements (e.g., lightning, ice), additional dungeons, and more complex spell combinations.
