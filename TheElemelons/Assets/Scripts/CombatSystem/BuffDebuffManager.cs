using System;
using System.Collections.Generic;
using UnityEngine;


public class BuffDebuffManager : MonoBehaviour
{
    // Singleton instance
    public static BuffDebuffManager Instance { get; private set; }
    private UIManager _uiManager;

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
        ManaRegenDebuff
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

        public BuffDebuff(BuffDebuffType type, float duration, float effectValue)
        {
            this.type = type;
            this.duration = duration;
            this.effectValue = effectValue;
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
        public void ApplyEffect(BuffDebuff effect)
        {
            Debug.Log($"Applying {effect.type} effect to target {Name}.");
            ActiveEffects.Add(effect);
            effect.OnApply?.Invoke(effect);
        }

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
    /// <param name="duration">Duration of the effect.</param>
    /// <param name="effectValue">Value of the effect (e.g., damage factor).</param>
    public void ApplyBuffDebuff(GameObject target, BuffDebuffType effectType, float duration, float effectValue)
    {
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
                _uiManager?.AddBuffDebuffIcon(effect.type);
                // Start the speed buff animation if buff is speed buff
                if (target.CompareTag("Player") && effectType == BuffDebuffType.SpeedBuff)
                {
                    _uiManager?.SetSpeedBuffAnimation(true);
                }
            },
            OnExpire = effect =>
            {
                // Remove buff/debuff icon from UI
                _uiManager?.RemoveBuffDebuffIcon(effect.type);
                // Only stop the speed buff animation if there is no other speed buff active
                if (target.CompareTag("Player") && effectType == BuffDebuffType.SpeedBuff
                    && !entity.ActiveEffects.Exists(e => e.type == BuffDebuffType.SpeedBuff))
                {
                    _uiManager?.SetSpeedBuffAnimation(false);
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

        if (targets.ContainsKey(target))
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
            foreach (var effect in entity.ActiveEffects)
            {
                effect.OnExpire?.Invoke(effect);
            }

            entity.ActiveEffects.Clear();
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
