using System.Collections.Generic;
using UnityEngine;

/// <summary>
<<<<<<< Updated upstream
/// Controls the animated background of the main menu, including the witch, magic circle, and button effects.
/// Moves and scales background elements based on mouse position for a dynamic visual effect.
=======
/// Controls the animated background for the main menu, including the witch and magic circle.
/// Responds to mouse movement and button positions to create a dynamic parallax effect.
>>>>>>> Stashed changes
/// </summary>
public class MainMenuBackground : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
    /// Reference to the witch RectTransform.
=======
    /// Reference to the witch's RectTransform.
>>>>>>> Stashed changes
    /// </summary>
    RectTransform witch;

    /// <summary>
<<<<<<< Updated upstream
    /// The anchor position of the witch.
=======
    /// The anchor (original) position of the witch.
>>>>>>> Stashed changes
    /// </summary>
    Vector3 witchAnchor;

    /// <summary>
<<<<<<< Updated upstream
    /// Reference to the magic circle RectTransform.
=======
    /// Reference to the magic circle's RectTransform.
>>>>>>> Stashed changes
    /// </summary>
    RectTransform magicCircle;

    /// <summary>
<<<<<<< Updated upstream
    /// The anchor position of the magic circle.
=======
    /// The anchor (original) position of the magic circle.
>>>>>>> Stashed changes
    /// </summary>
    Vector3 magicCircleAnchor;

    /// <summary>
<<<<<<< Updated upstream
    /// List of button transforms in the menu.
=======
    /// List of button transforms in the main menu.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Unity Start method. Initializes background elements and references.
=======
    /// Unity Start method. Initializes references and anchor positions.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Initializes references to background elements and menu buttons.
=======
    /// Initializes references to UI elements, anchor positions, and button list.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Unity FixedUpdate method. Updates mouse position and animates background elements.
=======
    /// Unity FixedUpdate method. Updates mouse position and animates the background.
>>>>>>> Stashed changes
    /// </summary>
    void FixedUpdate()
    {
        GetMousePos();
        MoveBackground();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Maps a value using an arctangent function for smooth scaling.
=======
    /// Smoothly maps a value x to a range [min, max] using an arctangent function.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Smoothly interpolates between two values with a maximum speed.
    /// </summary>
    /// <param name="previous">Previous value.</param>
    /// <param name="next">Target value.</param>
    /// <param name="speed">Maximum speed of change.</param>
=======
    /// Smoothly interpolates between previous and next values, limited by speed.
    /// </summary>
    /// <param name="previous">Previous value.</param>
    /// <param name="next">Target value.</param>
    /// <param name="speed">Maximum change per second.</param>
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Animates the background elements (witch and magic circle) based on mouse position.
=======
    /// Animates the witch and magic circle based on mouse position and button locations.
    /// Creates a parallax and scaling effect for the main menu background.
>>>>>>> Stashed changes
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
