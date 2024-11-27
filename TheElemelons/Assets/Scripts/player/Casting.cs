using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.Xml;
public enum ChargeTypes{
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
    Mana_UI manaScript;
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
    public GameObject currentSpell;
    public GameObject noChargeSpell;
    public GameObject fireFireSpell;
    public GameObject windSpell;
    public GameObject fireEarthSpell;
    public GameObject fireWindSpell;
    public GameObject earthWindSpell;
    public float manaCost = 1f;
    public Dictionary<ChargeTuple,GameObject> chargeToSpell = new Dictionary<ChargeTuple,GameObject>();
    [Header("Internal Stuff")]
    player_movement player_Movement;
    void Start(){
        player_Movement = gameObject.GetComponent<player_movement>();
        manaScript = gameObject.GetComponent<Mana_UI>();
        maxMana = manaScript.maxMana;
        mana = manaScript.mana;
        ClearCharges();
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.notype,ChargeTypes.notype),noChargeSpell);
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.fire,ChargeTypes.fire),fireFireSpell);
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.wind,ChargeTypes.notype),windSpell);
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.fire,ChargeTypes.earth),fireEarthSpell);
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.fire,ChargeTypes.wind),fireWindSpell);
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.earth,ChargeTypes.wind),earthWindSpell);
        determineSpell();        
    }
    
    void determineSpell(){
        if(chargeToSpell.ContainsKey(currentCharges)){
            currentSpell = chargeToSpell[currentCharges];
            manaCost = currentSpell.GetComponent<SpellEffect>().manaCost;
            if (mana>=manaCost)
            {
                canCast = true;
                
            }else
            {
                canCast = false;
            }
        }else
        {
            canCast = false;
        }
    }
    void Update()
    {
        mana = manaScript.mana;
        //sparkles of wand
        wandAnimator.SetBool("CanCast",canCast);
        //cast
        if (Input.GetMouseButtonDown(0)){
            CastSpell();
        }
        //Raise Wand 
        if (Input.GetMouseButtonDown(0) && !wandAnimator.GetBool("RaiseWand") && player_Movement.currentState != PlayerState.stagger)
        {
            wandAnimator.SetBool("RaiseWand",true);
        } 
        if (Input.GetMouseButtonUp(0) && wandAnimator.GetBool("RaiseWand"))
        {
            wandAnimator.SetBool("RaiseWand",false);
        }
        //select charhe
        OnSelectCharge();
    }
    public void OnSelectCharge()
    {
        //charges
        if (currentCharges.Count()<=maxCharges)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1) && currentCharges.Count()<2){
                currentCharges.Add(ChargeTypes.fire);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(fireSprite);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2) && currentCharges.Count()<2){
                currentCharges.Add(ChargeTypes.water);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(waterSprite);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3) && currentCharges.Count()<2){
                currentCharges.Add(ChargeTypes.earth);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(earthSprite);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4) && currentCharges.Count()<2){
                currentCharges.Add(ChargeTypes.wind);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(windSprite);
            }
        }
        if (Input.GetMouseButtonDown(1)){
            ClearCharges();
            loadChargeSprite(earthSprite); //loads null as current charges lenth 0
        }
        if (wandAnimator.GetBool("Charge") && !Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKeyDown(KeyCode.Alpha3) && !Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(RemoveWand());
        }
    }
    private IEnumerator RemoveWand(){
        yield return new WaitForSeconds(0.75f);
        wandAnimator.SetBool("Charge",false);
    }

    void CastSpell(){
        //needs to be generalized to be applicable to all spells
        determineSpell();  
        if(canCast){
        manaScript.SubtractMana(manaCost);
        Instantiate(currentSpell, castpoint.position,Quaternion.Euler(0,0,0));
        ClearCharges();
        loadChargeSprite(earthSprite); //loads null as current charges lenth 0
        manaScript.justCasted = true;
        }
    }

    void loadChargeSprite(Sprite chargeSprite){
        if (currentCharges.Count()==0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }else if(currentCharges.Count()==1)
        {
            charge1UI.sprite = chargeSprite;
        }else if(currentCharges.Count()==2)
        {
            charge2UI.sprite = chargeSprite;
        }

    }

    public void ClearCharges()
    {
        currentCharges.Clear();
    }
    
}








public class ChargeTuple{
    private ChargeTypes charge1;
    private ChargeTypes charge2;
    public ChargeTuple(){
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }
    public ChargeTuple(ChargeTypes chargeOne,ChargeTypes chargeTwo){
        charge1 = chargeOne;
        charge2 = chargeTwo;
    }
    public void Add(ChargeTypes charge){
        if (charge1==ChargeTypes.notype)
        {
            charge1=charge;
        }else if(charge2==ChargeTypes.notype){
            charge2=charge;
        }
    }
    public void Clear(){
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }
    public List<ChargeTypes> GetCharges(){
        return new List<ChargeTypes> {charge1,charge2};
    }
    public int Count(){
        int count = 0;
        if (charge1!=ChargeTypes.notype)
        {
            count++;
        }
        if (charge2!=ChargeTypes.notype)
        {
            count++;
        }
        return count;
    }
    
    public ChargeTypes first(){
        return charge1;
    }
    public ChargeTypes second(){
        return charge2;
    }
    public float HasHowMany(ChargeTypes other){
        float count = 0;
        if (charge1.HumanName() == other.HumanName())
        {   
            count++;
        }
        if (charge2.HumanName() == other.HumanName())
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
        ChargeTuple chargeObj= (ChargeTuple)obj;
        // TODO: write your implementation of Equals() here
        if ((charge1==chargeObj.first() && charge2==chargeObj.second()) 
        ||(charge2==chargeObj.first() && charge1==chargeObj.second()))
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
        byte[] bytes = MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(((int)charge1+(int)charge2).ToString()));
        if(BitConverter.IsLittleEndian){
           Array.Reverse(bytes);
        }
        return BitConverter.ToInt32(bytes);
    }
}