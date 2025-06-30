/// <summary>
/// Interface for entities that can receive buffs and debuffs affecting their stats or status.
/// Implement this interface to allow <see cref="BuffDebuffManager"/> to modify speed, damage, defense, mana regen, and special states.
/// </summary>
public interface IBuffable
{
    /// <summary>
    /// Modifies the speed of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the speed by.</param>
    void ModifySpeed(float factor);

    /// <summary>
    /// Modifies the damage of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the damage by.</param>
    void ModifyDamage(float factor);

    /// <summary>
    /// Modifies the defense of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the defense by.</param>
    void ModifyDefense(float factor);

    /// <summary>
    /// Modifies the mana regeneration of the combatant by a given factor.
    /// </summary>
    /// <param name="factor">The factor to multiply the mana regeneration by.</param>
    void ModifyManaRegen(float factor);

    /// <summary>
    /// Modifies the hidden state of the combatant.
    /// </summary>
    /// <param name="factor">1 to set hidden, 0 to unset.</param>
    void ModifyIsHidden(float factor);

    /// <summary>
    /// Modifies the confused state of the combatant.
    /// </summary>
    /// <param name="factor">1 to set confused, 0 to unset.</param>
    void ModifyIsConfused(float factor);

    /// <summary>
    /// Modifies the frozen state of the combatant.
    /// </summary>
    /// <param name="factor">1 to set frozen, 0 to unset.</param>
    void ModifyIsFrozen(float factor);
}