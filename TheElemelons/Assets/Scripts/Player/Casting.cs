using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public SpriteRenderer charge1UI;
    public SpriteRenderer charge2UI;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite earthSprite;
    public Sprite windSprite;
    [Header("Spell Selection")]
    GameObject currentSpell;
    public GameObject noTypeSpell;
    public float manaCost = 1f;
    public Dictionary<ChargeTuple, SpellEffect> chargeToSpell = new Dictionary<ChargeTuple, SpellEffect>();
    [Header("Internal Stuff")]
    PlayerMovement playerMovement;
    void Start()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        manaScript = gameObject.GetComponent<Mana>();
        maxMana = manaScript.maxMana;
        mana = manaScript.mana;
        ClearCharges();
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.notype, ChargeTypes.notype), noTypeSpell.GetComponent<SpellEffect>());
        currentSpell = noTypeSpell;
        determineSpell();
    }

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
        mana = manaScript.mana;


        if (wandAnimator.GetBool("Charge"))
        {
            StartCoroutine(RemoveWand());
        }
    }
    public void OnSelectCharge(ChargeTypes selectedCharge)
    {
        //charges
        if (currentCharges.Count() < maxCharges)
        {
            currentCharges.Add(selectedCharge);
            wandAnimator.SetBool("Charge", true);
            loadChargeSprite(selectedCharge);
        }
    }
    private IEnumerator RemoveWand()
    {
        yield return new WaitForSeconds(0.5f);
        wandAnimator.SetBool("Charge", false);
    }

    public void CastSpell()
    {
        GameObject newSpell = determineSpell();
        wandAnimator.SetBool("RaiseWand", true);
        if (CanCast() && newSpell != null)
        {
            wandAnimator.SetBool("CanCast", true);
            manaScript.SubtractMana(manaCost);
            Instantiate(newSpell, castpoint.position, Quaternion.Euler(0, 0, 0));
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
        yield return new WaitForSeconds(0.75f);
        wandAnimator.SetBool("RaiseWand", false);
    }

    //Refractor to UI manager
    void loadChargeSprite(ChargeTypes selectedCharge)
    {
        Sprite spriteToBeLoaded = null;
        if (selectedCharge == ChargeTypes.fire)
        {
            spriteToBeLoaded = fireSprite;
        }
        if (selectedCharge == ChargeTypes.water)
        {
            spriteToBeLoaded = waterSprite;
        }
        if (selectedCharge == ChargeTypes.earth)
        {
            spriteToBeLoaded = earthSprite;
        }
        if (selectedCharge == ChargeTypes.wind)
        {
            spriteToBeLoaded = windSprite;
        }

        if (currentCharges.Count() == 0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }
        else if (currentCharges.Count() == 1)
        {
            charge1UI.sprite = spriteToBeLoaded;
        }
        else if (currentCharges.Count() == 2)
        {
            charge2UI.sprite = spriteToBeLoaded;
        }

    }
    //Refractor to UI manager
    void ClearChargeSprites()
    {
        if (currentCharges.Count() == 0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }
    }

    public void ClearCharges()
    {
        wandAnimator.SetBool("Charge", true);
        currentCharges.Clear();
        ClearChargeSprites();
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











