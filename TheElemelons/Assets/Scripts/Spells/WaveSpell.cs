using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
/// <summary>
/// Wave
/// deals some damage
/// </summary>
public class WaveSpell : SpellEffect
{
    float spellLifeTime = 3f;
    float moveSpeed = 1f;
    Vector2 movement;
    new Rigidbody2D rigidbody2D;
    List<Transform> rayCastPoints;
    RaycastHit2D hit;
    public GameObject splashEffect;
    Dictionary<Rigidbody2D, GameObject> victims;
    float damage = 1f;
    float thrust = 20f;
    float knockTime = 0.7f;
    // Start is called before the first frame update
    void Start()
    {
        spellType = new ChargeTuple(ChargeTypes.water, ChargeTypes.water);
        movement = transform.position - caster.transform.position;
        Transform castPoint = GameObject.Find("Player/Aim/Casting_Point").transform;
        transform.rotation = Quaternion.Euler(0, 0, 90) * castPoint.rotation;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rayCastPoints = new List<Transform>();
        for (int i = 0; i < 3; i++)
        {
            rayCastPoints.Add(transform.GetChild(1).GetChild(i).transform);
        }
        victims = new Dictionary<Rigidbody2D, GameObject>();
        StartCoroutine(Expire());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!hit)
        {
            DoRaycasts();
            rigidbody2D.MovePosition(rigidbody2D.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
    /// <summary>
    /// raycast to that wave doesnt move through walls
    /// </summary>
    void DoRaycasts()
    {

        float rayLength = 0.8f;
        LayerMask mask = LayerMask.GetMask("Interactable", "Walls");
        foreach (Transform point in rayCastPoints)
        {
            if (!hit)
            {
                hit = Physics2D.Raycast(point.position, movement, rayLength, mask);
            }
        }
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(spellLifeTime);
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Shrink");
        yield return new WaitForSeconds(1.1f);
        Destroy(transform.gameObject);
    }
    /// <summary>
    /// damage enemy, summon splash visual effect
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && other.isTrigger &&
        other.GetComponent<Enemy>().GetCurrentState() != EnemyStatus.Staggered)
        {
            Rigidbody2D otherRb = other.gameObject.GetComponent<Rigidbody2D>();
            if (!victims.ContainsKey(otherRb))
            {
                GameObject splash = Instantiate(splashEffect, other.transform.position + (Vector3)Vector3.down * 0.01f, Quaternion.Euler(0, 0, 0));
                splash.transform.localScale = other.transform.localScale;
                splash.transform.parent = other.transform;
                victims.Add(otherRb, splash);
            }
            CombatManager.ExecuteAttack(caster, other.GetComponent<Rigidbody2D>(), spellType, movement, damage, thrust, knockTime);
        }
        else if (other.gameObject.CompareTag("Player") && other.isTrigger)
        {
            Rigidbody2D otherRb = other.gameObject.GetComponent<Rigidbody2D>();
            if (!victims.ContainsKey(otherRb))
            {
                GameObject splash = Instantiate(splashEffect, other.transform.position + (Vector3)Vector3.down * 0.01f, Quaternion.Euler(0, 0, 0));
                splash.transform.localScale = other.transform.localScale;
                splash.transform.parent = other.transform;
                victims.Add(otherRb, splash);
            }
        }
    }
    /// <summary>
    /// remove splash effect
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
        if (other.gameObject.CompareTag("Enemy") && otherRb != null && victims.ContainsKey(otherRb))
        {
            RemoveSplash(otherRb);
        }
    }
    /// <summary>
    /// remove splash effect
    /// </summary>
    void OnDestroy()
    {
        foreach ((Rigidbody2D key, GameObject value) in victims)
        {
            RemoveSplash(key, editList: false);
        }
    }
    void RemoveSplash(Rigidbody2D key, bool editList = true)
    {
        Destroy(victims[key].gameObject);
        key.angularVelocity = 0;
        key.velocity = Vector2.zero;
        if (editList)
        {
            victims.Remove(key);
        }
    }
}
