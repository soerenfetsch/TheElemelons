using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHurtbox : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D sc = gameObject.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
        sc.size = new Vector2(0.8931981f,1.579784f);
        sc.offset = new Vector2(-0.4195804f,0.6255565f);
        sc.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}