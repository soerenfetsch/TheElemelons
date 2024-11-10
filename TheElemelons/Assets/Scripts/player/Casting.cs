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
    public float maxMana = 5f;
    public float mana;
    public float manaPerRegen = 0.25f;
    public float manaRegenSpeed = 0.2f;
    bool gameRunning = true;
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
    public float manaCost = 1f;
    public Dictionary<ChargeTuple,GameObject> chargeToSpell = new Dictionary<ChargeTuple,GameObject>();
    [Header("Internal Stuff")]
    public float current_regenspeed;
    public bool justCasted = false;
    void Start(){
        current_regenspeed = manaRegenSpeed;
        mana = maxMana;
        StartCoroutine(ManaRegen());
        currentCharges.Clear();
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.notype,ChargeTypes.notype),noChargeSpell);
        chargeToSpell.Add(new ChargeTuple(ChargeTypes.fire,ChargeTypes.fire),fireFireSpell);
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
    private IEnumerator ManaRegen(){
        while(gameRunning){
            if (mana<=0.25)
            {
                yield return new WaitForSeconds(current_regenspeed*2);
            }else if (justCasted)
            {
                justCasted = false;
                yield return new WaitForSeconds(current_regenspeed);
            }
            yield return new WaitForSeconds(current_regenspeed);
            mana = Mathf.Clamp(mana+manaPerRegen,0f,maxMana);
        }
    }
    void Update()
    {
        
        wandAnimator.SetBool("CanCast",canCast);
        if (Input.GetMouseButtonDown(0)){
            Cast();
        }

        //charges
        if (currentCharges.Count()<=maxCharges)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)){
                currentCharges.Add(ChargeTypes.fire);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(fireSprite);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)){
                currentCharges.Add(ChargeTypes.water);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(waterSprite);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)){
                currentCharges.Add(ChargeTypes.earth);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(earthSprite);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)){
                currentCharges.Add(ChargeTypes.wind);
                wandAnimator.SetBool("Charge",true);
                loadChargeSprite(windSprite);
            }
        }

        if (Input.GetMouseButtonDown(1)){
            currentCharges.Clear();
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

    void Cast(){
        //needs to be generalized to be applicable to all spells
        determineSpell();  
        if(canCast){
        mana = Mathf.Clamp(mana-manaCost,0f,maxMana);
        GetComponent<Mana_UI>().hurt();
        currentCharges.Clear();
        loadChargeSprite(earthSprite); //loads null as current charges lenth 0
        justCasted = true;
        Instantiate(currentSpell, castpoint.position,Quaternion.Euler(0,0,0));
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
        return charge1;
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
        byte[] bytes = MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(charge1.HumanName()+charge2.HumanName()));
        if(BitConverter.IsLittleEndian){
           Array.Reverse(bytes);
        }
        return BitConverter.ToInt32(bytes);
    }
}