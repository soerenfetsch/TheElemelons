using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameProgressionStage CurrentProgressionStage { get; private set; } = GameProgressionStage.INVALID;
    public GameProgressionStage PreviousProgressionStage { get; private set; } = GameProgressionStage.INVALID;
    public GameStatus CurrentGameStatus { get; private set; } = GameStatus.MAIN_MENU;
    public GameStatus PreviousGameStatus { get; private set; }

    public GameSaveProvider SaveProvider;
    public UIManager uiManager; // Assuming this exists

    // Events for Game Status changes
    public static event Action<GameStatus> OnGameStatusChanged;

    private void Awake()
    {
        Debug.Log($"[GameManager] Awake on {gameObject.name}");
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[GameManager] Duplicate GameManager instance. Destroying this one.");
            Destroy(this.gameObject); // Destroy the GameObject this script is attached to
        }
        else
        {
            Instance = this;
            Debug.Log("[GameManager] GameManager Instance assigned.");
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("[GameManager] Start called.");
        if (SaveProvider == null)
        {
            SaveProvider = new GameSaveProvider(); // Default
            Debug.LogWarning("[GameManager] SaveProvider is null, creating default GameSaveProvider.");
        }
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
        ChangeGameStatus(GameStatus.MAIN_MENU);
    }

    public void ChangeProgressionStage(GameProgressionStage newStage)
    {
        Debug.Log($"Changing progression stage from {CurrentProgressionStage} to {newStage}");
        PreviousProgressionStage = CurrentProgressionStage; // Store the previous stage
        CurrentProgressionStage = newStage;
        if (newStage != GameProgressionStage.EARTH_EARLY && newStage != GameProgressionStage.EARTH_LATE
            && newStage != GameProgressionStage.EARTH_EARLY_BOSS && newStage != GameProgressionStage.EARTH_LATE_BOSS)
        {
            // Save the game if the stage is not EARTH_EARLY or EARTH_LATE
            SaveGame();
        }
        StartMainGameStage();
    }

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

    private void ResetProgressionStage()
    {
        CurrentProgressionStage = GameProgressionStage.INVALID;
        PreviousProgressionStage = GameProgressionStage.INVALID;
    }

    public void NewGame()
    {
        Debug.Log("[GameManager] NewGame called.");
        if (SaveProvider == null) { Debug.LogError("[GameManager] SaveProvider is NULL in NewGame!"); return; }
        SaveProvider.ResetSave();
        // Reset the progression stage
        ChangeProgressionStage(GameProgressionStage.CASTLE_INTRODUCTION);
        // CurrentProgressionStage = GameProgressionStage.CASTLE_INTRODUCTION;
        // CurrentGameStatus = GameStatus.INGAME; // Let ChangeGameStatus handle this if needed via StartMainGameStage
        // ChangeGameStatus(GameStatus.INGAME); // Explicitly set if StartMainGameStage doesn't
    }

    public void ContinueSavedGame()
    {
        GameSaveData data = SaveProvider.LoadGame();
        if (data != null)
        {
            CurrentProgressionStage = data.progressionStage;
            var health = FindObjectOfType<Health>();
            var mana = FindObjectOfType<Mana>();
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

    public void SaveGame()
    {
        var health = FindObjectOfType<Health>();
        var mana = FindObjectOfType<Mana>();
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

    private void StartMainGameStage()
    {
        MainGame.Instance?.StartGameStage(CurrentProgressionStage);
    }

    [ContextMenu("Change to Boss Stage")]
    private void ChangeToBossStage()
    {
        ChangeProgressionStage(GameProgressionStage.EARTH_EARLY_BOSS);
    }

}
