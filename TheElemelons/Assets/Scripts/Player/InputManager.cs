using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player input and key bindings across different game states.
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
    public bool canInput = true;
    public bool canMove = true;
    private GameStatus currentGameState;
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

    private void OnEnable()
    {
        GameManager.OnGameStatusChanged += HandleGameStatusChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStatusChanged -= HandleGameStatusChanged;
    }

    private void HandleGameStatusChanged(GameStatus newStatus)
    {
        currentGameState = newStatus;
        canInput = newStatus == GameStatus.INGAME || newStatus == GameStatus.DIALOGUE;
        canMove = newStatus == GameStatus.INGAME;
    }

    /// <summary>
    /// Handles updates for different game states and global inputs.
    /// </summary>
    private void Update()
    {
        HandleGlobalInputs(); // Seperate global inputs from state specific inputs
        if (!canInput) return;

        switch (currentGameState)
        {
            case GameStatus.INGAME:
                HandleGameplayInputs();
                break;

            case GameStatus.MAIN_MENU:
                HandleMenuInputs();
                break;

            case GameStatus.SETTINGS_MENU:
                HandleSettingsInputs();
                break;

            case GameStatus.PAUSE_MENU:
                HandlePauseMenuInputs();
                break;
            case GameStatus.DIALOGUE:
                HandleDialogueInputs();
                break;
            case GameStatus.DEATH_SCREEN:
                HandleDeathScreenInputs();
                break;
        }
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
    /// Handles gameplay-related inputs.
    /// </summary>
    private void HandleGameplayInputs()
    {
        if (canMove)
        {
            HandleMovementInputs();
            HandleSpellCastingInputs();
            HandleInteractionInputs();
        }
        HandleCameraInputs();
    }

    /// <summary>
    /// Processes player movement inputs.
    /// </summary>
    private void HandleMovementInputs()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(keyBindings["MoveLeft"])) direction.x -= 1;
        if (Input.GetKey(keyBindings["MoveRight"])) direction.x += 1;
        if (Input.GetKey(keyBindings["MoveUp"])) direction.y += 1;
        if (Input.GetKey(keyBindings["MoveDown"])) direction.y -= 1;

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

        if (Input.GetKeyDown(keyBindings["SelectFire"])) casting?.OnSelectCharge(ChargeTypes.fire);
        if (Input.GetKeyDown(keyBindings["SelectWater"])) casting?.OnSelectCharge(ChargeTypes.water);
        if (Input.GetKeyDown(keyBindings["SelectEarth"])) casting?.OnSelectCharge(ChargeTypes.earth);
        if (Input.GetKeyDown(keyBindings["SelectAir"])) casting?.OnSelectCharge(ChargeTypes.wind);
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
    /// Handles main menu inputs.
    /// </summary>
    private void HandleMenuInputs()
    {
        Debug.Log("Main Menu Input Handling");
        canMove = false;
        canInput = false;
    }

    /// <summary>
    /// Handles settings menu inputs.
    /// </summary>
    private void HandleSettingsInputs()
    {
        Debug.Log("Settings Menu Input Handling");
        canMove = false;
        canInput = false;
    }

    /// <summary>
    /// Handles pause menu inputs.
    /// </summary>
    private void HandlePauseMenuInputs()
    {
        Debug.Log("Pause Menu Input Handling");
        canMove = false;
        canInput = false;
    }

    /// <summary>
    /// Handles dialogue inputs.
    /// </summary>
    private void HandleDialogueInputs()
    {
        if (Input.GetKeyDown(keyBindings["Interact"]))
        {
            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
            dialogueUI?.SkipTyping();
        }
    }

    /// <summary>
    /// Handles death screen inputs.
    /// </summary>
    private void HandleDeathScreenInputs()
    {
        Debug.Log("Death Screen Input Handling");
        canMove = false;
        canInput = false;
    }

    /// <summary>
    /// Gets the aiming direction based on the mouse position relative to the player.
    /// </summary>
    /// <returns>The angle in degrees.</returns>
    public float GetAimDirection()
    {
        Vector2 mousePos = GetMousePosition();
        Vector2 aim_dir = mousePos - new Vector2(pivot.position.x, pivot.position.y);
        return Mathf.Atan2(aim_dir.y, aim_dir.x) * Mathf.Rad2Deg + 90f;
    }

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

    /// <summary>
    /// Starts the rebind key process for a specified action.
    /// </summary>
    /// <param name="action">The action to rebind.</param>
    /// <param name="onRebindComplete">Callback to execute when the rebind is complete.</param>
    public void StartRebinding(string action, Action<KeyCode> onRebindComplete)
    {
        StartCoroutine(WaitForKeyPress(action, onRebindComplete));
    }

    private IEnumerator WaitForKeyPress(string action, Action<KeyCode> onRebindComplete)
    {
        Debug.Log($"Waiting for new key binding for action: {action}");

        // Wait until a valid key is pressed
        bool keyDetected = false;
        while (!keyDetected)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    Debug.Log($"New key '{keyCode}' pressed for action '{action}'");
                    SaveKeyBinding(action, keyCode);
                    onRebindComplete?.Invoke(keyCode);
                    keyDetected = true;
                    break;
                }
            }
            yield return null;
        }
    }
    /// Handles global inputs, such as toggling the pause menu.
    /// </summary>
    private void HandleGlobalInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameStatus.INGAME)
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.PAUSE_MENU);
                Debug.Log("Game Paused.");
            }
            else if (currentGameState == GameStatus.PAUSE_MENU)
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
                Debug.Log("Game Resumed.");
            }
        }
    }
}
