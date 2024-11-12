using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Blink_Animate : MonoBehaviour
{
    public void MakeBlink(){
        StartCoroutine(gameObject.transform.parent.GetComponent<Bomb_Spell>().BlinkTime());
    }
}
