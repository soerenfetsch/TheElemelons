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
    public Mana manaScript;

    [Header("Charges")]
    /// <summary>
    /// Maximum number of charges the player can hold.
    /// </summary>
    public int maxCharges = 2;
    public ChargeTuple currentCharges = new ChargeTuple();
    [Header("Spell Selection")]
    public GameObject noTypeSpell;
    public GameObject windSpell;
    public GameObject firefireSpell;
    public GameObject fireearthSpell;
    public GameObject earthwindSpell;
    public GameObject firewindSpell;
    public GameObject earthwaterSpell;
    public GameObject earthSpell;
    public GameObject earthearthSpell;
    public GameObject windwindSpell;
    public float manaCost = 1f;

    /// <summary>
    /// Maps charge combinations to corresponding spell effects.
    /// </summary>
    public Dictionary<ChargeTuple, SpellEffect> chargeToSpell = new Dictionary<ChargeTuple, SpellEffect>();

    [Header("Internal Stuff")]
    public PlayerMovement playerMovement;
    public UIManager uiManager;
    private Player player;

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
            uiManager.loadChargeSprite(selectedCharge);
        }
    }

    private IEnumerator RemoveWand()
    {
        // Times wand animation.
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
            //clear chargetuple and according ui
            ClearCharges();
            uiManager.ClearChargeSprites();
            manaScript.justCasted = true;
        }
        else
        {
            wandAnimator.SetBool("CanCast", false);
        }

        StartCoroutine(StopCastAnimation());
    }

    private IEnumerator StopCastAnimation()
    {
        // Times wand animation.
        yield return new WaitForSeconds(0.75f);
        wandAnimator.SetBool("RaiseWand", false);
    }

    public void ClearCharges()
    {
        wandAnimator.SetBool("Charge", true);
        currentCharges.Clear();
        uiManager.ClearChargeSprites();
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
