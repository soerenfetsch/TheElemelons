using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    PlayerMovement playerMovement;
    public Camera cam; 
    UnityEngine.Vector2 mousePos; 
    
    // Start is called before the first frame update
    void Start()
    {
        keys.Add("moveLeft",KeyCode.A);
        keys.Add("moveRight",KeyCode.D);
        keys.Add("moveDown",KeyCode.S);
        keys.Add("moveUp",KeyCode.W);

        keys.Add("interact",KeyCode.Space);

        keys.Add("castSpell",KeyCode.Mouse0);
        keys.Add("clearSpell",KeyCode.Mouse1); 

        keys.Add("selectFire",KeyCode.Alpha1);
        keys.Add("selectWater",KeyCode.Alpha2);
        keys.Add ("selectEarth",KeyCode.Alpha3);
        keys.Add("selectAir",KeyCode.Alpha4);

        playerMovement = GetComponent<PlayerMovement>();
        cam =  Camera.main;
        mousePos = UnityEngine.Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition); 
        playerMovement.mousePos = mousePos;
        HandleHorizontalVerticalMovementInput();
    }

    void HandleHorizontalVerticalMovementInput()
    {
        Vector2 newMovement = playerMovement.GetMovement();
        //Horizontal Movement
        if(Input.GetKeyDown(keys["moveLeft"]))
        {
            newMovement.x = -1;
        }else if(Input.GetKeyUp(keys["moveLeft"]) && Input.GetKey(keys["moveRight"]))
        {
            newMovement.x = 1;
        }
        if(Input.GetKeyDown(keys["moveRight"]))
        {
            newMovement.x = 1;
        }else if(Input.GetKeyUp(keys["moveRight"]) && Input.GetKey(keys["moveLeft"]))
        {
            newMovement.x = -1;
        }
        if (!Input.GetKey(keys["moveLeft"]) 
        && !Input.GetKey(keys["moveRight"]))
        {
            newMovement.x = 0;
        }

        //Vertical Movement
        if(Input.GetKeyDown(keys["moveDown"]))
        {
            newMovement.y = -1;
        }else if(Input.GetKeyUp(keys["moveDown"]) && Input.GetKey(keys["moveUp"]))
        {
            newMovement.y = 1;
        }
        if(Input.GetKeyDown(keys["moveUp"]))
        {
            newMovement.y = 1;
        }else if(Input.GetKeyUp(keys["moveUp"]) && Input.GetKey(keys["moveDown"]))
        {
            newMovement.y = -1;
        }
        if (!Input.GetKey(keys["moveDown"]) 
        && !Input.GetKey(keys["moveUp"]))
        {
            newMovement.y = 0;
        }
        playerMovement.SetMovement(newMovement);
    }
    
}
