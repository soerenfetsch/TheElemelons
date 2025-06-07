using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI icon bar for displaying active buffs and debuffs on the player.
/// Handles adding, removing, and updating buff/debuff icons, supporting duplicate instances.
/// </summary>
public class BuffDebuffIconBar : MonoBehaviour
{
    [SerializeField]
    private List<Image> buffIcons; // List of 8 Image components for the icon bar

    [SerializeField]
    private Sprite defaultSprite; // Default sprite when no buff/debuff is active

    [Serializable]
    public class BuffSpritePair
    {
        public BuffDebuffManager.BuffDebuffType type;
        public Sprite sprite;
    }

    [SerializeField]
    private List<BuffSpritePair> buffSpritePairs = new List<BuffSpritePair>();

    private Dictionary<BuffDebuffManager.BuffDebuffType, Sprite> buffSprites; // Map of BuffDebuffType to Sprite

    private List<BuffDebuffManager.BuffDebuffType> activeBuffs = new List<BuffDebuffManager.BuffDebuffType>();

    private void Awake()
    {
        // Initialize the dictionary from the List
        buffSprites = new Dictionary<BuffDebuffManager.BuffDebuffType, Sprite>();
        foreach (var pair in buffSpritePairs)
        {
            if (!buffSprites.ContainsKey(pair.type))
            {
                buffSprites[pair.type] = pair.sprite;
            }
        }
    }

    /// <summary>
    /// Adds a buff or debuff to the icon bar. Each buff instance is treated separately.
    /// </summary>
    public void AddBuff(BuffDebuffManager.BuffDebuffType buffType)
    {
        activeBuffs.Add(buffType);
    }

    /// <summary>
    /// Removes a buff or debuff from the icon bar. Removes the first occurrence of the buff.
    /// </summary>
    public void RemoveBuff(BuffDebuffManager.BuffDebuffType buffType)
    {
        if (activeBuffs.Contains(buffType))
        {
            activeBuffs.Remove(buffType);
        }
    }

    /// <summary>
    /// Updates the icon bar to display up to 10 active buffs/debuffs, including duplicates.
    /// </summary>
    public void UpdateIconBar()
    {
        int index = 0;

        // Display sprites for the first 10 active buffs/debuffs
        foreach (var buff in activeBuffs)
        {
            if (index >= buffIcons.Count)
                break;

            if (buffSprites.TryGetValue(buff, out Sprite sprite))
            {
                buffIcons[index].sprite = sprite;
                buffIcons[index].enabled = true;
            }
            else
            {
                Debug.LogWarning($"No sprite found for BuffDebuffType: {buff}");
                buffIcons[index].sprite = defaultSprite;
                buffIcons[index].enabled = true;
            }

            index++;
        }

        // Fill remaining slots with the default sprite
        for (; index < buffIcons.Count; index++)
        {
            buffIcons[index].sprite = defaultSprite;
            buffIcons[index].enabled = false;
        }
    }
}
