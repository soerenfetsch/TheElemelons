using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_Spell : SpellEffect
{
    public float blinkTime = 0.1f;
    Animator blinkingAnimator;
    int blinked = 0;
    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        blinkingAnimator = gameObject.GetComponent<Animator>();
        spellType = new ChargeTuple(ChargeTypes.fire,ChargeTypes.earth);
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
            blinkingAnimator.SetTrigger("blink");
            yield return new WaitForSeconds(blinkTime);
            blinkingAnimator.SetTrigger("blink");
            blinked += 1;
        }
        
    }
}
