using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

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
        List<BuffDebuffManager.BuffDebuff> buffs = BuffDebuffManager.Instance.GetActiveEffects(caster);
        foreach (BuffDebuffManager.BuffDebuff buff in buffs)
        {
            switch (buff.name)
            {
                case "EarthSkin":
                    Debug.Log("earthskin effect");
                    GameObject earthSpell = GameObject.Find("DefenseSpell(Clone)");
                    Debug.Log("spell object not null " + (earthSpell != null).ToString());
                    DefenseBuffSpell earthSpellScript = earthSpell.GetComponent<DefenseBuffSpell>();
                    earthSpellScript.InterruptSpell();
                    break;
            }
        }
        BuffDebuffManager.Instance.ClearEffects(caster);
    }
}
