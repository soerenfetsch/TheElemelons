using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICombatant
{
    float attackFactor { get; set; }
    float defenseFactor { get; set; }
    bool isInvincible { get; set; }
    ElementType elementType { get; set; }
    void GetKnocked(float knockTime);
}
