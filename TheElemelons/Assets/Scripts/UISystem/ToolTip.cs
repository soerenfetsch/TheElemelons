using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Regex = System.Text.RegularExpressions.Regex;

/// <summary>
/// Enum representing the different types of tutorial tips that can be shown in the tooltip.
/// </summary>
public enum TutorialTip
{
    /// <summary>
    /// No tutorial tip is active.
    /// </summary>
    None,
    /// <summary>
    /// Tip for shooting or casting a spell.
    /// </summary>
    Shoot,
    /// <summary>
    /// Tip for clearing the current spell selection.
    /// </summary>
    ClearSpell,
    /// <summary>
    /// Tip for player movement.
    /// </summary>
    Walk,
    /// <summary>
    /// Tip for interacting with objects.
    /// </summary>
    Interact,
    /// <summary>
    /// Tip for selecting a single element.
    /// </summary>
    SelectElement,
    /// <summary>
    /// Tip for selecting multiple elements (initial state before selecting one).
    /// </summary>
    SelectElements,
    /// <summary>
    /// Tip for casting a spell after selecting elements.
    /// </summary>
    CastElements,
    /// <summary>
    /// Tip related to the field of view or camera.
    /// </summary>
    FieldOfView
}

/// <summary>
/// Makes tooltips appear and disappear under certain conditions.
/// If Tooltip receives a tutorial tip type from tooltiptrigger, the corresponding tip is displayed
/// (i.e. “Move: [W][A][S][D]”). The tooltip stays visible until the player inputs one of the described keys,
/// until a new tooltip is triggered or until he dies.
/// The interact tip appears whenever the player can interact with an object and
/// isn't provided by tooltiptrigger. It only disappears if
/// the player can’t interact anymore or if another tutorial tip type has been set
/// </summary>
public class ToolTip : MonoBehaviour
{
    // Reference to the main camera.
    Camera mainCamera;
    // Anchor position for the tooltip.
    Vector3 toolTipAnchor;
    // List of keys the player must press to clear the current tip.
    List<KeyCode> keyToPress;
    /// <summary>
    /// Whether the player can currently interact with something.
    /// </summary>
    public bool canInteract;
    // The current tutorial tip being displayed.
    TutorialTip currentTip;
    /// <summary>
    /// Reference to the TMP_Text component used for displaying the tooltip.
    /// </summary>
    public TMP_Text tooltip;
    // Reference to the InputManager for key binding lookups.
    InputManager inputManager;

    /// <summary>
    /// Unity Start method. Initializes references and anchor position.
    /// </summary>
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        mainCamera = Camera.main;
        toolTipAnchor = tooltip.transform.localPosition;
    }

    /// <summary>
    /// Enables or disables the tooltip display.
    /// </summary>
    /// <param name="onOff">True to show the tooltip, false to hide it.</param>
    public void ToggleToolTipOnOff(bool onOff)
    {
        tooltip.transform.parent.gameObject.SetActive(onOff);
    }

    /// <summary>
    /// Updates the tooltip each frame based on the current tutorial tip, interaction state, and camera zoom.
    /// </summary>
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

    /// <summary>
    /// Removes the current tutorial tip.
    /// </summary>
    /// <returns>Empty string (for clearing the tooltip).</returns>
    string OnTutorialSuccess()
    {
        ChangeCurrentTip(TutorialTip.None);
        return "";
    }

    /// <summary>
    /// Changes the current tutorial tip being displayed.
    /// </summary>
    /// <param name="tip">The new tutorial tip to display.</param>
    public void ChangeCurrentTip(TutorialTip tip)
    {
        currentTip = tip;
        Debug.Log(currentTip.ToString());
    }

    /// <summary>
    /// Returns a user-friendly string for the key binding of a given action.
    /// </summary>
    /// <param name="name">The action name.</param>
    /// <returns>Formatted key binding string.</returns>
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
