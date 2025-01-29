using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuffable : MonoBehaviour, IBuffable
{
    public float speed = 1f;
    public float damage = 1f;
    public float defense = 1f;
    public float manaRegen = 1f;

    public void ModifySpeed(float factor)
    {
        speed *= factor;
    }

    public void ModifyDamage(float factor)
    {
        damage *= factor;
    }

    public void ModifyDefense(float factor)
    {
        defense *= factor;
    }

    public void ModifyManaRegen(float factor)
    {
        manaRegen *= factor;
    }
}
