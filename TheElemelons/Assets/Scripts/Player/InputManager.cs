using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player input and key bindings across different game states.
/// This version defers all state management to GameManager and processes gameplay inputs only when appropriate.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("Key Bindings")]
    public Dictionary<string, KeyCode> keyBindings;

    [Header("Internal Components")]
    private PlayerMovement playerMovement;
    public Casting casting;
    public PlayerInteraction playerInteraction;
    private Camera mainCamera;
    private CameraMovement cameraMovement;
    public Transform pivot;
    
    // Input flags to enable/disable processing.
    public bool canInput = true;
    public bool canMove = true;

    private void Awake()
    {
        InitializeKeyBindings();
        playerMovement = GetComponent<PlayerMovement>();
        casting = GetComponent<Casting>();
        playerInteraction = GetComponent<PlayerInteraction>();
        
        cameraMovement = FindObjectOfType<CameraMovement>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!canInput)
            return;

        // Global input: Toggle Pause Menu.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Query the current state from GameManager
            IGameState currentState = GameManager.Instance.CurrentState;
            if (currentState is InGameState)
            {
                GameManager.Instance.PauseGame();
                Debug.Log("Game Paused via InputManager.");
            }
            else if (currentState is PauseState)
            {
                GameManager.Instance.ContinueGame();
                Debug.Log("Game Resumed via InputManager.");
            }
        }

        // Process gameplay inputs only if the GameManager state is InGameState.
        if (GameManager.Instance.CurrentState is InGameState)
        {
            HandleGameplayInputs();
        }

        // Optional: You can add separate handling for menu inputs, settings, etc.
    }

    /// <summary>
    /// Initializes default key bindings for player actions.
    /// </summary>
    private void InitializeKeyBindings()
    {
        // Define default key bindings.
        string[] actions = {
            "MoveLeft", "MoveRight", "MoveUp", "MoveDown",
            "CastSpell", "ClearCharges",
            "SelectFire", "SelectWater", "SelectEarth", "SelectAir",
            "ZoomIn", "ZoomOut",
            "Interact", "DialogueKey"
        };

        KeyCode[] defaultKeys = {
            KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S,
            KeyCode.Mouse0, KeyCode.Mouse1,
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            KeyCode.Z, KeyCode.X,
            KeyCode.E, KeyCode.Space
        };

        keyBindings = new Dictionary<string, KeyCode>();
        for (int i = 0; i < actions.Length; i++)
        {
            keyBindings[actions[i]] = defaultKeys[i];
        }
    }

    /// <summary>
    /// Updates the key binding for a specified action.
    /// </summary>
    public void SaveKeyBinding(string action, KeyCode newKey)
    {
        if (keyBindings.ContainsKey(action))
        {
            keyBindings[action] = newKey; // Update the key binding.
            Debug.Log($"Key binding for '{action}' updated to: {newKey}");
        }
        else
        {
            Debug.LogError($"Action '{action}' not found in key bindings.");
        }
    }

    /// <summary>
    /// Handles all gameplay-related inputs.
    /// </summary>
    private void HandleGameplayInputs()
    {
        if (canMove)
        {
            HandleMovementInputs();
        }
        HandleSpellCastingInputs();
        HandleCameraInputs();
        HandleInteractionInputs();
        HandleDialogueInput();
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
            casting?.CastSpell();

        if (Input.GetKeyDown(keyBindings["ClearCharges"]))
            casting?.ClearCharges();

        if (Input.GetKeyDown(keyBindings["SelectFire"]))  casting?.OnSelectCharge(ChargeTypes.fire);
        if (Input.GetKeyDown(keyBindings["SelectWater"])) casting?.OnSelectCharge(ChargeTypes.water);
        if (Input.GetKeyDown(keyBindings["SelectEarth"])) casting?.OnSelectCharge(ChargeTypes.earth);
        if (Input.GetKeyDown(keyBindings["SelectAir"]))   casting?.OnSelectCharge(ChargeTypes.wind);
    }

    /// <summary>
    /// Processes camera-related inputs (e.g., zooming).
    /// </summary>
    private void HandleCameraInputs()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            cameraMovement?.SetCameraSize(cameraMovement.MainCamera.orthographicSize - 1f); // Zoom-In
        }
        else if (scroll < 0f)
        {
            cameraMovement?.SetCameraSize(cameraMovement.MainCamera.orthographicSize + 1f); // Zoom-Out
        }
        if (Input.GetKeyDown(keyBindings["ZoomIn"]))
            cameraMovement?.SetCameraSize(3f);
        if (Input.GetKeyDown(keyBindings["ZoomOut"]))
            cameraMovement?.SetCameraSize(8f);
    }

    /// <summary>
    /// Processes interaction inputs.
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
    /// Checks and processes dialogue progression input.
    /// </summary>
    private void HandleDialogueInput()
    {
        if (Input.GetKeyDown(keyBindings["DialogueKey"]))
        {
            DialogueUI ui = FindObjectOfType<DialogueUI>();
            if (ui != null && ui.gameObject.activeSelf)
                ui.SkipTyping();
        }
    }

    /// <summary>
    /// Retrieves the aiming direction in degrees based on the mouse position relative to the pivot.
    /// </summary>
    public float GetAimDirection()
    {
        Vector2 mousePos = GetMousePosition();
        Vector2 aim_dir = mousePos - new Vector2(pivot.position.x, pivot.position.y);
        return Mathf.Atan2(aim_dir.y, aim_dir.x) * Mathf.Rad2Deg + 90f;
    }

    /// <summary>
    /// Retrieves the current mouse position in world coordinates.
    /// </summary>
    public Vector2 GetMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    /// <summary>
    /// Retrieves the key binding for a specified action.
    /// </summary>
    public KeyCode GetKeyBinding(string action)
    {
        if (keyBindings.ContainsKey(action))
            return keyBindings[action];
        throw new KeyNotFoundException($"No key binding found for action: {action}");
    }
}
