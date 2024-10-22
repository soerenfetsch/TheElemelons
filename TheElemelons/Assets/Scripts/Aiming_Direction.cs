using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming_Direction : MonoBehaviour
{
    Vector2 mousePos;
    public Camera cam;
    public Transform pivot; 
    // Start is called before the first frame update
    public Transform aim_transform;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        Vector2 aim_dir = mousePos-new Vector2(pivot.position.x,pivot.position.y);
        float angle = Mathf.Atan2(aim_dir.y,aim_dir.x)*Mathf.Rad2Deg +90f;
        aim_transform.rotation = Quaternion.Euler(0,0,angle);
        
        //aim_transform.Rotate(0,0,angle);
    }
}
