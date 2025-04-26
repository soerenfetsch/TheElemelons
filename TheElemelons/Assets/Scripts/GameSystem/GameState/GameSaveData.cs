using UnityEngine;
using System;

[System.Serializable]
public class GameSaveData
{
    public GameProgressionStage progressionStage;
    public int maxHealth;
    public int maxMana;
    public SerializableMapData mapData;
    public string saveFileName;
    public DateTime saveDate;
}