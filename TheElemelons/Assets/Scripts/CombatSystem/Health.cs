using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public float maxhealth = 3;
    public float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxhealth;
    }

    public void SubtractHealth(float damage)
    {
        health -= damage;
    }

    public float GetHealth()
    {
        return health;
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
        health += plus;
    }
}
