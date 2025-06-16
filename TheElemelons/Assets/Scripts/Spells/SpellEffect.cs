using UnityEngine;

/// <summary>
/// Abstract base class for all spells.
/// Provides common properties and methods for managing spell types and casting costs.
/// </summary>
public class SpellEffect : MonoBehaviour
{
    /// <summary>
    /// Mana cost required to cast the spell.
    /// </summary>
    public float manaCost;

    /// <summary>
    /// Type of the spell, represented by a <see cref="ChargeTuple"/>.
    /// </summary>
    public ChargeTuple spellType;

    /// <summary>
    /// The GameObject associated with the spell.
    /// </summary>
    public GameObject spellObject;
    public GameObject caster { get; set; } // Reference to the caster.

    /// <summary>
    /// Retrieves the mana cost of the spell.
    /// </summary>
    /// <returns>The mana cost of the spell.</returns>
    public float GetManaCost()
    {
        return manaCost;
    }
}
