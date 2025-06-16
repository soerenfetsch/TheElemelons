<<<<<<< Updated upstream
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for combatant entities that participate in combat.
/// Provides properties for attack/defense factors, elemental type, invincibility, and knockback handling.
=======
/// <summary>
/// Interface for combatant entities that participate in combat interactions.
/// Provides properties for attack/defense factors, invincibility, elemental type, and knockback handling.
>>>>>>> Stashed changes
/// </summary>
public interface ICombatant
{
    /// <summary>
<<<<<<< Updated upstream
    /// Gets or sets the attack factor of the combatant.
=======
    /// Gets or sets the attack factor multiplier for this combatant.
>>>>>>> Stashed changes
    /// </summary>
    float attackFactor { get; set; }

    /// <summary>
<<<<<<< Updated upstream
    /// Gets or sets the defense factor of the combatant.
=======
    /// Gets or sets the defense factor multiplier for this combatant.
>>>>>>> Stashed changes
    /// </summary>
    float defenseFactor { get; set; }

    /// <summary>
<<<<<<< Updated upstream
    /// Gets or sets whether the combatant is currently invincible.
=======
    /// Gets or sets whether this combatant is currently invincible.
>>>>>>> Stashed changes
    /// </summary>
    bool isInvincible { get; set; }

    /// <summary>
<<<<<<< Updated upstream
    /// Gets or sets the elemental type of the combatant.
=======
    /// Gets or sets the elemental type of this combatant.
>>>>>>> Stashed changes
    /// </summary>
    ElementType elementType { get; set; }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the knockback effect for the combatant.
=======
    /// Called when the combatant is knocked back.
>>>>>>> Stashed changes
    /// </summary>
    /// <param name="knockTime">The duration of the knockback effect.</param>
    void GetKnocked(float knockTime);
}
