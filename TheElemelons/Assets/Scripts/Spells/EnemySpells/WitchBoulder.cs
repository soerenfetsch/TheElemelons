using System.Collections;
using UnityEngine;

/// <summary>
/// Boulder that free falls ~20 units and then shatters (on the ground), dealing damage (~1 heart) to the 
/// player on collision when it is ~2 units or closer to its shattering point.
/// Drop shadow indicates where it will hit the ground. GameObject is destroyed after shatter animation.
/// </summary>
public class WitchBoulder : SpellEffect
{
    // Transform of the falling boulder.
    Transform boulderTransform;

    /// <summary>
    /// Height above the ground from which the boulder is dropped.
    /// </summary>
    public float dropHeight = 20;

    // Animator for the boulder shatter animation.
    Animator boulderAnimator;
    // Collider for detecting collisions when the boulder lands.
    CircleCollider2D circleCollider2D;
    // Rigidbody2D for boulder physics.
    Rigidbody2D boulderRb;

    /// <summary>
    /// Damage dealt to the player on impact.
    /// </summary>
    public float baseDamage = 0f;

    /// <summary>
    /// Knockback force applied to the player on impact.
    /// </summary>
    public float thrust = 0f;

    /// <summary>
    /// Duration of knockback applied to the player.
    /// </summary>
    public float knockTime = 0f;

    // Tracks whether the boulder has smashed.
    bool smash = false;

    /// <summary>
    /// Initializes the boulder's position, animator, collider, and physics.
    /// Sets damage and knockback values from the Witch caster if available.
    /// </summary>
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

    /// <summary>
    /// Handles the boulder's falling logic and triggers the smash sequence when it lands.
    /// </summary>
    void FixedUpdate()
    {
        float distance = (boulderTransform.position - transform.position).magnitude;
        if (distance < 0.2f && !smash)
        {
            smash = true;
            boulderTransform.position = transform.position;
            boulderRb.gravityScale = 0;
            boulderRb.angularVelocity = 0;
            boulderRb.velocity = Vector3.zero;
            boulderRb.isKinematic = true;
            circleCollider2D.enabled = true;
            StartCoroutine(Expire());
        }
    }

    /// <summary>
    /// Plays the smash animation and destroys the boulder after the animation completes.
    /// </summary>
    IEnumerator Expire()
    {
        boulderAnimator.SetTrigger("Smash");
        yield return new WaitForSeconds(0.9f);
        Destroy(gameObject);
    }
}
