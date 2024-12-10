using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public enum GameState
{
    InGame,
    MainMenu,
    SettingsMenu
}

public class InputManager : MonoBehaviour
{
    [Header("Game State")]
    public GameState currentState = GameState.InGame;

    [Header("Key Bindings")]

    // Dynamically managed key bindings
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    private PlayerMovement playerMovement;
    private Casting casting;
    private Camera cam;

    void Start()
    {
        // At startup, the game checks if there are saved key bindings in PlayerPrefs. 
        // If bindings exist, it loads them into the dictionary; otherwise, it uses default bindings.
        InitializeKeyBindings();
        playerMovement = GetComponent<PlayerMovement>();
        casting = GetComponent<Casting>();
        cam = Camera.main;
    }

    void Update()
    {
        // TODO: Add MainMenu and SettingsMenu state!
        if (currentState != GameState.InGame) return;

        HandleMovementInput();
        HandleSpellCastingInput();
    }

    // Initializes the default key bindings or loads them from PlayerPrefs.
    private void InitializeKeyBindings()
    {
        string[] actions = { "MoveLeft", "MoveRight", "MoveUp", "MoveDown", "CastSpell", "ClearCharges", "SelectFire", "SelectWater", "SelectEarth", "SelectAir" };
        KeyCode[] defaultKeys = { KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        for (int i = 0; i < actions.Length; i++)
        {
            // Retrieve the key binding for the current action from PlayerPrefs. If no binding is found, use the default key.
            // Convert the retrieved string to a KeyCode enum and store it in the keys dictionary.
            keys[actions[i]] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(actions[i], defaultKeys[i].ToString()));
            // If the player has saved KeyCode.LeftArrow for this action, the game will load that instead.
        }
    }

    // Saves/Updates key binding to PlayerPrefs.
    // <param name="action">The action to bind.</param>
    // <param name="newKey">The new key to bind to the action.</param>
    public void SaveKeyBinding(string action, KeyCode newKey)
    {
        if (keys.ContainsKey(action))
        {
            keys[action] = newKey;
            // PlayerPrefs is a Unity class used to store and access player preferences between game sessions.
            // It allows saving and loading data such as key bindings, settings, and other preferences.
            // Data is stored in a persistent storage and can be retrieved using a key.
            PlayerPrefs.SetString(action, newKey.ToString());
            PlayerPrefs.Save();
            // After updating the dictionary, the new binding is saved in PlayerPrefs.
        }
    }

    // Handles player movement input and passes it to PlayerMovement.
    private void HandleMovementInput()
    {
        Vector2 moveDirection = Vector2.zero;

        // The Movement is smooth and does not stop when both Movement Keys are pressed for the same axis. (e.g., Up and Down)

        if (Input.GetKey(keys["MoveLeft"]))
            moveDirection.x -= 1;
            else if (Input.GetKey(keys["MoveLeft"]) && Input.GetKey(keys["MoveRight"]))
                moveDirection.x = 1;
        if (Input.GetKey(keys["MoveRight"]))
            moveDirection.x += 1;
            else if (Input.GetKey(keys["MoveRight"]) && Input.GetKey(keys["MoveLeft"]))
                moveDirection.x = -1;
        if (Input.GetKey(keys["MoveUp"]))
            moveDirection.y += 1;
            else if (Input.GetKey(keys["MoveUp"]) && Input.GetKey(keys["MoveDown"]))
                moveDirection.y = -1;
        if (Input.GetKey(keys["MoveDown"]))
            moveDirection.y -= 1;
            else if (Input.GetKey(keys["MoveDown"]) && Input.GetKey(keys["MoveUp"]))
                moveDirection.y = 1;

        playerMovement?.SetMovement(moveDirection);
    }

    // Handles spell casting and charge selection input.
    private void HandleSpellCastingInput()
    {
        
        if (Input.GetKeyDown(keys["CastSpell"]))
        {
            // The ?. operator is the null-conditional operator. It ensures that the CastSpell method is only called if the casting object is not null.
            // If casting is null, the expression evaluates to null and CastSpell is not called, preventing a potential NullReferenceException.
            casting?.CastSpell();
        }
        if (Input.GetKeyDown(keys["ClearCharges"]))
        {
            casting?.ClearCharges();
        }

        if (Input.GetKeyDown(keys["SelectFire"]))
            casting?.OnSelectCharge(ChargeTypes.fire);
        if (Input.GetKeyDown(keys["SelectWater"]))
            casting?.OnSelectCharge(ChargeTypes.water);
        if (Input.GetKeyDown(keys["SelectEarth"]))
            casting?.OnSelectCharge(ChargeTypes.earth);
        if (Input.GetKeyDown(keys["SelectAir"]))
            casting?.OnSelectCharge(ChargeTypes.wind);
        
    }

    // Returns the aim direction based on the player's position and mouse position.
    public float GetAimDirection()
    {
        Vector2 playerPosition = playerMovement.transform.position;
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = mousePosition - playerPosition;
        return Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    }
}

//switch (currentState)
// {
//     case GameState.MainMenu:
//         // Handle MainMenu interactions
//         break;
//     case GameState.SettingsMenu:
//         // Handle SettingsMenu interactions
//         break;
//     case GameState.InGame:
//         HandleMovementInput();
//         HandleSpellCastingInput();
//         break;
// }
