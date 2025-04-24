using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BuffData
{
    public string targetName;
    public string name;
    public int type;  // Stored as int to be cast to BuffDebuffType enum
    public float remainingDuration;
    public float effectValue;
    public bool stackable;
}

[System.Serializable]
public class BuffsData
{
    public List<BuffData> activeBuffs = new List<BuffData>();
}

[System.Serializable]
public class PlayerData
{
    public Vector2 position;
    public float currentHealth;
    public float currentMana;
    public float attackFactor = 1f;
    public float defenseFactor = 1f;
    public int elementType = 0;  // Stored as int to be cast to ElementType enum
}