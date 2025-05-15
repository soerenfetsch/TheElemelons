using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class KeyBindingEntry
{
    public string action;
    public int keyCode; // store as int for KeyCode
}

[System.Serializable]
public class GameSaveData
{
    public GameProgressionStage progressionStage;
    public int maxHealth = 3;
    public int maxMana = 3;
    public DateTime saveDate;

    public List<KeyBindingEntry> keyBindings = new List<KeyBindingEntry>(); // ADD THIS
}