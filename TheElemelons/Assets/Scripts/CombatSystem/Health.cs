using UnityEngine;

/// <summary>
/// Manages the health value of an entity, including damage, healing, and health limits.
/// </summary>
public class Health : MonoBehaviour
{
    /// <summary>
    /// The maximum health value of the entity.
    /// </summary>
    public float maxhealth = 3;

    /// <summary>
    /// The current health value of the entity.
    /// </summary>
    public float health;

    /// <summary>
    /// Initializes the health to the maximum value.
    /// </summary>
    protected void Start()
    {
        health = maxhealth;
    }

    /// <summary>
    /// Reduces the entity's health by a specified amount, ensuring it does not drop below 0.
    /// </summary>
    /// <param name="damage">The amount of damage to subtract from health. Must be >= 0</param>
    public void SubtractHealth(float damage)
    {
        if (damage < 0)
        {
            Debug.LogError("Damage must be greater than or equal to 0.");
            return;
        }
        health = Mathf.Clamp(health - damage, 0f, maxhealth);
    }

    /// <summary>
    /// Checks if the entity's health is zero or less.
    /// </summary>
    /// <returns>True if health is zero or less; otherwise false.</returns>
    public bool IsZero()
    {
        return health <= 0;
    }

    /// <summary>
    /// Increases the entity's health by a specified amount, ensuring it does not exceed the maximum health.
    /// </summary>
    /// <param name="plus">The amount of health to add. Must be >= 0</param>
    public void AddHealth(float plus)
    {
        if (plus < 0)
        {
            Debug.LogError("Plus must be greater than or equal to 0.");
            return;
        }
        health = Mathf.Clamp(health + plus, 0f, maxhealth);
    }

    /// <summary>
    /// Retrieves the maximum health value of the entity.
    /// </summary>
    /// <returns>The maximum health value.</returns>
    public float GetMaxHealth()
    {
        return maxhealth;
    }

    /// <summary>
    /// Retrieves the current health value of the entity.
    /// </summary>
    /// <returns>The current health value.</returns>
    public float GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Sets the current health value of the entity.
    /// </summary>
    /// <param name="newHealth">The new health value to set. Must be >= 0.</param>
    public void SetHealth(float newHealth)
    {
        if (newHealth < 0)
        {
            Debug.LogError("Health must be greater than or equal to 0.");
            return;
        }
        health = newHealth;
    }

    /// <summary>
    /// Sets the maximum health value of the entity.
    /// </summary>
    /// <param name="newMaxHealth">The new maximum health value to set. Must be >= 0.</param>
    public void SetMaxHealth(float newMaxHealth)
    {
        if (newMaxHealth < 0)
        {
            Debug.LogError("MaxHealth must be greater than or equal to 0.");
            return;
        }
        maxhealth = newMaxHealth;
    }
}
