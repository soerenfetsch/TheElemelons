
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
- **Elemelon**: Magical fruits representing elemental powers (Fire, Water, Earth, Wind).
- **Mana**: Resource used for casting spells, which regenerates over time.
- **HUD**: Heads-Up Display, shows health, mana, equipped elements, and items.

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
1. **Technology Choice**: Unity (2022.3.50f1).
2. **Coding Standards**: Follow established C# conventions.
3. **Version Control**: Use Git with a structured branching strategy.
4. **Regular Backups**: Maintain backups of project files and documentation.

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
8. **Earth Dungeon Boss Battle: The Golem**
9. **Collecting the Earth Elemelon and Returning to the King**
10. **Exploring Other Elemental Dungeons**
11. **Final Retrieval of All Four Elemelons**
12. **Entering the Final Battle**

### 2.5.2 Use Case Model

1. **Starting a New Game**: Player selects "New Game" from the main menu.
2. **Receiving the Mission**: The king provides the mission to retrieve Elemelons.
3. **Basic Controls and Spell-Casting Tutorial**: Player learns movement, interaction, and spell-casting.
4. **Clearing Obstacles with Spells**: Player uses spells to remove obstacles.
5. **Dungeon Exploration**: Player navigates procedurally generated dungeons.
6. **Combat with Elemental Enemies**: Player uses elemental spells strategically.
7. **Boss Battle in the Dungeon**: Player battles the boss to retrieve the Elemelon.
8. **Collecting and Returning Elemelons**: Player returns Elemelons to the king for rewards.
9. **Accessing Menus**: Player can access settings, spell information, and HUD.
10. **Final Battle Setup**: Player prepares to confront the evil witch.

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


