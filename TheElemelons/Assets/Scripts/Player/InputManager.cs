using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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
    private Dictionary<string, KeyCode> keyBindings;

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

        // Globaler Input: Wechsel ins Pause-Menü
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.InGame)
            {
                currentState = GameState.PauseMenu;
                Debug.Log("Spiel pausiert.");
            }
            else if (currentState == GameState.PauseMenu)
            {
                currentState = GameState.InGame;
                Debug.Log("Spiel fortgesetzt.");
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
            keyBindings[actions[i]] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(actions[i], defaultKeys[i].ToString()));
        }
    }

    public void SaveKeyBinding(string action, KeyCode newKey)
    {
        if (keyBindings.ContainsKey(action))
        {
            keyBindings[action] = newKey;
            PlayerPrefs.SetString(action, newKey.ToString());
            PlayerPrefs.Save();
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
        // Zoom-In und Zoom-Out mit Maus-Scroll-Rad
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // mainCamera ist private, muss daher über ein Property (Gleicht Getter ab) abgerufen werden
        if (scroll > 0f) // Mausrad nach oben
        {
            cameraMovement?.SetCameraSize(cameraMovement.MainCamera.orthographicSize - 1f); // Zoom-In
        }
        else if (scroll < 0f) // Mausrad nach unten
        {
            cameraMovement?.SetCameraSize(cameraMovement.MainCamera.orthographicSize + 1f); // Zoom-Out
        }

        // Alternative: KeyBindings für Zoom-In und Zoom-Out
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
        // Inputs im Pause-Menü
        Debug.Log("Pause Menu Input Handling");
    }

    public float GetAimDirection()
    {
        Vector2 playerPosition = playerMovement?.transform.position ?? Vector2.zero;
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = mousePosition - playerPosition;
        return Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    }

    public KeyCode GetKeyBinding(string action)
    {
        if (keyBindings.ContainsKey(action))
            return keyBindings[action];
        throw new KeyNotFoundException($"No key binding found for action: {action}");
    }
}