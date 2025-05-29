using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Regex = System.Text.RegularExpressions.Regex;

public enum TutorialTip
{
    None,
    Shoot,
    ClearSpell,
    Walk,
    Interact,
    SelectElement,
    SelectElements,
    CastElements,
    FieldOfView
}
public class ToolTip : MonoBehaviour
{
    Camera mainCamera;
    Vector3 toolTipAnchor;
    List<KeyCode> keyToPress;
    public bool canInteract;
    TutorialTip currentTip;
    public TMP_Text tooltip;
    InputManager inputManager;
    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        mainCamera = Camera.main;
        toolTipAnchor = tooltip.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        string text = "";
        if (currentTip != TutorialTip.None)
        {
            switch (currentTip)
            {
                case TutorialTip.Shoot:
                    text = "Cast Spell with " + GetKeyBind("CastSpell");
                    keyToPress = new List<KeyCode>() { inputManager.keyBindings["CastSpell"] };
                    break;
                case TutorialTip.ClearSpell:
                    text = "Clear Spell Selection with " + GetKeyBind("ClearCharges");
                    keyToPress = new List<KeyCode>() { inputManager.keyBindings["ClearCharges"] };
                    break;
                case TutorialTip.Walk:
                    text = "Walk: [" + GetKeyBind("MoveUp") + "][" + GetKeyBind("MoveLeft") +
                    "][" + GetKeyBind("MoveDown") + "][" + GetKeyBind("MoveRight") + "]" + "\n Zoom: [MouseWheel]";
                    keyToPress = new List<KeyCode>() { inputManager.keyBindings["MoveUp"],
                    inputManager.keyBindings["MoveDown"],inputManager.keyBindings["MoveLeft"],
                    inputManager.keyBindings["MoveRight"]};
                    break;
                case TutorialTip.SelectElement:
                    text = "Select elemental charge: [" + GetKeyBind("SelectFire") + "][" + GetKeyBind("SelectWater") + "][" +
                    GetKeyBind("SelectEarth") + "][" + GetKeyBind("SelectAir") + "]";
                    keyToPress = new List<KeyCode>(){ inputManager.keyBindings["SelectFire"],inputManager.keyBindings["SelectWater"],
                    inputManager.keyBindings["SelectEarth"],inputManager.keyBindings["SelectAir"]};
                    break;
                case TutorialTip.SelectElements:
                    text = "Select elemental charge: [" + GetKeyBind("SelectFire") + "][" + GetKeyBind("SelectWater") + "][" +
                    GetKeyBind("SelectEarth") + "][" + GetKeyBind("SelectAir") + "]";
                    keyToPress = new List<KeyCode>(){ inputManager.keyBindings["SelectFire"],inputManager.keyBindings["SelectWater"],
                    inputManager.keyBindings["SelectEarth"],inputManager.keyBindings["SelectAir"]};
                    break;
                case TutorialTip.CastElements:
                    text = "Cast Spell with " + GetKeyBind("CastSpell") + "\n" +
                    "Clear Spell Selection with " + GetKeyBind("ClearCharges"); ;
                    keyToPress = new List<KeyCode>() { inputManager.keyBindings["CastSpell"], inputManager.keyBindings["ClearCharges"] };
                    break;
                default:
                    keyToPress = new List<KeyCode>();
                    text = "";
                    break;
            }
            if (text == "" || keyToPress == null || keyToPress.Count == 0)
            {
                OnTutorialSuccess();
            }
            else
            {
                foreach (KeyCode key in keyToPress)
                {
                    if (Input.GetKeyDown(key))
                    {
                        if (currentTip == TutorialTip.SelectElements)
                        {
                            ChangeCurrentTip(TutorialTip.SelectElement);
                        }
                        else if (currentTip == TutorialTip.SelectElement)
                        {
                            ChangeCurrentTip(TutorialTip.CastElements);
                        }
                        else
                        {
                            text = OnTutorialSuccess();
                        }
                    }
                }
            }

        }
        else if (canInteract)
        {
            text = "Interact [" + inputManager.keyBindings["Interact"].ToString() + "]";
        }
        else
        {
            text = "";
        }
        tooltip.text = text;
        tooltip.fontSize = 4;
        int lines = Regex.Matches(text, "\n").Count;
        tooltip.transform.localPosition = toolTipAnchor + (Vector3)(Vector2.up * lines);
        if ((currentTip != TutorialTip.Walk && lines > 0 && mainCamera.orthographicSize < 6f) ||
        (currentTip == TutorialTip.Walk && mainCamera.orthographicSize < 5f))
        {
            tooltip.fontSize = 2f;
        }
    }
    string OnTutorialSuccess()
    {
        ChangeCurrentTip(TutorialTip.None);
        return "";
    }
    public void ChangeCurrentTip(TutorialTip tip)
    {
        currentTip = tip;
        Debug.Log(currentTip.ToString());
    }
    string GetKeyBind(string name)
    {
        KeyCode key = inputManager.keyBindings[name];
        if (key.ToString().Contains("Alpha"))
        {
            return key.ToString().Remove(0, 5);
        }
        else if (key.ToString().Contains("Mouse"))
        {
            if (key == KeyCode.Mouse0)
            {
                return "left mouse button";
            }
            else if (key == KeyCode.Mouse1)
            {
                return "right mouse button";
            }
            return key.ToString();
        }
        return key.ToString();
    }
}
