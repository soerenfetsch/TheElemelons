using UnityEngine;


public interface IBuffable
{
    /// <summary>
    /// Modifies the speed of the combatant by a given factor.
    /// </summary>
    /// <param name="factor"></param>
    public void ModifySpeed(float factor);
    /// <summary>
    /// Modifies the damage of the combatant by a given factor.
    /// </summary>
    /// <param name="factor"></param>
    public void ModifyDamage(float factor);
    /// <summary>
    /// Modifies the defense of the combatant by a given factor.
    /// </summary>
    /// <param name="factor"></param>
    public void ModifyDefense(float factor);
    /// <summary>
    /// Modifies the mana regeneration of the combatant by a given factor.
    /// </summary> 
    /// <param name="factor"></param>
    public void ModifyManaRegen(float factor);
    public void ModifyIsHidden(float factor);
    public void ModifyIsConfused(float factor);
}