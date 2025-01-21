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
    private bool canCast;
    private float mana;
    private float maxMana;

    /// <summary>
    /// Reference to the Mana script for managing player mana.
    /// </summary>
    private Mana manaScript;

    [Header("Charges")]
    /// <summary>
    /// Maximum number of charges the player can hold.
    /// </summary>
    public int maxCharges = 2;

    /// <summary>
    /// The player's currently selected charges.
    /// </summary>
    private ChargeTuple currentCharges = new ChargeTuple();

    /// <summary>
    /// UI elements representing the first and second charges.
    /// </summary>
    public SpriteRenderer charge1UI;
    public SpriteRenderer charge2UI;

    /// <summary>
    /// Sprites representing charge types.
    /// </summary>
    public Sprite fireSprite, waterSprite, earthSprite, windSprite;

    [Header("Spell Selection")]
    private GameObject currentSpell;
    public GameObject noTypeSpell;
    public float manaCost = 1f;

    /// <summary>
    /// Maps charge combinations to corresponding spell effects.
    /// </summary>
    public Dictionary<ChargeTuple, SpellEffect> chargeToSpell = new Dictionary<ChargeTuple, SpellEffect>();

    [Header("Internal Stuff")]
    private Player player;

    private void Start()
    {
        // Initializes variables and setups.
        player = gameObject.GetComponent<Player>();
        manaScript = gameObject.GetComponent<Mana>();
        maxMana = manaScript.maxMana;
        mana = manaScript.mana;
        ClearCharges();
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype), noTypeSpell.GetComponent<SpellEffect>());
        currentSpell = noTypeSpell;
        determineSpell();
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
        return mana >= manaCost;
    }

    private void Update()
    {
        // Gets current mana from the Mana script.
        mana = manaScript.mana;

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
            loadChargeSprite(selectedCharge);
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
            Instantiate(newSpell, castpoint.position, Quaternion.identity);
            ClearCharges();
            ClearChargeSprites();
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

    /// <summary>
    /// Updates the UI to display the selected charge.
    /// </summary>
    /// <param name="selectedCharge">The charge type to display.</param>
    private void loadChargeSprite(ChargeTypes selectedCharge)
    {
        Sprite spriteToBeLoaded = null;
        switch (selectedCharge)
        {
            case ChargeTypes.fire:
                spriteToBeLoaded = fireSprite;
                break;
            case ChargeTypes.water:
                spriteToBeLoaded = waterSprite;
                break;
            case ChargeTypes.earth:
                spriteToBeLoaded = earthSprite;
                break;
            case ChargeTypes.wind:
                spriteToBeLoaded = windSprite;
                break;
        }

        if (currentCharges.Count() == 1)
        {
            charge1UI.sprite = spriteToBeLoaded;
        }
        else if (currentCharges.Count() == 2)
        {
            charge2UI.sprite = spriteToBeLoaded;
        }
    }

    /// <summary>
    /// Clears all charge sprites from the UI.
    /// </summary>
    private void ClearChargeSprites()
    {
        charge1UI.sprite = null;
        charge2UI.sprite = null;
    }

    /// <summary>
    /// Resets the current charges and updates UI.
    /// </summary>
    public void ClearCharges()
    {
        wandAnimator.SetBool("Charge", true);
        currentCharges.Clear();
        ClearChargeSprites();
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
}
