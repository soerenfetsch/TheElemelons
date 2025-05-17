using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Assertions;

public class WitchBoulder : SpellEffect
{
    Transform boulderTransform;
    public float dropHeight = 20;
    Animator boulderAnimator;
    CircleCollider2D circleCollider2D;
    Rigidbody2D boulderRb;
    public float baseDamage = 0f;
    public float thrust = 0f;
    public float knockTime = 0f;
    bool smash = false;
    // Start is called before the first frame update
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.earth, ChargeTypes.notype);
        boulderTransform = transform.GetChild(0);
        boulderTransform.position = transform.position + new Vector3(0, dropHeight, 0);
        boulderAnimator = boulderTransform.GetComponent<Animator>();
        circleCollider2D = boulderTransform.GetComponent<CircleCollider2D>();
        boulderRb = boulderTransform.GetComponent<Rigidbody2D>();
        Witch witchscript = caster.GetComponent<Witch>();
        if (witchscript)
        {
            baseDamage = witchscript.boulderDamage;
            thrust = witchscript.boulderThrust;
            knockTime = witchscript.boulderKnockTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = (boulderTransform.position - transform.position).magnitude;
        if (distance < 0.2f && !smash)
        {
            smash = true;
            Debug.Log("Close");
            boulderTransform.position = transform.position;
            boulderRb.gravityScale = 0;
            boulderRb.angularVelocity = 0;
            boulderRb.velocity = Vector3.zero;
            boulderRb.isKinematic = true;
            boulderAnimator.SetTrigger("Smash");
            StartCoroutine(Expire());
        }

        if (distance < 2f)
        {
            circleCollider2D.enabled = true;
        }
        if (circleCollider2D.enabled)
        {
            Debug.Log("circle enabled");
        }
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(0.9f);
        Destroy(gameObject);
    }

}
