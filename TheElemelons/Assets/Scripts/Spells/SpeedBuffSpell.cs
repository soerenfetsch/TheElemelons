using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SpeedBuffSpell : SpellEffect
{
    public float spellLifeTime = 30f;
    public float moveSpeedFactor = 1.5f;

    /// <summary>
    /// Called when the SpeedBuffSpell is created
    /// Uses the BuffDebuffManager to apply the speed buff to the player
    /// Duration and move speed factor are set in the inspector of the spell 
    /// </summary>
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.wind, ChargeTypes.wind);
        GameObject player = GameObject.Find("Player");
        BuffDebuffManager.Instance.ApplyBuffDebuff(player, "SpeedBuffSpell", BuffDebuffManager.BuffDebuffType.SpeedBuff, spellLifeTime, moveSpeedFactor,false);
        // Destroy the object directly
        Destroy(gameObject);
    }
}
