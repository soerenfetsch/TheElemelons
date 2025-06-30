using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the resurrection button UI on the death screen.
/// Handles player resurrection, death effect, and player visibility/interactivity.
/// </summary>
public class ResurrectionButton : MonoBehaviour
{
    /// <summary>
    /// Reference to the death effect prefab to spawn when resurrecting.
    /// </summary>
    public DeathEffect deathEffect;

    // Reference to the player's transform.
    Transform playerTransform;

    [Header("Hovering Motion")]
    /// <summary>
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
    /// </summary>
    void Awake()
    {
        InititializeResurrectButton();
        playerTransform = GameObject.Find("Player").transform;
    }

    /// <summary>
    /// Unity Start method. Stores the button's initial position for hovering.
    /// </summary>
    void Start()
    {
        fixpoint = transform.position;
    }

    /// <summary>
    /// Unity OnEnable method. Hides the player and disables interaction when the button appears.
    /// </summary>
    void OnEnable()
    {
        playerOnOff(false);
    }

    /// <summary>
    /// Unity FixedUpdate method. Animates the button with a hovering motion.
    /// </summary>
    void FixedUpdate()
    {
        Vector2 moving_by = Vector2.up * Mathf.Sin(frequency * Time.frameCount * Time.fixedDeltaTime) * amplitude;
        transform.position = fixpoint + moving_by;
    }

    /// <summary>
    /// Initializes the resurrection button and attaches the click event.
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
    /// Enables or disables the player sprite, colliders, tooltip, and wand animation.
    /// Also spawns the death effect and clears buffs/debuffs when disabling.
    /// </summary>
    /// <param name="onOff">True to enable player, false to disable.</param>
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
    /// Handles the resurrection button click: continues the saved game and restores player visibility.
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
