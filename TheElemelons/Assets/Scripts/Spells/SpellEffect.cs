using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//abstract class for all spells
//all spells have a chargetuple as type and a cost for casting
public class SpellEffect : MonoBehaviour
{
    public float manaCost;
    public ChargeTuple spellType;
    public GameObject spellObject;
    public float GetManaCost()
    {
        return manaCost;
    }
}
