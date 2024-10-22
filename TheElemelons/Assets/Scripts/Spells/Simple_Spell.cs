using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_Spell : MonoBehaviour
{
    public float spellLifeTime = 1f;
    public GameObject effect;
    // Start is called before the first frame update
    void Start (){
    }
    void Update()
    {
        Destroy(effect,spellLifeTime);
    }
}
