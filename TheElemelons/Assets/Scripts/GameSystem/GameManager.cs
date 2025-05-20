using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameProgressionStage CurrentProgressionStage { get; private set; }
    public GameStatus CurrentGameStatus { get; private set; }

    public GameSaveProvider SaveProvider;
    public UIManager uiManager;

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
        CurrentGameStatus = newStatus;
        OnGameStatusChanged?.Invoke(newStatus); // Invoke the event

        switch (newStatus)
        {
            case GameStatus.DEATH_SCREEN:
                uiManager.EnableDeathScreen();
                break;
            case GameStatus.MAIN_MENU:
                SaveGame();
                SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene
                break;
            case GameStatus.PAUSE_MENU:
                //uiManager.OpenPauseMenu(); // UIManager will handle this via the event
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.DIALOGUE:
                Time.timeScale = 0; // Pause the game
                break;
            case GameStatus.SETTINGS_MENU:
                //uiManager.OpenSettingsMenu(); // UIManager will handle this via the event
                break;
            case GameStatus.INGAME:
                Time.timeScale = 1; // Resume the game
                break;
        }
    }

    public void NewGame()
    {
        SaveProvider.ResetSave();
        CurrentProgressionStage = GameProgressionStage.CASTLE_INTRODUCTION;
        CurrentGameStatus = GameStatus.INGAME;
        StartMainGameStage();
    }

    public void ContinueSavedGame()
    {
        GameSaveData data = SaveProvider.LoadGame();
        if (data != null)
        {
            CurrentProgressionStage = data.progressionStage;
            // Load player stats, button settings, etc.
        }
        else
        {
            NewGame(); // If no save data, start a new game
        }
        CurrentGameStatus = GameStatus.INGAME;
        StartMainGameStage();
    }

    private void SaveGame()
    {
        GameSaveData data = new GameSaveData
        {
            progressionStage = CurrentProgressionStage,
            // Save player stats, button settings, etc.
        };
        SaveProvider.SaveGame(data);
    }

    private void StartMainGameStage()
    {
        // Implement logic to load different scenes or enable/disable game objects
        // based on the CurrentProgressionStage.
        Debug.Log("Starting game stage: " + CurrentProgressionStage);
    }
}
