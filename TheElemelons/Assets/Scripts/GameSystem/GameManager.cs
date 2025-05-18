using UnityEngine;
using UnityEngine.SceneManagement;
using System; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameProgressionStage CurrentProgressionStage { get; private set; }
    public GameStatus CurrentGameStatus { get; private set; }

    public GameSaveProvider SaveProvider; // Assuming this is IGameSaveProvider
    public UIManager uiManager; // Assuming this exists

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
            Debug.LogWarning("[GameManager] SaveProvider is null, creating default GameSaveProvider.");
            SaveProvider = new GameSaveProvider();
        }
        // Initialize with a default status if not set elsewhere, e.g., from main menu
        // if(CurrentGameStatus == default(GameStatus)) // Check if it's the default enum value
        // {
        //     ChangeGameStatus(GameStatus.INGAME); // Or MAIN_MENU
        // }
    }

    public void ChangeProgressionStage(GameProgressionStage newStage)
    {
        Debug.Log($"[GameManager] ChangeProgressionStage called. Old: {CurrentProgressionStage}, New: {newStage}");
        CurrentProgressionStage = newStage;
        if (newStage != GameProgressionStage.EARTH_EARLY && newStage != GameProgressionStage.EARTH_LATE)
        {
            SaveGame();
        }
        StartMainGameStage();
    }

    public void ChangeGameStatus(GameStatus newStatus)
    {
        Debug.Log($"[GameManager] ChangeGameStatus called. Old: {CurrentGameStatus}, New: {newStatus}");
        CurrentGameStatus = newStatus;
        OnGameStatusChanged?.Invoke(newStatus);
        Debug.Log($"[GameManager] OnGameStatusChanged event invoked with {newStatus}.");

        switch (newStatus)
        {
            case GameStatus.DEATH_SCREEN:
                Debug.Log("[GameManager] Status: DEATH_SCREEN");
                break;
            case GameStatus.MAIN_MENU:
                Debug.Log("[GameManager] Status: MAIN_MENU. Saving game and loading MainMenu scene.");
                SaveGame();
                SceneManager.LoadScene("MainMenu");
                break;
            case GameStatus.PAUSE_MENU:
                Debug.Log("[GameManager] Status: PAUSE_MENU. Time.timeScale = 0");
                Time.timeScale = 0;
                break;
            case GameStatus.DIALOGUE:
                Debug.Log("[GameManager] Status: DIALOGUE. Time.timeScale = 0");
                Time.timeScale = 0;
                break;
            case GameStatus.SETTINGS_MENU:
                Debug.Log("[GameManager] Status: SETTINGS_MENU");
                break;
            case GameStatus.INGAME:
                Debug.Log("[GameManager] Status: INGAME. Time.timeScale = 1");
                Time.timeScale = 1;
                break;
            default:
                Debug.LogWarning($"[GameManager] Unhandled GameStatus: {newStatus}");
                break;
        }
    }

    public void NewGame()
    {
        Debug.Log("[GameManager] NewGame called.");
        if(SaveProvider == null) { Debug.LogError("[GameManager] SaveProvider is NULL in NewGame!"); return; }
        SaveProvider.ResetSave();
        CurrentProgressionStage = GameProgressionStage.CASTLE_INTRODUCTION;
        // CurrentGameStatus = GameStatus.INGAME; // Let ChangeGameStatus handle this if needed via StartMainGameStage
        StartMainGameStage(); // This should probably set the status to INGAME
        ChangeGameStatus(GameStatus.INGAME); // Explicitly set if StartMainGameStage doesn't
    }

    public void ContinueSavedGame()
    {
        Debug.Log("[GameManager] ContinueSavedGame called.");
        if(SaveProvider == null) { Debug.LogError("[GameManager] SaveProvider is NULL in ContinueSavedGame!"); return; }
        GameSaveData data = SaveProvider.LoadGame();
        if (data != null)
        {
            Debug.Log($"[GameManager] Loaded save data. Progression: {data.progressionStage}");
            CurrentProgressionStage = data.progressionStage;
            // Load player stats, button settings, etc.
            InputManager.Instance.ImportKeyBindings(data.keyBindings); // ADD THIS
        }
        else
        {
            Debug.LogWarning("[GameManager] No save data found or failed to load. Starting NewGame instead.");
            NewGame();
            return; // NewGame will handle the rest
        }
        // CurrentGameStatus = GameStatus.INGAME;
        StartMainGameStage();
        ChangeGameStatus(GameStatus.INGAME); // Explicitly set
    }

    public void SaveGame()
    {
        Debug.Log("[GameManager] SaveGame called.");
        if(SaveProvider == null) { Debug.LogError("[GameManager] SaveProvider is NULL in SaveGame!"); return; }
        GameSaveData data = new GameSaveData
        {
            progressionStage = CurrentProgressionStage,
        };
        SaveProvider.SaveGame(data);
    }

    private void StartMainGameStage()
    {
        Debug.Log("[GameManager] StartMainGameStage called. CurrentProgressionStage: " + CurrentProgressionStage);
        // Potentially set GameStatus to INGAME here if this function implies gameplay starts/resumes
        // Example: SceneManager.LoadScene(GetSceneNameForStage(CurrentProgressionStage));
    }
}