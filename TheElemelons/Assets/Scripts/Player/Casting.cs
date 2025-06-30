using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum representing types of elemental charges.
/// </summary>
public enum ChargeTypes
{
    fire,
    water,
    earth,
    wind,
    notype
}

/// <summary>
/// Handles the player's spell-casting mechanics, including charge management,
/// spell selection, and mana consumption.
/// </summary>
public class Casting : MonoBehaviour
{
    /// <summary>
    /// The point from which spells are cast.
    /// </summary>
    public Transform castpoint;

    /// <summary>
    /// Animator for managing wand animations.
    /// </summary>
    public Animator wandAnimator;

    [Header("Mana Management")]
    /// <summary>
    /// Reference to the player's mana script.
    /// </summary>
    public Mana manaScript;

    [Header("Charges")]
    /// <summary>
    /// Maximum number of charges the player can hold.
    /// </summary>
    public int maxCharges = 2;

    /// <summary>
    /// The current tuple of selected charges.
    /// </summary>
    public ChargeTuple currentCharges = new ChargeTuple();

    [Header("Spell Selection")]
    /// <summary>
    /// Spell prefab for no-type (empty) charge.
    /// </summary>
    public GameObject noTypeSpell;
    /// <summary>
    /// Spell prefab for wind.
    /// </summary>
    public GameObject windSpell;
    /// <summary>
    /// Spell prefab for fire-fire.
    /// </summary>
    public GameObject firefireSpell;
    /// <summary>
    /// Spell prefab for fire-earth.
    /// </summary>
    public GameObject fireearthSpell;
    /// <summary>
    /// Spell prefab for earth-wind.
    /// </summary>
    public GameObject earthwindSpell;
    /// <summary>
    /// Spell prefab for fire-wind.
    /// </summary>
    public GameObject firewindSpell;
    /// <summary>
    /// Spell prefab for earth-water.
    /// </summary>
    public GameObject earthwaterSpell;
    /// <summary>
    /// Spell prefab for earth (single).
    /// </summary>
    public GameObject earthSpell;
    /// <summary>
    /// Spell prefab for earth-earth.
    /// </summary>
    public GameObject earthearthSpell;
    /// <summary>
    /// Spell prefab for wind-wind.
    /// </summary>
    public GameObject windwindSpell;
    /// <summary>
    /// Spell prefab for water (single).
    /// </summary>
    public GameObject waterSpell;
    /// <summary>
    /// Spell prefab for water-wind.
    /// </summary>
    public GameObject waterWindSpell;
    /// <summary>
    /// Spell prefab for water-water.
    /// </summary>
    public GameObject waterWaterSpell;
    /// <summary>
    /// Spell prefab for water-fire.
    /// </summary>
    public GameObject waterFireSpell;
    /// <summary>
    /// Spell prefab for fire (single).
    /// </summary>
    public GameObject fireSpell;

    /// <summary>
    /// Mana cost for the current spell.
    /// </summary>
    public float manaCost = 1f;

    /// <summary>
    /// Maps charge combinations to corresponding spell effects.
    /// </summary>
    public Dictionary<ChargeTuple, SpellEffect> chargeToSpell = new Dictionary<ChargeTuple, SpellEffect>();

    [Header("Internal Stuff")]
    /// <summary>
    /// Reference to the player's movement script.
    /// </summary>
    public PlayerMovement playerMovement;

    // Reference to the Player component.
    private Player player;

    /// <summary>
    /// Initializes spell dictionary and component references.
    /// </summary>
    private void Start()
    {
        // Initializes variables and setups.
        player = gameObject.GetComponent<Player>();
        manaScript = gameObject.GetComponent<Mana>();
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype), noTypeSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.wind, ChargeTypes.notype), windSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.fire, ChargeTypes.fire), firefireSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.fire, ChargeTypes.earth), fireearthSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.wind, ChargeTypes.earth), earthwindSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.wind, ChargeTypes.fire), firewindSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.water, ChargeTypes.earth), earthwaterSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.notype, ChargeTypes.earth), earthSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.earth, ChargeTypes.earth), earthearthSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.wind, ChargeTypes.wind), windwindSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.water, ChargeTypes.notype), waterSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.water, ChargeTypes.wind), waterWindSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.water, ChargeTypes.water), waterWaterSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.water, ChargeTypes.fire), waterFireSpell.GetComponent<SpellEffect>());
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.fire, ChargeTypes.notype), fireSpell.GetComponent<SpellEffect>());
    }

    /// <summary>
    /// Determines the spell associated with the current charges.
    /// </summary>
    /// <returns>The spell GameObject, or null if no spell exists.</returns>
    public GameObject determineSpell()
    {
        if (chargeToSpell.ContainsKey(currentCharges))
        {
            return chargeToSpell[currentCharges].spellObject;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if the player can cast the selected spell.
    /// </summary>
    /// <returns>True if casting is possible; otherwise false.</returns>
    public bool CanCast()
    {
        if (!chargeToSpell.ContainsKey(currentCharges) || player.GetCurrentState() == PlayerState.Stagger)
        {
            return false;
        }
        manaCost = chargeToSpell[currentCharges].manaCost;
        return manaScript.mana >= manaCost;
    }

    /// <summary>
    /// Unity Update method. Handles wand animation state.
    /// </summary>
    private void Update()
    {
        // Handles wand animations.
        if (wandAnimator.GetBool("Charge"))
        {
            StartCoroutine(RemoveWand());
        }
    }

    /// <summary>
    /// Adds a selected charge to the current tuple and updates UI.
    /// </summary>
    /// <param name="selectedCharge">The charge type to add.</param>
    public void OnSelectCharge(ChargeTypes selectedCharge)
    {
        if (currentCharges.Count() < maxCharges)
        {
            currentCharges.Add(selectedCharge);
            wandAnimator.SetBool("Charge", true);
            UIManager.Instance.loadChargeSprite(selectedCharge);
        }
    }

    // Handles wand animation timing for removing charge visual.
    private IEnumerator RemoveWand()
    {
        yield return new WaitForSeconds(0.5f);
        wandAnimator.SetBool("Charge", false);
    }

    /// <summary>
    /// Casts the spell corresponding to the current charges.
    /// </summary>
    public void CastSpell()
    {
        GameObject newSpell = determineSpell();
        wandAnimator.SetBool("RaiseWand", true);

        if (CanCast() && newSpell != null)
        {
            wandAnimator.SetBool("CanCast", true);
            manaScript.SubtractMana(manaCost);
            CombatManager.InstantiateSpell(newSpell, gameObject, castpoint.position, Quaternion.Euler(0, 0, 0));
            // Clear charge tuple and corresponding UI
            ClearCharges();
            UIManager.Instance.ClearChargeSprites();
            manaScript.justCasted = true;
        }
        else
        {
            wandAnimator.SetBool("CanCast", false);
        }

        StartCoroutine(StopCastAnimation());
    }

    // Handles wand animation timing for casting.
    private IEnumerator StopCastAnimation()
    {
        yield return new WaitForSeconds(0.75f);
        wandAnimator.SetBool("RaiseWand", false);
    }

    /// <summary>
    /// Clears all current charges and updates the UI.
    /// </summary>
    public void ClearCharges()
    {
        wandAnimator.SetBool("Charge", true);
        currentCharges.Clear();
        UIManager.Instance.ClearChargeSprites();
    }

    /// <summary>
    /// Gets the current charge tuple.
    /// </summary>
    /// <returns>The current charges as a ChargeTuple.</returns>
    public ChargeTuple GetCurrentCharges()
    {
        return currentCharges;
    }

    /// <summary>
    /// Gets the count of currently selected charges.
    /// </summary>
    /// <returns>The number of selected charges.</returns>
    public int GetCurrentChargesCount()
    {
        return currentCharges.Count();
    }

    /// <summary>
    /// Adds a spell to the charge-to-spell dictionary.
    /// </summary>
    /// <param name="newSpell">The spell GameObject to add.</param>
    /// <param name="chargeTuple">The charge tuple associated with the spell.</param>
    public void AddSpellToDict(GameObject newSpell, ChargeTuple chargeTuple)
    {
        chargeToSpell.Add(chargeTuple, newSpell.GetComponent<SpellEffect>());
    }
}
