using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    InGame,
    MainMenu,
    SettingsMenu,
    PauseMenu
}

public class InputManager : MonoBehaviour
{
    [Header("Game State")]
    public GameState currentState = GameState.InGame;

    [Header("Key Bindings")]
    public Dictionary<string, KeyCode> keyBindings;

    // References to other components
    private PlayerMovement playerMovement;
    public Casting casting;
    private Camera mainCamera;

    private CameraMovement cameraMovement;

    private void Awake()
    {
        InitializeKeyBindings();
        playerMovement = GetComponent<PlayerMovement>();
        casting = GetComponent<Casting>();
        // Find the CameraMovement script in the scene
        cameraMovement = FindObjectOfType<CameraMovement>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.InGame:
                HandleGameplayInputs();
                break;

            case GameState.MainMenu:
                HandleMenuInputs();
                break;

            case GameState.SettingsMenu:
                HandleSettingsInputs();
                break;

            case GameState.PauseMenu:
                HandlePauseMenuInputs();
                break;
        }

        // Global input: Toggle Pause Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.InGame)
            {
                currentState = GameState.PauseMenu;
                Debug.Log("Game Paused.");
            }
            else if (currentState == GameState.PauseMenu)
            {
                currentState = GameState.InGame;
                Debug.Log("Game Resumed.");
            }
        }
    }

    private void InitializeKeyBindings()
    {
        // Define default key bindings
        string[] actions = { 
            "MoveLeft", "MoveRight", "MoveUp", "MoveDown", 
            "CastSpell", "ClearCharges", 
            "SelectFire", "SelectWater", "SelectEarth", "SelectAir", 
            "ZoomIn", "ZoomOut" 
        };

        KeyCode[] defaultKeys = { 
            KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S, 
            KeyCode.Mouse0, KeyCode.Mouse1, 
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, 
            KeyCode.Z, KeyCode.X 
        };

        keyBindings = new Dictionary<string, KeyCode>();

        for (int i = 0; i < actions.Length; i++)
        {
            // Initialize with default keys
            keyBindings[actions[i]] = defaultKeys[i];
        }
    }

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

    private void HandleGameplayInputs()
    {
        HandleMovementInputs();
        HandleSpellCastingInputs();
        HandleCameraInputs();
    }

    private void HandleMovementInputs()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(keyBindings["MoveLeft"])) direction.x -= 1;
        if (Input.GetKey(keyBindings["MoveRight"])) direction.x += 1;
        if (Input.GetKey(keyBindings["MoveUp"])) direction.y += 1;
        if (Input.GetKey(keyBindings["MoveDown"])) direction.y -= 1;

        playerMovement?.SetMovement(direction.normalized);
    }

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

    private void HandleMenuInputs()
    {
        // Handle Main Menu inputs
        Debug.Log("Main Menu Input Handling");
    }

    private void HandleSettingsInputs()
    {
        // Handle Settings Menu inputs
        Debug.Log("Settings Menu Input Handling");
    }

    private void HandlePauseMenuInputs()
    {
        // Handle inputs in Pause Menu
        Debug.Log("Pause Menu Input Handling");
    }

    public float GetAimDirection()
    {   
        Vector2 playerPosition = playerMovement?.transform.position ?? Vector2.zero;
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = mousePosition - playerPosition;
        return Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    }

    public Vector2 GetMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    public KeyCode GetKeyBinding(string action)
    {
        if (keyBindings.ContainsKey(action))
            return keyBindings[action];
        throw new KeyNotFoundException($"No key binding found for action: {action}");
    }
}
