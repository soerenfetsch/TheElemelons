using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
/// <summary>
/// spell to cleanse player of all buffs and debuffs
/// </summary>
public class CleanseSpell : SpellEffect
{
    Transform casterTransform;
    float animationtime = 2.3f;
    InputManager playerInput;
    Animator animator;
    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        casterTransform = caster.transform;
        animator = GetComponent<Animator>();
        try
        {
            playerInput = caster.GetComponent<InputManager>();
            playerMovement = caster.GetComponent<PlayerMovement>();
        }
        catch (System.Exception)
        {
            throw;
        }
        RemoveBuffs();
        StartCoroutine(Expire());
    }
    void FixedUpdate()
    {
        transform.position = casterTransform.position + (Vector3)Vector2.down * 0.001f;
        Vector2 movement = playerMovement.GetMovement();
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }
    IEnumerator Expire()
    {
        if (playerInput != null)
        {
            playerInput.canInput = false;
        }
        yield return new WaitForSeconds(animationtime);
        if (playerInput != null)
        {
            playerInput.canInput = true;
        }
        Destroy(gameObject);
    }
    void RemoveBuffs()
    {
        BuffDebuffManager.Instance.ClearEffects(caster);
    }
}
