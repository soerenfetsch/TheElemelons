using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameProgressionStage CurrentProgressionStage { get; private set; }
    public GameStatus CurrentGameStatus { get; private set; } = GameStatus.MAIN_MENU;
    public GameStatus PreviousGameStatus { get; private set; }

    public GameSaveProvider SaveProvider;

    // Events for Game Status changes
    public static event Action<GameStatus> OnGameStatusChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (SaveProvider == null)
        {
            SaveProvider = new GameSaveProvider(); // Default
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
        CurrentProgressionStage = newStage;
        if (newStage != GameProgressionStage.EARTH_EARLY && newStage != GameProgressionStage.EARTH_LATE)
        {
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
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.LOADING_SCREEN:
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.MAIN_MENU:
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.PAUSE_MENU:
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.DIALOGUE:
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.SETTINGS_MENU:
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.INGAME:
                Time.timeScale = 1; // Resume the game
                break;
        }
    }

    public void NewGame()
    {
        SaveProvider.ResetSave();
        ChangeProgressionStage(GameProgressionStage.CASTLE_INTRODUCTION);
    }

    public void ContinueSavedGame()
    {
        GameSaveData data = SaveProvider.LoadGame();
        if (data != null)
        {
            ChangeProgressionStage(data.progressionStage);
        }
        else
        {
            NewGame(); // If no save data, start a new game
        }
    }

    private void SaveGame()
    {
        GameSaveData data = new GameSaveData
        {
            progressionStage = CurrentProgressionStage,
            // Save player stats, button settings, etc.
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
}
