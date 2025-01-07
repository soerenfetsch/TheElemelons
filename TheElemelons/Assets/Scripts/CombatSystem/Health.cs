using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//manage health value of entity 
public class Health : MonoBehaviour
{

    public float maxhealth = 3;
    public float health;

    void Start()
    {
        health = maxhealth;
    }

    public void SubtractHealth(float damage)
    {
        //reduce health, cant go below 0
        health = Mathf.Clamp(health - damage, 0f, maxhealth);
    }

    public bool IsZero()
    {
        if (health <= 0)
        {
            return true;
        }
        return false;
    }
    public void AddHealth(float plus)
    {
        //increase health, cant exceed max health
        health = Mathf.Clamp(health + plus, 0f, maxhealth);
    }

    public float GetMaxHealth()
    {
        return maxhealth;
    }
    public float GetHealth()
    {
        return health;
    }
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }
    public void SetMaxHealth(float newMaxHealth)
    {
        maxhealth = newMaxHealth;
    }
}
