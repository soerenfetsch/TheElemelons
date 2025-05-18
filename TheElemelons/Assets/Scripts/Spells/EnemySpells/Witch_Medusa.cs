using System.Collections;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Witch_Medusa : MonoBehaviour
{
    Witch witchScript;
    Transform playerTransform;
    InputManager inputManager;
    Animator animator;
    float offset = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        witchScript = GameObject.Find("Witch").GetComponent<Witch>();
        animator = GetComponent<Animator>();
        Animator playerAnimator = playerTransform.GetComponent<Animator>();
        AnimatorClipInfo[] playeranimatorinfo = playerAnimator.GetCurrentAnimatorClipInfo(0);
        string current_wand_animation = playeranimatorinfo[0].clip.name;
        if (current_wand_animation != "idle")
        {
            animator.SetBool("Wand", true);
        }
        Vector2 dir = new Vector2(playerAnimator.GetFloat("Horizontal"), playerAnimator.GetFloat("Vertical"));
        animator.SetFloat("Horizontal", dir.x);
        animator.SetFloat("Vertical", dir.y);
        transform.position = playerTransform.position - new Vector3(0, offset, 0); ;
        inputManager = playerTransform.GetComponent<InputManager>();
        inputManager.canInput = false;
        playerTransform.GetComponent<PlayerMovement>().SetMovement(Vector2.zero);
        StartCoroutine(Expire());
    }
    void FixedUpdate()
    {
        playerTransform.position = transform.position + new Vector3(0, offset, 0);
        transform.position = playerTransform.position + new Vector3(0, -offset, 0);
    }
    IEnumerator Expire()
    {
        yield return new WaitForSeconds(witchScript.medusaTime);
        animator.SetTrigger("Decompose");
        yield return new WaitForSeconds(0.6f);
        inputManager.canInput = true;
        Destroy(gameObject);
    }

}
