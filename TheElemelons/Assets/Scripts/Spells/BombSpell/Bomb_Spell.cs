using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Spell : SpellEffect
{
    public float blinkTime = 0.1f;
    GameObject blink;
    int blinked = 0;
    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        blink = gameObject.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (blinked == 3)
        {
            Instantiate(explosion, transform.position,UnityEngine.Quaternion.Euler(0,0,0));
            Destroy(gameObject);
        }

    }
    public IEnumerator BlinkTime(){
        if (blinked<=3)
        {
            blink.SetActive(true);
            yield return new WaitForSeconds(blinkTime);
            blink.SetActive(false);
            blinked += 1;
        }
        
    }
}
