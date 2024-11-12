
# Requirements Analysis Document for *The Elemelons*

## 1. Introduction

### 1.1 Purpose of the System
*The Elemelons* is a 2D dungeon crawler game where players explore elemental-themed dungeons, battle enemies, and retrieve magical artifacts called Elemelons.

### 1.2 Scope of the System
The game is designed for casual to midcore players who enjoy fantasy, dungeon crawlers, and pixel art. The player’s goal is to recover all four Elemelons and restore balance to the kingdom.

### 1.3 Objectives and Success Criteria
- Engaging elemental spell-casting mechanics
- Procedurally generated dungeons with unique elemental themes
- Fluid and strategic real-time combat
- Accessible user interface and HUD

### 1.4 Definitions, Acronyms, and Abbreviations
- **Player**: The main character controlled by the user, capable of casting and navigating dungeons.
- **Spell**: A magical ability created by combining elemental powers, used for combat or interacting with the environment.
- **Elements**: Basic building blocks for spells; includes Fire, Water, Earth, and Wind.
- **Enemy**: Non-playable characters that oppose the player, often with elemental strength.
- **Boss**: A powerful enemy encountered at the end of each dungeon, representing the dungeon's element and guarding an Elemelon.
- **Dungeons**: Procedurally generated environments where the player encounters enemies, solves puzzles, and collects items.
- **NPC (Non-Player Character)**: Characters like the King who interact with the player but are not controlled by the player.
- **Traps**:Environmental hazards that harm or hinder the player, such as spikes or fire pits.
- **Altar**: An object found in dungeons that restores the player's health when interacted with.
- **Health**: A resource representing the player's life, displayed as hearts in the HUD.

### 1.5 Overview
Players navigate elemental dungeons to retrieve the Elemelons, using unique spell combinations to overcome enemies and obstacles. Each dungeon features themed challenges, leading to a boss battle and the collection of an Elemelon.

---

## 2. Proposed System

### 2.1 Overview
The player takes on the role of a spellcaster tasked with exploring dungeons, using elemental combinations to cast spells, and defeating enemies to collect the Elemelons.

### 2.2 Functional Requirements

1. **Dungeon Exploration**: Procedurally generated dungeons with elemental themes.
2. **Elemental Spell-Casting**: Players combine elements to cast spells.
3. **Combat System**: Real-time combat using spells to exploit enemy weaknesses.
4. **Collecting Elemelons**: Players retrieve an Elemelon after each boss battle.
5. **Health and Mana Management**: HUD displays health and mana, which are consumed and replenished during gameplay.
6. **User Interface (UI) Navigation**: Players can access HUD, spell information, and settings through the menu.

### 2.3 Nonfunctional Requirements

1. **User Interface and Human Factors**: Intuitive and visually cohesive UI.
2. **Documentation**: In-game help for controls and spells.
3. **Hardware Considerations**: Compatible with mid-range gaming PCs.
4. **Performance Characteristics**: Target frame rate of 60 FPS.
5. **Error Handling**: Handles unexpected inputs gracefully.
6. **Quality Issues**: Minimize bugs and glitches.
7. **System Modifications**: Modular code structure.
8. **Physical Environment**: Designed for indoor play.
9. **Security Issues**: Save file integrity.
10. **Resource Issues**: Optimized CPU and GPU usage.

### 2.4 Pseudo Requirements
????????????????????????????????????

---

## 2.5 System Models

### 2.5.1 Scenarios

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

### 2.5.2 Use Case Model

1. **Starting a New Game**: Player selects "New Game" from the main menu.
2. **Basic Controls and Spell-Casting Tutorial**: Player learns movement, interaction, and spell-casting.
3. **Clearing Obstacles with Spells**: Player uses spells to remove obstacles.
4. **Accessing Menus**: Player can access settings, spell information, and HUD.

### 2.5.3 Object Model

- **Player**: Health, Mana, Inventory, Position.
- **Spell**: Element1, Element2, ManaCost, Effect.
- **Enemy**: Health, Type, Position.
- **Boss**: Health, Type, SpecialAbilities.
- **Dungeon**: Theme, Layout, Enemies, Traps.
- **King (NPC)**: Position, Dialogue.

### 2.5.4 Dynamic Models

1. **Spell-Casting Sequence**: Selecting elements, casting, and applying effects.
2. **Dungeon Exploration and Enemy Encounter**: Navigating, encountering, and battling enemies.
3. **Boss Battle Sequence**: Defeating the dungeon boss and obtaining the Elemelon.
4. **Returning Elemelons to the King**: Player returns Elemelon, receives a reward, and is prepared for further exploration.

### 2.5.5 User Interface - Navigational Paths and Screen Mock-Ups

- **Main Menu**: Continue, New Game, Settings, End Game.
- **In-Game HUD**: Displays health, mana, and elemental slots.
- **Pause Menu**: Access to settings, grimoire, and quit.
- **Settings Menu**: Adjust sound, controls, and visual effects.
- **Grimoire (Spellbook)**: Review spell combinations and effects.
- **Interaction with King**: Dialogue with king when returning Elemelons.

---

## 3. Glossary

1. **Elemelon**: Magical fruit representing one of the four elements.
2. **Mana**: Resource for casting spells.
3. **Procedural Generation**: Random generation of dungeon layouts.
4. **Elemental Spell**: Spell created by combining elements.
5. **Dungeon**: Game environment with themed enemies and puzzles.
6. **HUD**: Displays player stats like health, mana, and items.
7. **NPC**: Non-Player Character, such as the king or bosses.
8. **Real-Time Combat**: Combat requiring quick responses and strategies.
9. **Spell-Casting**: Creating and using spells with combined elements.
10. **Boss**: Powerful enemy guarding an Elemelon.


