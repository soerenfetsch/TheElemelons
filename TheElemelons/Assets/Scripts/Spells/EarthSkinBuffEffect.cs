using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DefenseBuffSpell inherits SpellEffect
/// Gives Player a defense buff
/// </summary>
public class EarthSkinBuffEffect : MonoBehaviour
{
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
        transform.position = playerTransform.position;
        transform.parent = playerTransform;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();
        playerRigidBody = player.transform.GetComponent<Rigidbody2D>();
        inputManager = player.GetComponent<InputManager>();
        StartCoroutine(Appear());
    }
    /// <summary>
    /// First locks player inputs for animation where earthskin appears
    /// unlock inputs, wait till spelllifetime expires
    /// if no other defensspell active, restore original animator, trigger shatter animation
    /// destroy gameobject
    /// </summary>
    IEnumerator Appear()
    {
        // Lock movement for animation
        playerMovement.SetMovement(new Vector2(0, 0));
        playerAnimator.SetFloat("Vertical", -1);
        playerAnimator.SetFloat("Horizontal", 0);
        playerRigidBody.velocity = new Vector2(0, 0);
        inputManager.canInput = false;
        yield return new WaitForSeconds(1.5f);
        inputManager.canInput = true;
    }
    public IEnumerator Expire()
    {
        Debug.Log("earth expiring");
        playerAnimator.runtimeAnimatorController = playerRuntimeAnimator;
        gameObject.GetComponent<Animator>().SetTrigger("Shatter");
        yield return new WaitForSeconds(0.4f);
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
        playerAnimator.runtimeAnimatorController = earthPlayerAnimator;
    }
}
