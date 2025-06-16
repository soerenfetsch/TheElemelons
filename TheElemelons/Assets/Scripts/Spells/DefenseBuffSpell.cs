using UnityEngine;

/// <summary>
/// DefenseBuffSpell inherits SpellEffect.
/// Gives Player a defense buff for a set duration and factor.
/// </summary>
public class DefenseBuffSpell : SpellEffect
{
    /// <summary>
    /// Duration of the defense buff in seconds.
    /// </summary>
    public float defenseTime = 15f;

    /// <summary>
    /// Multiplier applied to the player's defense while buff is active.
    /// </summary>
    public float defenseFactor = 2f;

    // Reference to the player GameObject.
    GameObject player;

    /// <summary>
    /// Unity Start method. Applies the defense buff to the player and destroys the spell object.
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        BuffDebuffManager.Instance.ApplyBuffDebuff(player, "EarthSkin", BuffDebuffManager.BuffDebuffType.DefenseBuff, defenseTime, defenseFactor, false);
        Destroy(gameObject);
    }
}
