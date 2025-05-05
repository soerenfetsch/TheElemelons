using UnityEngine;
using System;

[System.Serializable]
public class GameSaveData
{
    public GameProgressionStage progressionStage;
    public int maxHealth = 3;
    public int maxMana = 3;
    // public SerializableMapData mapData;
    public DateTime saveDate;
}