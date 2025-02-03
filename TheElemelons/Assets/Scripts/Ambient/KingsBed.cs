using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KingsBed : MonoBehaviour
{
    public float opacityPlayerBehind = 0.35f;
    SpriteRenderer spriteRendererRoof;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRendererRoof = GameObject.Find("KingsBed/Roof").GetComponent<SpriteRenderer>();
        SetAlpha(spriteRendererRoof,1.0f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.isTrigger)
        {
            SetAlpha(spriteRendererRoof,opacityPlayerBehind);
        }
    }
    private void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.CompareTag("Player") && other.isTrigger)
        {   
            SetAlpha(spriteRendererRoof,1.0f);
        }
    }
    public void SetAlpha(SpriteRenderer spriteRenderer, float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}
