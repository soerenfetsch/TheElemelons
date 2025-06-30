using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component that displays active buffs and debuffs as icons.
/// Supports up to 10 simultaneous effects, including duplicates.
/// </summary>
public class BuffDebuffIconBar : MonoBehaviour
{
    /// <summary>
    /// List of Image components representing buff/debuff icon slots in the UI.
    /// </summary>
    [SerializeField]
    private List<Image> buffIcons; // List of 8 Image components for the icon bar

    /// <summary>
    /// Default sprite to display when no buff/debuff is active in a slot.
    /// </summary>
    [SerializeField]
    private Sprite defaultSprite; // Default sprite when no buff/debuff is active

    /// <summary>
    /// Serializable pair mapping a BuffDebuffType to its corresponding sprite.
    /// </summary>
    [Serializable]
    public class BuffSpritePair
    {
        /// <summary>
        /// The type of buff or debuff.
        /// </summary>
        public BuffDebuffManager.BuffDebuffType type;
        /// <summary>
        /// The sprite to display for this buff/debuff type.
        /// </summary>
        public Sprite sprite;
    }

    /// <summary>
    /// List of sprite pairs for mapping buff/debuff types to sprites.
    /// </summary>
    [SerializeField]
    private List<BuffSpritePair> buffSpritePairs = new List<BuffSpritePair>();

    // Dictionary mapping BuffDebuffType to its sprite for quick lookup.
    private Dictionary<BuffDebuffManager.BuffDebuffType, Sprite> buffSprites;

    // List of currently active buffs/debuffs (duplicates allowed).
    private List<BuffDebuffManager.BuffDebuffType> activeBuffs = new List<BuffDebuffManager.BuffDebuffType>();

    /// <summary>
    /// Unity Awake method. Initializes the buffSprites dictionary from the sprite pairs.
    /// </summary>
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
    /// <param name="buffType">The type of buff or debuff to add.</param>
    public void AddBuff(BuffDebuffManager.BuffDebuffType buffType)
    {
        activeBuffs.Add(buffType);
    }

    /// <summary>
    /// Removes a buff or debuff from the icon bar. Removes the first occurrence of the buff.
    /// </summary>
    /// <param name="buffType">The type of buff or debuff to remove.</param>
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

        // Display sprites for the first 10 active buffs/deebuffs
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

        // Fill remaining slots with the default sprite and disable them
        for (; index < buffIcons.Count; index++)
        {
            buffIcons[index].sprite = defaultSprite;
            buffIcons[index].enabled = false;
        }
    }
}
