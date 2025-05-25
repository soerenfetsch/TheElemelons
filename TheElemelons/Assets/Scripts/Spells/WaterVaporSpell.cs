using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVaporSpell : SpellEffect
{

    // Start is called before the first frame update
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.water, ChargeTypes.fire);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
