using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Handle Spell Casting
public enum ChargeTypes
{
    fire,
    water,
    earth,
    wind,
    notype
}
public class Casting : MonoBehaviour
{
    public Transform castpoint;
    public Animator wandAnimator;
    [Header("Mana Management")]
    bool canCast;
    float mana;
    float maxMana;
    Mana manaScript;
    [Header("Charges")]
    public int maxCharges = 2;
    ChargeTuple currentCharges = new ChargeTuple();
    [Header("Spell Selection")]
    GameObject currentSpell;
    public GameObject noTypeSpell;
    public float manaCost = 1f;
    public Dictionary<ChargeTuple, SpellEffect> chargeToSpell = new Dictionary<ChargeTuple, SpellEffect>();
    [Header("Internal Stuff")]
    PlayerMovement playerMovement;
    public UIManager uIManager;
    void Start()
    {
        //Variable assignment
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        manaScript = gameObject.GetComponent<Mana>();
        maxMana = manaScript.maxMana;
        mana = manaScript.mana;
        ClearCharges();
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype), noTypeSpell.GetComponent<SpellEffect>());
        currentSpell = noTypeSpell;
        determineSpell();
    }
    /// <summary>
    /// Determines which spell would be casted according to currently selected charges
    /// </summary>
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
    /// checks if player can cast selected spell, depending on whether spell for charge combo exists
    /// and on whether player has enough mana
    /// </summary>
    public bool CanCast()
    {
        if (!chargeToSpell.ContainsKey(currentCharges) || playerMovement.GetCurrentState() == PlayerState.stagger)
        {
            return false;
        }
        manaCost = chargeToSpell[currentCharges].manaCost;
        if (mana >= manaCost)
        {
            return true;
        }
        return false;
    }
    void Update()
    {
        //get current mana from script
        mana = manaScript.mana;
        //animation handling
        if (wandAnimator.GetBool("Charge"))
        {
            StartCoroutine(RemoveWand());
        }
    }
    /// <summary>
    /// select charge for current charge tuple
    /// cant add charge if chargetuple already full
    /// </summary>
    /// <param name="selectedCharge"></param>
    public void OnSelectCharge(ChargeTypes selectedCharge)
    {
        if (currentCharges.Count() < maxCharges)
        {
            currentCharges.Add(selectedCharge);
            wandAnimator.SetBool("Charge", true);
            uIManager.loadChargeSprite(selectedCharge);
        }
    }
    private IEnumerator RemoveWand()
    {
        //time wand animation
        yield return new WaitForSeconds(0.5f);
        wandAnimator.SetBool("Charge", false);
    }
    /// <summary>
    /// Spawn Spell gameobject correspoding to selected charges
    /// then clear selected charges and substract manacost of spell
    /// </summary>
    public void CastSpell()
    {
        //cast spell on input
        //determine spell to be casted
        GameObject newSpell = determineSpell();
        wandAnimator.SetBool("RaiseWand", true);
        //check if can cast
        if (CanCast() && newSpell != null)
        {
            //handle wand animator, substract manacost and instanstiate spell gameobject
            wandAnimator.SetBool("CanCast", true);
            manaScript.SubtractMana(manaCost);
            Instantiate(newSpell, castpoint.position, Quaternion.Euler(0, 0, 0));
            //clear chargetuple and according ui
            ClearCharges();
            uIManager.ClearChargeSprites();
            manaScript.justCasted = true;
        }
        else
        {
            //trigger smoking wand animation because casting failed
            wandAnimator.SetBool("CanCast", false);
        }
        StartCoroutine(StopCastAnimation());
    }
    private IEnumerator StopCastAnimation()
    {
        //time wand animation
        yield return new WaitForSeconds(0.75f);
        wandAnimator.SetBool("RaiseWand", false);
    }

    public void ClearCharges()
    {
        wandAnimator.SetBool("Charge", true);
        currentCharges.Clear();
        uIManager.ClearChargeSprites();
    }


    public ChargeTuple GetCurrentCharges()
    {
        return currentCharges;
    }
    public int GetCurrentChargesCount()
    {
        return currentCharges.Count();
    }
}











