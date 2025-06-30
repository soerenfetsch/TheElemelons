/// <summary>
/// Interface for combatant entities that participate in combat interactions.
/// Provides properties for attack/defense factors, invincibility, elemental type, and knockback handling.
/// </summary>
public interface ICombatant
{
    /// <summary>
    /// Gets or sets the attack factor multiplier for this combatant.
    /// </summary>
    float attackFactor { get; set; }

    /// <summary>
    /// Gets or sets the defense factor multiplier for this combatant.
    /// </summary>
    float defenseFactor { get; set; }

    /// <summary>
    /// Gets or sets whether this combatant is currently invincible.
    /// </summary>
    bool isInvincible { get; set; }

    /// <summary>
    /// Gets or sets the elemental type of this combatant.
    /// </summary>
    ElementType elementType { get; set; }

    /// <summary>
    /// Called when the combatant is knocked back.
    /// </summary>
    /// <param name="knockTime">The duration of the knockback effect.</param>
    void GetKnocked(float knockTime);
}
