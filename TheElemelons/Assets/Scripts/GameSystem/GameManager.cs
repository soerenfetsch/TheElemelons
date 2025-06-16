using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Central manager for game state, progression, and save/load operations.
/// Handles transitions between progression stages, game statuses, and manages saving/loading player data.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// The current progression stage of the game.
    /// </summary>
    public GameProgressionStage CurrentProgressionStage { get; private set; } = GameProgressionStage.INVALID;

    /// <summary>
    /// The previous progression stage before the last change.
    /// </summary>
    public GameProgressionStage PreviousProgressionStage { get; private set; } = GameProgressionStage.INVALID;

    /// <summary>
    /// The current status of the game (e.g., in-game, menu, paused).
    /// </summary>
    public GameStatus CurrentGameStatus { get; private set; } = GameStatus.MAIN_MENU;

    /// <summary>
    /// The previous game status before the last change.
    /// </summary>
    public GameStatus PreviousGameStatus { get; private set; }

    /// <summary>
    /// Provider for saving and loading game data.
    /// </summary>
    public GameSaveProvider SaveProvider;

    /// <summary>
    /// Reference to the UIManager for UI updates.
    /// </summary>
    public UIManager uiManager;

    /// <summary>
    /// Event triggered when the game status changes.
    /// </summary>
    public static event Action<GameStatus> OnGameStatusChanged;

    // Reference to the player GameObject, for saving/loading player state.
    private GameObject player;

    // Unity Awake callback. Initializes the singleton instance and finds the player GameObject.
    private void Awake()
    {
        Debug.Log($"[GameManager] Awake on {gameObject.name}");
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[GameManager] Duplicate GameManager instance. Destroying this one.");
            Destroy(gameObject); // Destroy the GameObject this script is attached to
        }
        else
        {
            Instance = this;
            Debug.Log("[GameManager] GameManager Instance assigned.");
            DontDestroyOnLoad(gameObject);
        }

        player = GameObject.FindWithTag("Player");
    }

    // Unity Start callback. Initializes the save provider, registers player death event, and sets up UI.
    private void Start()
    {
        Debug.Log("[GameManager] Start called.");
        if (SaveProvider == null)
        {
            SaveProvider = new GameSaveProvider(); // Default
            Debug.LogWarning("[GameManager] SaveProvider is null, creating default GameSaveProvider.");
        }

        // Add event when the player is defeated
        CombatManager.OnPlayerDeath += () =>
        {
            HandlePlayerDeath();
        };
        ChangeProgressionStage(GameProgressionStage.INVALID, true);

        // Check if a save file exists then enable the continue button
        if (SaveProvider.SaveExists())
        {
            // Enable the continue button in the UI
            UIManager.Instance?.ToggleContinueGameButton(true);
        }
        else
        {
            // Disable the continue button in the UI
            UIManager.Instance?.ToggleContinueGameButton(false);
        }
    }

    /// <summary>
    /// Changes the current progression stage and starts the corresponding game stage.
    /// </summary>
    /// <param name="newStage">The new progression stage to switch to.</param>
    /// <param name="isStartScene">Whether this is the start scene (optional).</param>
    public void ChangeProgressionStage(GameProgressionStage newStage, bool isStartScene = false)
    {
        Debug.Log($"Changing progression stage from {CurrentProgressionStage} to {newStage}");
        // Save the current game state if transitioning from an ingame stage
        if (CurrentProgressionStage != GameProgressionStage.INVALID)
            SaveGame();
        PreviousProgressionStage = CurrentProgressionStage; // Store the previous stage
        CurrentProgressionStage = newStage;
        StartMainGameStage(isStartScene);
    }

    /// <summary>
    /// Changes the current game status (e.g., in-game, menu, paused) and triggers related events and time scaling.
    /// </summary>
    /// <param name="newStatus">The new game status to switch to.</param>
    public void ChangeGameStatus(GameStatus newStatus)
    {
        Debug.Log($"Changing game status from {CurrentGameStatus} to {newStatus}");
        PreviousGameStatus = CurrentGameStatus; // Store the previous status
        CurrentGameStatus = newStatus;
        OnGameStatusChanged?.Invoke(newStatus); // Invoke the event

        switch (newStatus)
        {
            case GameStatus.DEATH_SCREEN:
                // Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.LOADING_SCREEN:
                Time.timeScale = 0; // Pause the game
                Debug.Log("[GameManager] Status: LOADING_SCREEN.");
                break;
            case GameStatus.MAIN_MENU:
                ResetProgressionStage();
                Time.timeScale = 1; // no pause game for background image movement
                break;
            case GameStatus.PAUSE_MENU:
                Time.timeScale = 0; // Pause the game
                Debug.Log("[GameManager] Status: PAUSE_MENU. Time.timeScale = 0");
                break;
            case GameStatus.DIALOGUE:
                Time.timeScale = 0; // Pause the game
                Debug.Log("[GameManager] Status: DIALOGUE. Time.timeScale = 0");
                break;
            case GameStatus.SETTINGS_MENU:
                Time.timeScale = 0; // Pause the game
                Debug.Log("[GameManager] Status: SETTINGS_MENU");
                break;
            case GameStatus.INGAME:
                Time.timeScale = 1; // Resume the game
                Debug.Log("[GameManager] Status: INGAME. Time.timeScale = 1");
                break;
            default:
                Debug.LogWarning($"[GameManager] Unhandled GameStatus: {newStatus}");
                break;
        }
    }

    // Resets the progression stage to INVALID.
    private void ResetProgressionStage()
    {
        CurrentProgressionStage = GameProgressionStage.INVALID;
        PreviousProgressionStage = GameProgressionStage.INVALID;
    }

    /// <summary>
    /// Starts a new game, resetting save data and setting the progression stage to the introduction.
    /// </summary>
    public void NewGame()
    {
        Debug.Log("[GameManager] NewGame called.");
        if (SaveProvider == null) { Debug.LogError("[GameManager] SaveProvider is NULL in NewGame!"); return; }
        SaveProvider.ResetSave();
        // Reset the progression stage
        ChangeProgressionStage(GameProgressionStage.CASTLE_INTRODUCTION);
    }

    /// <summary>
    /// Continues the game from saved data, restoring progression stage, player stats, and key bindings.
    /// </summary>
    public void ContinueSavedGame()
    {
        GameSaveData data = SaveProvider.LoadGame();
        if (data != null)
        {
            CurrentProgressionStage = data.progressionStage;
            var health = player.GetComponent<Health>();
            var mana = player.GetComponent<Mana>();
            if (health != null) health.SetMaxHealth(data.maxHealth);
            if (mana != null) mana.SetMaxMana(data.maxMana);
            if (InputManager.Instance != null && data.keyBindings != null)
                InputManager.Instance.ImportKeyBindings(data.keyBindings);
            ChangeProgressionStage(data.progressionStage);
        }
        else
        {
            NewGame(); // If no save data, start a new game
        }
    }

    /// <summary>
    /// Returns to the main menu and resets the progression stage.
    /// </summary>
    public void GoMainMenu()
    {
        ChangeProgressionStage(GameProgressionStage.INVALID, true);
    }

    /// <summary>
    /// Saves the current game state, including progression, player stats, and key bindings.
    /// </summary>
    public void SaveGame()
    {
        if (CurrentProgressionStage == GameProgressionStage.INVALID)
            return; // Do not save if still in the main menu

        var health = player.GetComponent<Health>();
        var mana = player.GetComponent<Mana>();
        var inputManager = InputManager.Instance;

        GameSaveData data = new GameSaveData
        {
            progressionStage = CurrentProgressionStage,
            // Save player stats, button settings, etc.
            maxHealth = health != null ? (int)health.GetMaxHealth() : 3,
            maxMana = mana != null ? (int)mana.GetMaxMana() : 3,
            saveDate = DateTime.Now,
            keyBindings = inputManager != null ? inputManager.ExportKeyBindings() : new List<GameSaveData.KeyBindingEntry>()
        };
        SaveProvider.SaveGame(data);
        if (File.Exists(SaveProvider.SavePath))
        {
            Debug.Log("Game saved successfully.");
            UIManager.Instance?.ToggleContinueGameButton(true);
        }
        else
        {
            Debug.LogError("Failed to save game.");
        }
    }

    // Starts the main game stage for the current progression stage.
    private void StartMainGameStage(bool isStartScene = false)
    {
        MainGame.Instance?.StartGameStage(CurrentProgressionStage, isStartScene);
    }

    /// <summary>
    /// Changes the progression stage to the early earth boss stage (for debugging).
    /// </summary>
    [ContextMenu("Change to Boss Stage")]
    private void ChangeToBossStage()
    {
        ChangeProgressionStage(GameProgressionStage.EARTH_EARLY_BOSS);
    }

    // Handles player death: saves the game, changes status, and hides the boss bar.
    private void HandlePlayerDeath()
    {
        SaveGame(); // Save the game and the player stats so that
        ChangeGameStatus(GameStatus.DEATH_SCREEN);
        player.GetComponent<PlayerMovement>().SetMovement(Vector2.zero);
        UIManager.Instance.ToggleBossBar(false, null);
    }
}