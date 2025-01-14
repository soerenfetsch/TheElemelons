using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// manage mana value of entity (used for spellcasting)
public class Mana : MonoBehaviour
{

    [Header("Mana Management")]
    bool canCast;
    public float maxMana = 5f;
    public float mana;
    public float manaPerRegen = 0.25f;
    public float manaRegenTime = 0.2f;
    public bool gameRunning = true;

    public float current_regenTime;
    public bool justCasted = false;
    // Start is called before the first frame update
    void Start()
    {
        //Assign variables
        current_regenTime = manaRegenTime;
        mana = maxMana;
        StartCoroutine(ManaRegen());
    }
    /// <summary>
    /// Handles Mana regeneration coroutine
    /// regens slower if mana was just exhausted
    /// </summary>
    public IEnumerator ManaRegen()
    {
        //manaregen coroutine
        //just a suggestion
        //later finetune, so that regen is slow i.e. if mana exhausted etc.
        while (gameRunning)
        {
            if (mana <= 0.25)
            {
                yield return new WaitForSeconds(current_regenTime * 2);
            }
            else if (justCasted)
            {
                justCasted = false;
                yield return new WaitForSeconds(current_regenTime);
            }
            yield return new WaitForSeconds(current_regenTime);
            mana = Mathf.Clamp(mana + manaPerRegen, 0f, maxMana);
        }
    }
    public void SubtractMana(float manaCost)
    {
        mana = Mathf.Clamp(mana - manaCost, 0f, maxMana);
    }

    public void AddMana(float plus)
    {
        mana = Mathf.Clamp(mana + plus, 0f, maxMana);
    }

    public float GetMana()
    {
        return mana;
    }
    public bool IsZero()
    {
        if (mana <= 0)
        {
            return true;
        }
        return false;
    }
    public float GetMaxMana()
    {
        return maxMana;
    }
    public void SetMana(float newMana)
    {
        mana = newMana;
    }
    public void SetMaxMana(float newMaxMana)
    {
        maxMana = newMaxMana;
    }
    public void SetManaPerRegen(float newManaPerRegen)
    {
        manaPerRegen = newManaPerRegen;
    }
}
