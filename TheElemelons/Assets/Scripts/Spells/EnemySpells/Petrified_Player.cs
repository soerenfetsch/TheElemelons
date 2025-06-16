using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petrified_Player : MonoBehaviour
{
    Transform playerTransform;
    InputManager inputManager;
    Animator animator;
    float offset = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;

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
        transform.position = playerTransform.position - new Vector3(0, offset, 0);
        playerTransform.GetComponent<PlayerMovement>().SetMovement(Vector2.zero);
        transform.SetParent(playerTransform, worldPositionStays: true);
    }
    public IEnumerator Expire()
    {
        animator.SetTrigger("Decompose");
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
