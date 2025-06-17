using UnityEngine;

public class TikiHealth : Health
{
    Tiki tikiscript;
    new void Start()
    {
        tikiscript = transform.parent.GetComponent<Tiki>();
        health = maxhealth;
    }
    /// <summary>
    /// amount of substracted health to tiki increased if head detached
    /// </summary>
    /// <param name="damage"></param>
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
