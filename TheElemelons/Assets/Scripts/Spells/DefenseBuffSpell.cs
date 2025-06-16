using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DefenseBuffSpell inherits SpellEffect
/// Gives Player a defense buff
/// </summary>
public class DefenseBuffSpell : SpellEffect
{
    public float defenseTime = 15f;
    public float defenseFactor = 2f;
    GameObject player;
    /// <summary>
    /// Initializes spelltype (Earth Notype), starts coroutine to expire, applies defense buff
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        BuffDebuffManager.Instance.ApplyBuffDebuff(player, "EarthSkin", BuffDebuffManager.BuffDebuffType.DefenseBuff, defenseTime, defenseFactor, false);
        Destroy(gameObject);
    }
}
