using System;
using System.Collections.Generic;
using UnityEngine;


public class BuffDebuffManager : MonoBehaviour
{
    // Singleton instance
    public static BuffDebuffManager Instance { get; private set; }
    private UIManager _uiManager;
    private BuffDebuffManager() { } // Prevent instantiation

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Enforce singleton
        }

        // Cache UIManager reference
        _uiManager = FindObjectOfType<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogWarning("UIManager not found in the scene. Ensure there is a UIManager component.");
        }
    }

    // Buff/Debuff types
    public enum BuffDebuffType
    {
        SpeedBuff,
        DamageBuff,
        DefenseBuff,
        ManaRegenBuff,
        SlownessDebuff,
        WeaknessDebuff,
        FragilityDebuff,
        ManaRegenDebuff,
        HiddenBuff,
        ConfusedDebuff,
        FreezeDebuff,
    }

    // Buff/Debuff data structure
    [Serializable]
    public class BuffDebuff
    {
        public BuffDebuffType type;
        public float duration;
        public float effectValue;

        public Action<BuffDebuff> OnApply;
        public Action<BuffDebuff> OnExpire;
        public string name = "none";
        public bool stackable = true;

        public BuffDebuff(BuffDebuffType type, float duration, float effectValue)
        {
            this.type = type;
            this.duration = duration;
            this.effectValue = effectValue;
        }
        public BuffDebuff(string name, BuffDebuffType type, float duration, float effectValue, bool stackable)
        {
            this.type = type;
            this.duration = duration;
            this.effectValue = effectValue;
            this.name = name;
            this.stackable = stackable;
        }
    }

    // Target entity structure
    public class TargetEntity
    {
        public string Name { get; private set; } // Name of the target
        public List<BuffDebuff> ActiveEffects = new List<BuffDebuff>();

        public TargetEntity(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Applies a buff or debuff effect to the target.
        /// </summary>
        /// <param name="effect">The buff or debuff effect to apply.</param>
        public void ApplyEffect(BuffDebuff effect)
        {
            Debug.Log($"Applying {effect.type} effect to target {Name}.");
            ActiveEffects.Add(effect);
            effect.OnApply?.Invoke(effect);
        }

        /// <summary>
        /// Removes a buff or debuff effect from the target.
        /// </summary>
        /// <param name="effect">The buff or debuff effect to remove.</param>
        public void RemoveEffect(BuffDebuff effect)
        {
            Debug.Log($"Removing {effect.type} effect from target {Name}.");
            ActiveEffects.Remove(effect);
            effect.OnExpire?.Invoke(effect);
        }
    }

    private Dictionary<GameObject, TargetEntity> targets = new Dictionary<GameObject, TargetEntity>();

    /// <summary>
    /// Apply a buff or debuff to a target.
    /// </summary>
    /// <param name="target">The target GameObject.</param>
    /// <param name="effectType">Type of buff or debuff.</param>
    /// <param name="duration">Duration of the effect. Must be greater than 0.</param>
    /// <param name="effectValue">Value of the effect as a factor. Must be greater or equal 1.</param>
    public void ApplyBuffDebuff(GameObject target, BuffDebuffType effectType, float duration, float effectValue)
    {
        // Assert that duration is greater than 0 and effect value is greater than or equal to 1
        if (duration <= 0)
        {
            Debug.LogWarning("Duration must be greater than 0.");
            return;
        }
        if (effectValue < 1)
        {
            Debug.LogWarning("Effect value must be greater than or equal to 1.");
            return;
        }

        if (!targets.ContainsKey(target))
        {
            targets[target] = new TargetEntity(target.name);
        }

        TargetEntity entity = targets[target];

        BuffDebuff effect = new BuffDebuff(effectType, duration, effectValue)
        {
            OnApply = effect =>
            {
                // Add buff/debuff icon to UI
                if (target.CompareTag("Player"))
                {
                    _uiManager?.AddBuffDebuffIcon(effect.type);
                }

                // Start the speed buff animation if buff is speed buff
                if (target.CompareTag("Player") && effectType == BuffDebuffType.SpeedBuff)
                {
                    _uiManager?.SetSpeedBuffAnimation(true);
                }
                else if (effectType == BuffDebuffType.ConfusedDebuff)
                {
                    _uiManager?.SetConfusedEffect(true, target);
                }
                else if (effectType == BuffDebuffType.FreezeDebuff)
                {
                    _uiManager?.SetFreezeEffect(true, target);
                }
            },
            OnExpire = effect =>
            {
                // Remove buff/debuff icon from UI
                if (target.CompareTag("Player"))
                {
                    _uiManager?.RemoveBuffDebuffIcon(effect.type);
                }

                // Only stop the speed buff animation if there is no other speed buff active
                if (target.CompareTag("Player") && effectType == BuffDebuffType.SpeedBuff
                    && !entity.ActiveEffects.Exists(e => e.type == BuffDebuffType.SpeedBuff))
                {
                    _uiManager?.SetSpeedBuffAnimation(false);
                }
                else if (effectType == BuffDebuffType.ConfusedDebuff)
                {

                    Debug.Log("calling ui manager to remove question mark");
                    _uiManager?.SetConfusedEffect(false, target);
                }
                else if (effectType == BuffDebuffType.FreezeDebuff)
                {
                    _uiManager?.SetFreezeEffect(false, target);
                }
            }
        };

        // TODO: Add more OnApply and OnExpire delegates for other effects

        // Apply effect to target stats
        ApplyEffectToStats(target, effect, true);

        entity.ApplyEffect(effect);

        // Start the timer to remove the buff/debuff after its duration
        StartCoroutine(EffectTimer(target, effect));
    }
    public void ApplyBuffDebuff(GameObject target, string buffName, BuffDebuffType effectType, float duration, float effectValue, bool stackable)
    {
        // If not stackable and effect already active, stop old effect before starting new
        // Essentially resets timer
        if (!stackable)
        {
            List<BuffDebuff> buffList = Instance.GetActiveEffects(target);
            List<int> doubles_indexes = new List<int> { };
            for (int i = 0; i < buffList.Count; i++)
            {
                if (buffList[i] != null && buffName != "none" && buffList[i].name == buffName)
                {
                    doubles_indexes.Add(i);
                }
            }
            foreach (int i in doubles_indexes)
            {
                Debug.Log("Removing " + i.ToString());
                if (targets.ContainsKey(target))
                {
                    BuffDebuff doubleBuffDebuff = buffList[i];
                    targets[target].RemoveEffect(doubleBuffDebuff);
                    ApplyEffectToStats(target, doubleBuffDebuff, false);
                }
            }
        }
        // Assert that duration is greater than 0 and effect value is greater than or equal to 1
        if (duration <= 0)
        {
            Debug.LogWarning("Duration must be greater than 0.");
            return;
        }
        if (effectValue < 1)
        {
            Debug.LogWarning("Effect value must be greater than or equal to 1.");
            return;
        }

        if (!targets.ContainsKey(target))
        {
            targets[target] = new TargetEntity(target.name);
        }

        TargetEntity entity = targets[target];
        BuffDebuff effect = new BuffDebuff(buffName, effectType, duration, effectValue, stackable)
        {
            OnApply = effect =>
            {
                // Add buff/debuff icon to UI
                if (target.CompareTag("Player"))
                {
                    _uiManager?.AddBuffDebuffIcon(effect.type);
                }

                // Start the speed buff animation if buff is speed buff
                if (target.CompareTag("Player") && effectType == BuffDebuffType.SpeedBuff)
                {
                    _uiManager?.SetSpeedBuffAnimation(true);
                }
                else if (effectType == BuffDebuffType.ConfusedDebuff)
                {
                    Debug.Log("calling ui manager to apply question mark");
                    _uiManager?.SetConfusedEffect(true, target);
                }
                else if (effectType == BuffDebuffType.FreezeDebuff)
                {
                    _uiManager?.SetFreezeEffect(true, target);
                }
            },
            OnExpire = effect =>
            {
                // Remove buff/debuff icon from UI
                if (target.CompareTag("Player"))
                {
                    _uiManager?.RemoveBuffDebuffIcon(effect.type);
                }

                // Only stop the speed buff animation if there is no other speed buff active
                if (target.CompareTag("Player") && effectType == BuffDebuffType.SpeedBuff
                    && !entity.ActiveEffects.Exists(e => e.type == BuffDebuffType.SpeedBuff))
                {
                    _uiManager?.SetSpeedBuffAnimation(false);
                }
                else if (effectType == BuffDebuffType.ConfusedDebuff)
                {

                    Debug.Log("calling ui manager to remove question mark");
                    _uiManager?.SetConfusedEffect(false, target);
                }
                else if (effectType == BuffDebuffType.FreezeDebuff)
                {
                    _uiManager?.SetFreezeEffect(false, target);
                }
            }
        };

        // TODO: Add more OnApply and OnExpire delegates for other effects

        // Apply effect to target stats
        ApplyEffectToStats(target, effect, true);

        entity.ApplyEffect(effect);

        // Start the timer to remove the buff/debuff after its duration
        StartCoroutine(EffectTimer(target, effect));
    }
    private void ApplyEffectToStats(GameObject target, BuffDebuff effect, bool isApplying)
    {
        var buffable = target.GetComponent<IBuffable>();

        if (buffable == null)
        {
            Debug.LogWarning($"No IBuffable component found on {target.name}");
            return;
        }

        // Check that effect factor is greater or equal to 1
        if (effect.effectValue < 1)
        {
            Debug.LogWarning($"Effect value must be greater or equal 1. Effect: {effect.type}");
            return;
        }
        // Determine effect direction
        float factor = isApplying ? effect.effectValue : 1 / effect.effectValue;

        switch (effect.type)
        {
            case BuffDebuffType.SpeedBuff:
                buffable.ModifySpeed(factor);
                break;

            case BuffDebuffType.DamageBuff:
                buffable.ModifyDamage(factor);
                break;

            case BuffDebuffType.DefenseBuff:
                buffable.ModifyDefense(factor);
                break;

            case BuffDebuffType.ManaRegenBuff:
                buffable.ModifyManaRegen(factor);
                break;

            case BuffDebuffType.SlownessDebuff:
                buffable.ModifySpeed(1 / factor); // Decrease speed
                break;

            case BuffDebuffType.WeaknessDebuff:
                buffable.ModifyDamage(1 / factor); // Decrease damage
                break;

            case BuffDebuffType.FragilityDebuff:
                buffable.ModifyDefense(1 / factor); // Decrease defense
                break;

            case BuffDebuffType.ManaRegenDebuff:
                buffable.ModifyManaRegen(1 / factor); // Decrease mana regen
                break;
            case BuffDebuffType.HiddenBuff:
                factor = isApplying ? 1 : 0;
                buffable.ModifyIsHidden(factor);
                break;
            case BuffDebuffType.ConfusedDebuff:
                factor = isApplying ? 1 : 0;
                buffable.ModifyIsConfused(factor);
                break;
            case BuffDebuffType.FreezeDebuff:
                factor = isApplying ? 1 : 0;
                buffable.ModifyIsFrozen(factor);
                break;

            default:
                Debug.LogWarning($"Unhandled Buff/Debuff type: {effect.type}");
                break;
        }
    }

    /// <summary>
    /// Coroutine to manage effect duration.
    /// </summary>
    private System.Collections.IEnumerator EffectTimer(GameObject target, BuffDebuff effect)
    {
        yield return new WaitForSeconds(effect.duration);

        // Check that the target still exists and the effect is still active
        if (targets.ContainsKey(target) && !ReferenceEquals(target, null) && Instance.GetActiveEffects(target).Contains(effect))
        {
            targets[target].RemoveEffect(effect);
            ApplyEffectToStats(target, effect, false);
        }
    }

    /// <summary>
    /// Clear all buffs and debuffs from a target.
    /// </summary>
    /// <param name="target">The target GameObject.</param>
    public void ClearEffects(GameObject target)
    {
        if (targets.ContainsKey(target))
        {
            TargetEntity entity = targets[target];
            // Cannot directly remove from the list while iterating
            // so we need to create a copy of the list

            foreach (var effect in new List<BuffDebuff>(GetActiveEffects(target)))
            {
                entity.RemoveEffect(effect);
                ApplyEffectToStats(target, effect, false);
            }
        }
    }
    public void ClearEffect(GameObject target, BuffDebuffType type, string effectName = "")
    {
        bool lookForName = effectName != "";
        if (targets.ContainsKey(target))
        {
            TargetEntity entity = targets[target];
            // Cannot directly remove from the list while iterating
            // so we need to create a copy of the list
            foreach (var effect in new List<BuffDebuff>(GetActiveEffects(target)))
            {
                if (effect.type == type || lookForName && effect.name == effectName)
                {
                    entity.RemoveEffect(effect);
                    ApplyEffectToStats(target, effect, false);
                }
            }
        }
    }

    /// <summary>
    /// Get the active buffs and debuffs for a target.
    /// </summary>
    public List<BuffDebuff> GetActiveEffects(GameObject target)
    {
        if (targets.ContainsKey(target))
        {
            return targets[target].ActiveEffects;
        }

        return new List<BuffDebuff>();
    }

}
