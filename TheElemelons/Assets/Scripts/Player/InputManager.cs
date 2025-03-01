using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player input and key bindings (without referencing any external GameSystem).
/// </summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Dictionary of actions mapped to their key bindings.
    /// </summary>
    [Header("Key Bindings")]
    public Dictionary<string, KeyCode> keyBindings;

    [Header("Internal Stuff")]
    // References to other components
    private PlayerMovement playerMovement;
    /// <summary>
    /// Reference to the Casting component for spell casting functionality.
    /// </summary>
    public Casting casting;
    /// <summary>
    /// Reference to the PlayerInteraction component for interacting with objects.
    /// </summary>
    public PlayerInteraction playerInteraction;

    private Camera mainCamera;
    private CameraMovement cameraMovement;
    public Transform pivot;

    /// <summary>
    /// Initializes the InputManager and references required components.
    /// </summary>
    private void Awake()
    {
        InitializeKeyBindings();
        playerMovement = GetComponent<PlayerMovement>();
        casting = GetComponent<Casting>();
        playerInteraction = GetComponent<PlayerInteraction>();

        // Find the CameraMovement script in the scene
        cameraMovement = FindObjectOfType<CameraMovement>();
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Handles updates for player inputs (movement, camera, interaction, etc.).
    /// </summary>
    private void Update()
    {
        HandleGameplayInputs();
    }

    /// <summary>
    /// Initializes default key bindings for player actions.
    /// </summary>
    private void InitializeKeyBindings()
    {
        // Define default key bindings
        string[] actions = {
            "MoveLeft", "MoveRight", "MoveUp", "MoveDown",
            "CastSpell", "ClearCharges",
            "SelectFire", "SelectWater", "SelectEarth", "SelectAir",
            "ZoomIn", "ZoomOut",
            "Interact"
        };

        KeyCode[] defaultKeys = {
            KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,
            KeyCode.Mouse0, KeyCode.Mouse1,
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            KeyCode.Z, KeyCode.X,
            KeyCode.E
        };

        keyBindings = new Dictionary<string, KeyCode>();

        for (int i = 0; i < actions.Length; i++)
        {
            // Initialize with default keys
            keyBindings[actions[i]] = defaultKeys[i];
        }
    }

    /// <summary>
    /// Updates the key binding for a specified action.
    /// </summary>
    /// <param name="action">The action to update.</param>
    /// <param name="newKey">The new key to bind to the action.</param>
    public void SaveKeyBinding(string action, KeyCode newKey)
    {
        if (keyBindings.ContainsKey(action))
        {
            keyBindings[action] = newKey; // Update the key binding in the dictionary
            Debug.Log($"Key binding for '{action}' updated to: {newKey}");
        }
        else
        {
            Debug.LogError($"Action '{action}' not found in key bindings.");
        }
    }

    /// <summary>
    /// Central method to handle all gameplay-related inputs.
    /// </summary>
    private void HandleGameplayInputs()
    {
        HandleMovementInputs();
        HandleSpellCastingInputs();
        HandleCameraInputs();
        HandleInteractionInputs();
    }

    /// <summary>
    /// Processes player movement inputs.
    /// </summary>
    private void HandleMovementInputs()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(keyBindings["MoveLeft"]))  direction.x -= 1;
        if (Input.GetKey(keyBindings["MoveRight"])) direction.x += 1;
        if (Input.GetKey(keyBindings["MoveUp"]))    direction.y += 1;
        if (Input.GetKey(keyBindings["MoveDown"]))  direction.y -= 1;

        playerMovement?.SetMovement(direction.normalized);
    }

    /// <summary>
    /// Processes spell casting inputs.
    /// </summary>
    private void HandleSpellCastingInputs()
    {
        if (Input.GetKeyDown(keyBindings["CastSpell"]))
        {
            casting?.CastSpell();
        }

        if (Input.GetKeyDown(keyBindings["ClearCharges"]))
        {
            casting?.ClearCharges();
        }

        if (Input.GetKeyDown(keyBindings["SelectFire"]))   casting?.OnSelectCharge(ChargeTypes.fire);
        if (Input.GetKeyDown(keyBindings["SelectWater"]))  casting?.OnSelectCharge(ChargeTypes.water);
        if (Input.GetKeyDown(keyBindings["SelectEarth"]))  casting?.OnSelectCharge(ChargeTypes.earth);
        if (Input.GetKeyDown(keyBindings["SelectAir"]))    casting?.OnSelectCharge(ChargeTypes.wind);
    }

    /// <summary>
    /// Processes camera-related inputs.
    /// </summary>
    private void HandleCameraInputs()
    {
        // Zoom-In and Zoom-Out with Mouse Scroll Wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) // Scroll up
        {
            cameraMovement?.SetCameraSize(cameraMovement.MainCamera.orthographicSize - 1f); // Zoom-In
        }
        else if (scroll < 0f) // Scroll down
        {
            cameraMovement?.SetCameraSize(cameraMovement.MainCamera.orthographicSize + 1f); // Zoom-Out
        }

        // Alternative: KeyBindings for Zoom-In and Zoom-Out
        if (Input.GetKeyDown(keyBindings["ZoomIn"]))
        {
            cameraMovement?.SetCameraSize(3f); // Zoom in
        }
        if (Input.GetKeyDown(keyBindings["ZoomOut"]))
        {
            cameraMovement?.SetCameraSize(8f); // Zoom out
        }
    }

    /// <summary>
    /// Processes interaction inputs.
    /// If the player presses the interaction button, the player will use
    /// its PlayerInteraction script to interact with the closest interactable object
    /// within the interaction range.
    /// </summary>
    private void HandleInteractionInputs()
    {
        if (Input.GetKeyDown(keyBindings["Interact"]))
        {
            if (playerInteraction == null)
            {
                Debug.LogWarning("PlayerInteraction component not found.");
                return;
            }
            playerInteraction?.InteractWithClosestInteractable();
        }
    }

    /// <summary>
    /// Gets the aiming direction based on the mouse position relative to the player.
    /// </summary>
    /// <returns>The angle in degrees.</returns>
    public float GetAimDirection()
    {
        Vector2 mousePos = GetMousePosition();
        Vector2 aimDir = mousePos - new Vector2(pivot.position.x, pivot.position.y);
        return Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg + 90f;
    }

    /// <summary>
    /// Gets the current mouse position in world space.
    /// </summary>
    /// <returns>Vector2 representing mouse position in the world.</returns>
    public Vector2 GetMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Retrieves the key binding for a specified action.
    /// </summary>
    /// <param name="action">The action to retrieve the key binding for.</param>
    /// <returns>The KeyCode associated with the action.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the action does not exist.</exception>
    public KeyCode GetKeyBinding(string action)
    {
        if (keyBindings.ContainsKey(action))
            return keyBindings[action];

        throw new KeyNotFoundException($"No key binding found for action: {action}");
    }
}
