using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.Xml;
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

    GameObject determineSpell()
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
        if (!chargeToSpell.ContainsKey(currentCharges) || playerMovement.GetCurrentState()==PlayerState.stagger)
        {
            return false;
        }
        manaCost = chargeToSpell[currentCharges].manaCost;
        if(mana >= manaCost)
        {
            return true;
        }
        return false;
    }
    void Update()
    {
        mana = manaScript.mana;


        if (wandAnimator.GetBool("Charge")){
            StartCoroutine(RemoveWand());
        }
    }
    public void OnSelectCharge(ChargeTypes selectedCharge)
    {
        //charges
        if (currentCharges.Count() <= maxCharges)
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
        if (CanCast() && newSpell!=null)
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

}











public class ChargeTuple
{
    private ChargeTypes charge1;
    private ChargeTypes charge2;
    public ChargeTuple()
    {
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }
    public ChargeTuple(ChargeTypes chargeOne, ChargeTypes chargeTwo)
    {
        charge1 = chargeOne;
        charge2 = chargeTwo;
    }
    public void Add(ChargeTypes charge)
    {
        if (charge1 == ChargeTypes.notype)
        {
            charge1 = charge;
        }
        else if (charge2 == ChargeTypes.notype)
        {
            charge2 = charge;
        }
    }
    public void Clear()
    {
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }
    public List<ChargeTypes> GetCharges()
    {
        return new List<ChargeTypes> { charge1, charge2 };
    }
    public int Count()
    {
        int count = 0;
        if (charge1 != ChargeTypes.notype)
        {
            count++;
        }
        if (charge2 != ChargeTypes.notype)
        {
            count++;
        }
        return count;
    }

    public ChargeTypes first()
    {
        return charge1;
    }
    public ChargeTypes second()
    {
        return charge2;
    }
    public float HasHowMany(ChargeTypes other)
    {
        float count = 0;
        if (charge1 == other)
        {
            count++;
        }
        if (charge2 == other)
        {
            count++;
        }
        return count;
    }
    // override object.Equals
    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        ChargeTuple chargeObj = (ChargeTuple)obj;
        // TODO: write your implementation of Equals() here
        if ((charge1 == chargeObj.first() && charge2 == chargeObj.second())
        || (charge2 == chargeObj.first() && charge1 == chargeObj.second()))
        {
            return true;
        }
        return false;
    }

    //override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        //throw new System.NotImplementedException();
        byte[] bytes = MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(((int)charge1 + (int)charge2).ToString()));
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return BitConverter.ToInt32(bytes);
    }
}