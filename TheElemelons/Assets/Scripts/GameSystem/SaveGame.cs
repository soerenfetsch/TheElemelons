using UnityEngine;
using System;

[Serializable]
public class SaveGame // ✅ Renamed for clarity
{
    public string version; // Added version tracking
    public int currentLevel;
    public bool[] completedDungeons;
    public bool[] completedDialogues;
    public float maxHealth;
    public float maxMana;

    public SaveGame()
    {
        ResetToDefaults();
    }

    public void ResetToDefaults()
    {
        version = Application.version;
        currentLevel = 1;
        completedDungeons = new bool[10];
        completedDialogues = new bool[10];
        maxHealth = 100f;
        maxMana = 50f;
    }
}
