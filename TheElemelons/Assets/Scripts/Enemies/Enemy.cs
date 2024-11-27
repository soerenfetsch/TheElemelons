using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public enum EnemyState{
    idle,
    walk,
    attack,
    stagger
}

public class Enemy : MonoBehaviour {

    [Header("State Machine")]
    public EnemyState currentState;

    [Header("Enemy Stats")]
    public float maxHealth;
    float health;
    public string enemyName;
    public float baseAttack;
    public float moveSpeed;
    public Vector2 homePosition;
    public float staggerTime = 0.5f;

    [Header("Death Effects")]
    public GameObject Death_Effect;
    private float deathEffectDelay = 1f;
    public ChargeTypes type;

    public float weaknessResistanceFactor = 0.2f;

   
    private void Awake()
    {
        health = maxHealth;
    }

    private void OnEnable()
    {
        //transform.position = homePosition;
        health = maxHealth;
        currentState = EnemyState.idle;
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            DeathEffect();

            this.gameObject.SetActive(false);
        }
    }

    

    private void DeathEffect()
    {
        if(Death_Effect != null)
        {
            GameObject effect = Instantiate(Death_Effect, transform.position, Quaternion.identity);
            Destroy(effect, deathEffectDelay);
        }
    }

    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage, ChargeTuple attackType)
    {
        currentState = EnemyState.stagger;
        myRigidbody.GetComponent<Animator>().SetBool("Stagger",true);
        myRigidbody.GetComponent<Animator>().SetTrigger("Staggertrig");
        StartCoroutine(KnockCo(myRigidbody, knockTime));
        TakeDamage(damage*typeWeaknessResistanceCalc(attackType));
    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = EnemyState.idle;
            myRigidbody.velocity = Vector2.zero;
            yield return new WaitForSeconds(staggerTime);
            myRigidbody.GetComponent<Animator>().SetBool("Stagger",false);
        }
    }

//computes modifier depending on own type and suffered attack type
// water beats fire beats earth beats wind
    float typeWeaknessResistanceCalc(ChargeTuple attackType){
        float modifier = 1;
        if (type == ChargeTypes.water){
            modifier += weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.wind);
            modifier -= weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.fire);
        }else if (type == ChargeTypes.fire){
            modifier += weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.water);
            modifier -= weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.earth);
        }else if (type == ChargeTypes.earth){
            modifier += weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.fire);
            modifier -= weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.wind);
        }else if (type == ChargeTypes.wind){
            modifier += weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.earth);
            modifier -= weaknessResistanceFactor*attackType.HasHowMany(ChargeTypes.water);
        }
        return modifier;
    }
}