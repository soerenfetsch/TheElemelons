using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator playerAnimator;
    public float interactRadius = 4f;
    public Sprite openSprite;
    public Sprite closedSprite;
    bool isOpen = false;
    bool isPlayerInRange = false;
    public Transform refPoint;
    public Transform playerTransform;
    bool canToggle = false;

    public Collider2D rigidCollider;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sprite = closedSprite;
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
         if (other.gameObject.CompareTag("Player"))
        {   
            isPlayerInRange = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
         if (other.gameObject.CompareTag("Player"))
        {   
            isPlayerInRange = false;
        }
    }
    void Update()
    {
        //SpriteRender
        


        //check if can toggle door
        if (isPlayerInRange)
        {
            if (playerTransform.position.y < refPoint.position.y)
            {
                if (playerAnimator.GetFloat("Vertical")>0 && MathF.Abs(playerAnimator.GetFloat("Vertical"))>MathF.Abs(playerAnimator.GetFloat("Horizontal")))
                {
                    canToggle = true;
                }
                else
                {
                    canToggle = false;
                }
            }
            else
            {
                if (playerAnimator.GetFloat("Vertical")<0 && MathF.Abs(playerAnimator.GetFloat("Vertical"))>MathF.Abs(playerAnimator.GetFloat("Horizontal")))
                {
                    canToggle = true;
                }
                else
                {
                    canToggle = false;
                }
            }
        }
        if(canToggle && Input.GetKeyDown("space"))
         {
            ToggleDoor();
         }
    }
    public void ToggleDoor()
    {
        if (isOpen)
        {
            spriteRenderer.sprite = closedSprite;
            rigidCollider.enabled = true;
        }else
        {
            spriteRenderer.sprite = openSprite;
            
            rigidCollider.enabled = false;
        }
        isOpen = !isOpen;
    }
}
