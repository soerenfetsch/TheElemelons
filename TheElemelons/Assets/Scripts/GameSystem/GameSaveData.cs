using System;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int currentLevel; // Current level or dungeon
    public bool[] completedDungeons; // Dungeons completed
    public bool[] completedBossFights; // Boss fights completed
    public bool[] completedDialogues; // Dialogues completed
    public float maxHealth; // Player's maximum health
    public float maxMana; // Player's maximum mana
}
