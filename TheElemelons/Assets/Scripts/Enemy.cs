using System.Collections;
using System.Collections.Generic;
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
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;
    public Vector2 homePosition;

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
        transform.position = homePosition;
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

    public void Knock(Rigidbody2D myRigidbody, float knockTime, float damage, ChargeTypes attackType)
    {
        StartCoroutine(KnockCo(myRigidbody, knockTime));
        TakeDamage(damage*typeWeaknessResistanceCalc(attackType));
    }

    private IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            myRigidbody.GetComponent<Animator>().SetBool("Stagger",true);
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = EnemyState.idle;
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.GetComponent<Animator>().SetBool("Stagger",false);
        }
    }

//computes modifier depending on own type and suffered attack type
// water beats fire beats earth beats wind
    float typeWeaknessResistanceCalc(ChargeTypes attackType){
        if (type == ChargeTypes.water){
            if(attackType == ChargeTypes.wind){
                //is weak, thus more damage
                return 1+weaknessResistanceFactor;
            }else if(attackType == ChargeTypes.fire)
            {
                //is resistance, thus less damage
                return 1-weaknessResistanceFactor;
            }
        }else if (type == ChargeTypes.fire){
            if(attackType == ChargeTypes.water){
                //is weak, thus more damage
                return 1+weaknessResistanceFactor;
            }else if(attackType == ChargeTypes.earth)
            {
                //is resistance, thus less damage
                return 1-weaknessResistanceFactor;
            }
        }else if (type == ChargeTypes.earth){
            if(attackType == ChargeTypes.fire){
                //is weak, thus more damage
                return 1+weaknessResistanceFactor;
            }else if(attackType == ChargeTypes.wind)
            {
                //is resistance, thus less damage
                return 1-weaknessResistanceFactor;
            }
        }else if (type == ChargeTypes.wind){
            if(attackType == ChargeTypes.earth){
                //is weak, thus more damage
                return 1+weaknessResistanceFactor;
            }else if(attackType == ChargeTypes.water)
            {
                //is resistance, thus less damage
                return 1-weaknessResistanceFactor;
            }
        }

        return 1;
    }
}