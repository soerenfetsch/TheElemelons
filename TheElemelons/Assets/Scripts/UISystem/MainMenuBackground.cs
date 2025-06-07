using System.Collections;
using System.Collections.Generic;
using Codice.CM.Client.Gui;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controls the animated background of the main menu, including the witch, magic circle, and button effects.
/// Moves and scales background elements based on mouse position for a dynamic visual effect.
/// </summary>
public class MainMenuBackground : MonoBehaviour
{
    /// <summary>
    /// Reference to the witch RectTransform.
    /// </summary>
    RectTransform witch;

    /// <summary>
    /// The anchor position of the witch.
    /// </summary>
    Vector3 witchAnchor;

    /// <summary>
    /// Reference to the magic circle RectTransform.
    /// </summary>
    RectTransform magicCircle;

    /// <summary>
    /// The anchor position of the magic circle.
    /// </summary>
    Vector3 magicCircleAnchor;

    /// <summary>
    /// List of button transforms in the menu.
    /// </summary>
    List<Transform> buttons;

    /// <summary>
    /// Current mouse position in world coordinates.
    /// </summary>
    Vector3 mousePos;

    /// <summary>
    /// Reference to the main camera.
    /// </summary>
    Camera mainCamera;

    /// <summary>
    /// Unity Start method. Initializes background elements and references.
    /// </summary>
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Updates the mouse position in world coordinates.
    /// </summary>
    void GetMousePos()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Initializes references to background elements and menu buttons.
    /// </summary>
    void Initialize()
    {
        witch = transform.GetChild(2).GetComponent<RectTransform>();
        magicCircle = transform.GetChild(1).GetComponent<RectTransform>();
        magicCircleAnchor = magicCircle.position;
        witchAnchor = witch.position;
        mainCamera = Camera.main;
        buttons = new List<Transform>();
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Transform potentialButton = transform.parent.GetChild(i).transform;
            if (potentialButton.gameObject.name.Contains("Button"))
            {
                buttons.Add(potentialButton);
            }
        }
    }

    /// <summary>
    /// Unity FixedUpdate method. Updates mouse position and animates background elements.
    /// </summary>
    void FixedUpdate()
    {
        GetMousePos();
        MoveBackground();
    }

    /// <summary>
    /// Maps a value using an arctangent function for smooth scaling.
    /// </summary>
    /// <param name="x">Input value.</param>
    /// <param name="min">Minimum output value.</param>
    /// <param name="max">Maximum output value.</param>
    /// <returns>Mapped value between min and max.</returns>
    float f(float x, float min, float max)
    {
        return (Mathf.Atan(x) * (2 / Mathf.PI) * ((max - min) / 2)) + min;
    }

    /// <summary>
    /// Smoothly interpolates between two values with a maximum speed.
    /// </summary>
    /// <param name="previous">Previous value.</param>
    /// <param name="next">Target value.</param>
    /// <param name="speed">Maximum speed of change.</param>
    /// <returns>New interpolated value.</returns>
    float changeSpeed(float previous, float next, float speed)
    {
        float diff = next - previous;
        float maxChange = Time.fixedDeltaTime * speed;
        if (Mathf.Abs(diff) > maxChange)
        {
            diff = Mathf.Sign(diff) * maxChange;
        }
        return previous + diff;
    }

    /// <summary>
    /// Animates the background elements (witch and magic circle) based on mouse position.
    /// </summary>
    void MoveBackground()
    {
        Vector3 moveFactor = new Vector3(300f, -200f, 0);
        float maxWitchScale = 1.5f;
        float minWitchScale = 1.2f;
        Vector3 diff = mousePos - mainCamera.ScreenToWorldPoint(buttons[0].transform.position);
        float witchScale = f(diff.x, minWitchScale, maxWitchScale);
        witchScale = changeSpeed(witch.localScale.x, witchScale, 1f);
        witch.localScale = new Vector3(witchScale, witchScale, 1);
        witch.position = witchAnchor + (witchScale - minWitchScale) / (maxWitchScale - minWitchScale) * moveFactor * (2f / 3f);
        magicCircle.position = magicCircleAnchor - (witchScale - 1f) / (maxWitchScale - 1f) * moveFactor;
        float circleScale = 1f - (maxWitchScale - witchScale);
        magicCircle.localScale = new Vector3(circleScale, circleScale, 1);
    }
}
