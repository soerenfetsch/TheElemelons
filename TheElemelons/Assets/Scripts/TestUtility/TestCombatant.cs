using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCombatant : MonoBehaviour, ICombatant
{
    public float attackFactor { get; set; }
    public float defenseFactor { get; set; }
    public ElementType elementType { get; set; }
    public bool isInvincible { get; set; }

    public void GetKnocked(float duration) { }
}
