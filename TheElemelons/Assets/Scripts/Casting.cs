using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
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
    public GameObject castingObject;
    public Animator wandAnimator;
    public float spellLifeTime = 1f;
    bool canCast;
    public float maxMana = 5f;
    float mana;
    float manaCost = 1f;
    public float manaRegenSpeed = 0.25f;
    bool gameRunning = true;
    public int maxCharges = 2;
    List<ChargeTypes> currentCharges = new List<ChargeTypes>();
    public SpriteRenderer charge1UI;
    public SpriteRenderer charge2UI;
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite earthSprite;
    public Sprite windSprite;
    void Start(){
        
        mana = maxMana;
        StartCoroutine(ManaRegen());
    }
    private IEnumerator ManaRegen(){
        while(gameRunning){
            yield return new WaitForSeconds(1);
            mana = Mathf.Clamp(mana+manaRegenSpeed,0f,maxMana);
        }
    }
    void Update()
    {
        if (mana>=manaCost)
        {
            canCast = true;
            
        }else
        {
            canCast = false;
        }
        wandAnimator.SetBool("CanCast",canCast);
        if (Input.GetMouseButtonDown(0)){
            Cast();
        }

        //charges
        if (currentCharges.Count<=maxCharges)
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
        if(canCast){
        mana = Mathf.Clamp(mana-manaCost,0f,maxMana);
        
        currentCharges.Clear();
        loadChargeSprite(earthSprite); //loads null as current charges lenth 0
        
        GameObject simple_spell = Instantiate(castingObject, castpoint.position,Quaternion.Euler(0,0,0));

        }
    }

    void loadChargeSprite(Sprite chargeSprite){
        if (currentCharges.Count==0)
        {
            charge1UI.sprite = null;
            charge2UI.sprite = null;
        }else if(currentCharges.Count==1)
        {
            charge1UI.sprite = chargeSprite;
        }else if(currentCharges.Count==2)
        {
            charge2UI.sprite = chargeSprite;
        }

    }
    
}
