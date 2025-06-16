using UnityEngine;
using UnityEngine.UI;

/// <summary>
<<<<<<< Updated upstream
/// Manages the functionality of the resurrection button, including its appearance,
/// interaction, and the effects of resurrecting the player.
=======
/// Controls the resurrection button UI on the death screen.
/// Handles player resurrection, death effect, and player visibility/interactivity.
>>>>>>> Stashed changes
/// </summary>
public class ResurrectionButton : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
    /// Prefab for the visual effect played when the player dies.
    /// This is instantiated when the resurrection UI is enabled.
    /// </summary>
    public DeathEffect deathEffect;
    /// <summary>
    /// Reference to the player's Transform component.
    /// </summary>
=======
    /// Reference to the death effect prefab to spawn when resurrecting.
    /// </summary>
    public DeathEffect deathEffect;

    // Reference to the player's transform.
>>>>>>> Stashed changes
    Transform playerTransform;

    [Header("Hovering Motion")]
    /// <summary>
<<<<<<< Updated upstream
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
=======
    /// Amplitude of the button's hovering animation.
    /// </summary>
    public float amplitude = 20f;

    /// <summary>
    /// Frequency of the button's hovering animation.
    /// </summary>
    public float frequency = 0.25f;

    // The fixed anchor position for the button's hovering animation.
    Vector2 fixpoint;

    /// <summary>
    /// Unity Awake method. Initializes the button and finds the player transform.
>>>>>>> Stashed changes
    /// </summary>
    void Awake()
    {
        InititializeResurrectButton();
        playerTransform = GameObject.Find("Player").transform;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Called before the first frame update.
    /// Sets the initial position for the hovering motion.
=======
    /// Unity Start method. Stores the button's initial position for hovering.
>>>>>>> Stashed changes
    /// </summary>
    void Start()
    {
        fixpoint = transform.position;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Called when the GameObject becomes enabled and active.
    /// Hides the player and triggers death-related effects.
=======
    /// Unity OnEnable method. Hides the player and disables interaction when the button appears.
>>>>>>> Stashed changes
    /// </summary>
    void OnEnable()
    {
        playerOnOff(false);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Called every fixed framerate frame.
    /// Updates the button's position to create a hovering effect.
=======
    /// Unity FixedUpdate method. Animates the button with a hovering motion.
>>>>>>> Stashed changes
    /// </summary>
    void FixedUpdate()
    {
        Vector2 moving_by = Vector2.up * Mathf.Sin(frequency * Time.frameCount * Time.fixedDeltaTime) * amplitude;
        transform.position = fixpoint + moving_by;
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Initializes the resurrection button component and attaches its click listener.
=======
    /// Initializes the resurrection button and attaches the click event.
>>>>>>> Stashed changes
    /// </summary>
    private void InititializeResurrectButton()
    {
        Button button = gameObject.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("ResurrectionButton not found in SettingsMenuUI.");
            return;
        }
        // Add on click listener to handle resurrection
        button.onClick.AddListener(() => OnResurrectButtonClicked());
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Toggles the player's visibility and interactive components.
    /// Also handles instantiation of death effects and clearing buffs/charges when hiding the player.
    /// </summary>
    /// <param name="onOff">True to make the player visible and active, false to hide.</param>
=======
    /// Enables or disables the player sprite, colliders, tooltip, and wand animation.
    /// Also spawns the death effect and clears buffs/debuffs when disabling.
    /// </summary>
    /// <param name="onOff">True to enable player, false to disable.</param>
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Handles the click event for the resurrection button.
    /// Continues the saved game and makes the player visible and active.
=======
    /// Handles the resurrection button click: continues the saved game and restores player visibility.
>>>>>>> Stashed changes
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
