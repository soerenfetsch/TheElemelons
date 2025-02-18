using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the mana value of an entity, including regeneration and consumption for spellcasting.
/// </summary>
public class Mana : MonoBehaviour
{
    [Header("Mana Management")]
    /// <summary>
    /// Indicates whether the entity can cast spells.
    /// </summary>
    private bool canCast;

    /// <summary>
    /// The maximum mana value of the entity.
    /// </summary>
    public float maxMana = 5f;

    /// <summary>
    /// The current mana value of the entity.
    /// </summary>
    public float mana;

    /// <summary>
    /// The amount of mana regenerated per cycle.
    /// </summary>
    public float manaPerRegen = 0.25f;

    /// <summary>
    /// The time interval for mana regeneration.
    /// </summary>
    public float manaRegenTime = 0.2f;

    /// <summary>
    /// Indicates whether the game is running.
    /// </summary>
    public bool gameRunning = true;

    /// <summary>
    /// The current regeneration time interval.
    /// </summary>
    public float current_regenTime;

    /// <summary>
    /// Indicates whether mana was recently used.
    /// </summary>
    public bool justCasted = false;

    /// <summary>
    /// Unity Start method called before the first frame update.
    /// Initializes mana values and starts the regeneration coroutine.
    /// </summary>
    void Start()
    {
        current_regenTime = manaRegenTime;
        mana = maxMana;
        StartCoroutine(ManaRegen());
    }

    /// <summary>
    /// Coroutine for regenerating mana over time.
    /// Slows down regeneration when mana is exhausted or recently used.
    /// </summary>
    /// <returns>An enumerator for the coroutine.</returns>
    public IEnumerator ManaRegen()
    {
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

    /// <summary>
    /// Reduces the entity's mana by a specified amount, ensuring it does not drop below 0.
    /// </summary>
    /// <param name="manaCost">The amount of mana to subtract. Must be >= 0</param>
    public void SubtractMana(float manaCost)
    {
        if (manaCost < 0)
        {
            Debug.LogError("Mana cost must be greater than or equal to 0.");
            return;
        }
        mana = Mathf.Clamp(mana - manaCost, 0f, maxMana);
    }

    /// <summary>
    /// Increases the entity's mana by a specified amount, ensuring it does not exceed the maximum mana.
    /// </summary>
    /// <param name="plus">The amount of mana to add. Must be >= 0</param>
    public void AddMana(float plus)
    {
        if (plus < 0)
        {
            Debug.LogError("Plus must be greater than or equal to 0.");
            return;
        }
        mana = Mathf.Clamp(mana + plus, 0f, maxMana);
    }

    /// <summary>
    /// Retrieves the current mana value of the entity.
    /// </summary>
    /// <returns>The current mana value.</returns>
    public float GetMana()
    {
        return mana;
    }

    /// <summary>
    /// Checks if the entity's mana is zero or less.
    /// </summary>
    /// <returns>True if mana is zero or less; otherwise false.</returns>
    public bool IsZero()
    {
        return mana <= 0;
    }

    /// <summary>
    /// Retrieves the maximum mana value of the entity.
    /// </summary>
    /// <returns>The maximum mana value.</returns>
    public float GetMaxMana()
    {
        return maxMana;
    }

    /// <summary>
    /// Sets the current mana value of the entity.
    /// </summary>
    /// <param name="newMana">The new mana value to set. Must be >= 0</param>
    public void SetMana(float newMana)
    {
        if (newMana < 0)
        {
            Debug.LogError("Mana must be greater than or equal to 0.");
            return;
        }
        mana = newMana;
    }

    /// <summary>
    /// Sets the maximum mana value of the entity.
    /// </summary>
    /// <param name="newMaxMana">The new maximum mana value to set. Must be >= 0</param>
    public void SetMaxMana(float newMaxMana)
    {
        if (newMaxMana < 0)
        {
            Debug.LogError("Max mana must be greater than or equal to 0.");
            return;
        }
        maxMana = newMaxMana;
    }

    /// <summary>
    /// Sets the amount of mana regenerated per cycle.
    /// </summary>
    /// <param name="newManaPerRegen">The new mana regeneration amount. Must be >= 0</param>
    public void SetManaPerRegen(float newManaPerRegen)
    {
        if (newManaPerRegen < 0)
        {
            Debug.LogError("Mana per regen must be greater than or equal to 0.");
            return;
        }
        manaPerRegen = newManaPerRegen;
    }
}
