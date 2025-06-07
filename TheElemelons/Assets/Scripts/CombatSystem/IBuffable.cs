using UnityEngine;

/// <summary>
/// Interface for combatants that can be affected by buffs and debuffs.
/// Provides methods to modify speed, damage, defense, mana regeneration, and status effects.
/// </summary>
public interface IBuffable
{
    /// <summary>
    /// Modifies the speed of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to modify speed by.</param>
    public void ModifySpeed(float factor);

    /// <summary>
    /// Modifies the damage of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to modify damage by.</param>
    public void ModifyDamage(float factor);

    /// <summary>
    /// Modifies the defense of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to modify defense by.</param>
    public void ModifyDefense(float factor);

    /// <summary>
    /// Modifies the mana regeneration of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to modify mana regeneration by.</param>
    public void ModifyManaRegen(float factor);

    /// <summary>
    /// Modifies the hidden status of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to modify hidden status by.</param>
    public void ModifyIsHidden(float factor);

    /// <summary>
    /// Modifies the confused status of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to modify confused status by.</param>
    public void ModifyIsConfused(float factor);

    /// <summary>
    /// Modifies the frozen status of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to modify frozen status by.</param>
    public void ModifyIsFrozen(float factor);
}