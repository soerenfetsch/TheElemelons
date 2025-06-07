using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the functionality of the resurrection button, including its appearance,
/// interaction, and the effects of resurrecting the player.
/// </summary>
public class ResurrectionButton : MonoBehaviour
{
    /// <summary>
    /// Prefab for the visual effect played when the player dies.
    /// This is instantiated when the resurrection UI is enabled.
    /// </summary>
    public DeathEffect deathEffect;
    /// <summary>
    /// Reference to the player's Transform component.
    /// </summary>
    Transform playerTransform;

    [Header("Hovering Motion")]
    /// <summary>
    /// The maximum vertical distance the button will move from its starting point.
    /// </summary>
    public float amplitude = 20f;
    /// <summary>
    /// The speed of the button's hovering oscillation.
    /// </summary>
    public float frequency = 0.25f;
    /// <summary>
    /// The initial position of the button, used as the center for hovering motion.
    /// </summary>
    Vector2 fixpoint;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the button and finds the player's transform.
    /// </summary>
    void Awake()
    {
        InititializeResurrectButton();
        playerTransform = GameObject.Find("Player").transform;
    }

    /// <summary>
    /// Called before the first frame update.
    /// Sets the initial position for the hovering motion.
    /// </summary>
    void Start()
    {
        fixpoint = transform.position;
    }

    /// <summary>
    /// Called when the GameObject becomes enabled and active.
    /// Hides the player and triggers death-related effects.
    /// </summary>
    void OnEnable()
    {
        playerOnOff(false);
    }

    /// <summary>
    /// Called every fixed framerate frame.
    /// Updates the button's position to create a hovering effect.
    /// </summary>
    void FixedUpdate()
    {
        Vector2 moving_by = Vector2.up * Mathf.Sin(frequency * Time.frameCount * Time.fixedDeltaTime) * amplitude;
        transform.position = fixpoint + moving_by;
    }

    /// <summary>
    /// Initializes the resurrection button component and attaches its click listener.
    /// </summary>
    private void InititializeResurrectButton()
    {
        Button button = gameObject.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("ResurrectionButton not found in SettingsMenuUI.");
            return;
        }
        // Add on click listener to return to last menu
        button.onClick.AddListener(() => OnResurrectButtonClicked());
    }

    /// <summary>
    /// Toggles the player's visibility and interactive components.
    /// Also handles instantiation of death effects and clearing buffs/charges when hiding the player.
    /// </summary>
    /// <param name="onOff">True to make the player visible and active, false to hide.</param>
    void playerOnOff(bool onOff)
    {
        playerTransform.GetComponent<SpriteRenderer>().enabled = onOff;
        foreach (BoxCollider2D boxCollider2D in playerTransform.GetComponents<BoxCollider2D>())
        {
            boxCollider2D.enabled = onOff;
        }
        playerTransform.GetComponent<ToolTip>().ToggleToolTipOnOff(onOff);
        playerTransform.GetComponent<Player>().ModifyIsHidden(onOff ? -1 : 1);
        playerTransform.Find("Wand_Animation").gameObject.SetActive(onOff);
        if (!onOff)
        {
            Instantiate(deathEffect, playerTransform.position, Quaternion.Euler(0, 0, 0));
            playerTransform.GetComponent<Casting>().ClearCharges();
            BuffDebuffManager.Instance.ClearEffects(playerTransform.gameObject);
        }
    }

    /// <summary>
    /// Handles the click event for the resurrection button.
    /// Continues the saved game and makes the player visible and active.
    /// </summary>
    private void OnResurrectButtonClicked()
    {
        GameManager.Instance.ContinueSavedGame();
        if (playerTransform != null)
        {
            playerOnOff(true);
        }
    }
}
