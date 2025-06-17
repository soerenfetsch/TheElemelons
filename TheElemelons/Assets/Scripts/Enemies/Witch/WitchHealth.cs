using UnityEngine;

public class WitchHealth : Health
{
    Witch witchscript;
    new void Start()
    {
        witchscript = transform.GetComponent<Witch>();
        health = maxhealth;
    }
    /// <summary>
    /// amount of substracted health to witch increased if is bald
    /// </summary>
    /// <param name="damage"></param>
    public new void SubtractHealth(float damage)
    {
        if (damage < 0)
        {
            Debug.LogError("Damage must be greater than or equal to 0.");
            return;
        }
        if (witchscript.currentWitchState == WitchState.Bald)
        {
            damage *= 1.5f;
        }
        health = Mathf.Clamp(health - damage, 0f, maxhealth);
    }
}
