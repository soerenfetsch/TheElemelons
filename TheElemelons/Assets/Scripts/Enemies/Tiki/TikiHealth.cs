using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikiHealth : Health
{
    Tiki tikiscript;
    new void Start()
    {
        tikiscript = transform.parent.GetComponent<Tiki>();
        health = maxhealth;
    }
    public new void SubtractHealth(float damage)
    {
        if (damage < 0)
        {
            Debug.LogError("Damage must be greater than or equal to 0.");
            return;
        }
        if (!tikiscript.IsHeadAttached())
        {
            damage *= tikiscript.noHeadVulnerabilityFactor;
        }
        health = Mathf.Clamp(health - damage, 0f, maxhealth);
    }
}
