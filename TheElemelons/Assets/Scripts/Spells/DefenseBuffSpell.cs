using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBuffSpell : SpellEffect
{
    public float defenseTime = 15f;
    public float defenseFactor = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
        GameObject player = GameObject.Find("Player");
        BuffDebuffManager.Instance.ApplyBuffDebuff(player, BuffDebuffManager.BuffDebuffType.DefenseBuff, defenseTime, defenseFactor);
        Destroy(gameObject);    
    }
}
