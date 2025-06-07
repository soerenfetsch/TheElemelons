using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for combatant entities that participate in combat.
/// Provides properties for attack/defense factors, elemental type, invincibility, and knockback handling.
/// </summary>
public interface ICombatant
{
    /// <summary>
    /// Gets or sets the attack factor of the combatant.
    /// </summary>
    float attackFactor { get; set; }

    /// <summary>
    /// Gets or sets the defense factor of the combatant.
    /// </summary>
    float defenseFactor { get; set; }

    /// <summary>
    /// Gets or sets whether the combatant is currently invincible.
    /// </summary>
    bool isInvincible { get; set; }

    /// <summary>
    /// Gets or sets the elemental type of the combatant.
    /// </summary>
    ElementType elementType { get; set; }

    /// <summary>
    /// Handles the knockback effect for the combatant.
    /// </summary>
    /// <param name="knockTime">The duration of the knockback effect.</param>
    void GetKnocked(float knockTime);
}
