using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DefenseBuffSpell inherits SpellEffect
/// Gives Player a defense buff
/// </summary>
public class DefenseBuffSpell : SpellEffect
{
    public float defenseTime = 15f;
    public float defenseFactor = 2f;
    public RuntimeAnimatorController earthPlayerAnimator;
    public RuntimeAnimatorController playerRuntimeAnimator;
    GameObject player;
    Transform playerTransform;
    InputManager inputManager;
    PlayerMovement playerMovement;
    Animator playerAnimator;
    Rigidbody2D playerRigidBody;
    /// <summary>
    /// Initializes spelltype (Earth Notype), starts coroutine to expire, applies defense buff
    /// </summary>
    void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
        playerRigidBody = player.transform.GetComponent<Rigidbody2D>();
        inputManager = player.GetComponent<InputManager>();
        BuffDebuffManager.Instance.ApplyBuffDebuff(player, "EarthSkin", BuffDebuffManager.BuffDebuffType.DefenseBuff, defenseTime, defenseFactor, false);
        StartCoroutine(Expire());
    }
    /// <summary>
    /// synchs position with player to that animations appear at right place
    /// </summary>
    void FixedUpdate()
    {
        gameObject.transform.position = playerTransform.position;
    }
    /// <summary>
    /// First locks player inputs for animation where earthskin appears
    /// unlock inputs, wait till spelllifetime expires
    /// if no other defensspell active, restore original animator, trigger shatter animation
    /// destroy gameobject
    /// </summary>
    IEnumerator Expire()
    {
        // Lock movement for animation
        playerMovement.SetMovement(new Vector2(0, 0));
        playerAnimator.SetFloat("Vertical", -1);
        playerAnimator.SetFloat("Horizontal", 0);
        playerRigidBody.velocity = new Vector2(0, 0);
        inputManager.canInput = false;
        yield return new WaitForSeconds(1.5f);
        inputManager.canInput = true;

        yield return new WaitForSeconds(defenseTime - 1.5f);

        bool otherSpellActive = false;
        List<BuffDebuffManager.BuffDebuff> buffList = BuffDebuffManager.Instance.GetActiveEffects(player);
        foreach (BuffDebuffManager.BuffDebuff buff in buffList)
        {
            if (buff != null && buff.name == "EarthSkin")
            {
                otherSpellActive = true;
            }
        }
        if (!otherSpellActive)
        {
            playerAnimator.runtimeAnimatorController = playerRuntimeAnimator;
            gameObject.GetComponent<Animator>().SetTrigger("Shatter");
            yield return new WaitForSeconds(0.4f);
        }

        Destroy(gameObject);
    }
    public void InterruptSpell()
    {
        playerAnimator.runtimeAnimatorController = playerRuntimeAnimator;
        Destroy(gameObject);
    }
    /// <summary>
    /// Switch player animator to earthy, triggered by animation event
    /// </summary>
    public void Switch_to_Earthy_Animator()
    {
        RuntimeAnimatorController animatorController = playerAnimator.GetComponent<RuntimeAnimatorController>();
        playerAnimator.runtimeAnimatorController = earthPlayerAnimator;
        earthPlayerAnimator = animatorController;
    }
}
