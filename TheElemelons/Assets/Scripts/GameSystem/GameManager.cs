using UnityEngine;

public enum GameState
{
    MainMenu,
    InGame,
    PauseMenu,
    SettingsMenu
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private Player player;
    [SerializeField] private GameSaveProvider gameSaveProvider;

    [Header("Game State")]
    public GameState CurrentGameState;
    public GameSaveData gameSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load the saved game state if it exists
        gameSaveData = gameSaveProvider.LoadGame();
        if (gameSaveData == null)
        {
            Debug.Log("No saved game found. Starting a new game.");
            NewGame();
        }
        else
        {
            ContinueGame();
        }
    }

    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                uiManager.ShowMainMenu();
                break;
            case GameState.InGame:
                uiManager.HideMenus();
                Time.timeScale = 1f; // Resume the game
                break;
            case GameState.PauseMenu:
                uiManager.ShowPauseMenu();
                Time.timeScale = 0f; // Pause the game
                break;
            case GameState.SettingsMenu:
                uiManager.ShowSettingsMenu();
                break;
        }
    }

    public void NewGame()
    {
        // Reset progression
        gameSaveProvider.ResetSave();
        gameSaveData = new GameSaveData
        {
            currentLevel = 1,
            completedDungeons = new bool[10], // Example size
            completedBossFights = new bool[10],
            completedDialogues = new bool[10],
            maxHealth = player.GetMaxHealth(),
            maxMana = player.GetMaxMana()
        };

        ChangeGameState(GameState.InGame);
        LoadLevel(1);
    }

    public void ContinueGame()
    {
        if (gameSaveData == null)
        {
            Debug.LogError("No saved game data to continue!");
            return;
        }

        ChangeGameState(GameState.InGame);
        LoadLevel(gameSaveData.currentLevel);
        player.InitializePlayer(gameSaveData.maxHealth, gameSaveData.maxMana);
    }

    public void PauseGame()
    {
        ChangeGameState(GameState.PauseMenu);
    }

    public void ResumeGame()
    {
        ChangeGameState(GameState.InGame);
    }

    public void GoToMainMenu()
    {
        SaveGameProgress();
        ChangeGameState(GameState.MainMenu);
    }

    public void QuitGame()
    {
        SaveGameProgress();
        Application.Quit();
    }

    public void SaveGameProgress()
    {
        gameSaveData.maxHealth = player.GetMaxHealth();
        gameSaveData.maxMana = player.GetMaxMana();
        gameSaveProvider.SaveGame(gameSaveData);
    }

    public void LoadLevel(int level)
    {
        gameSaveData.currentLevel = level;
        dungeonGenerator.GenerateDungeon();
        player.ResetPlayerState();
    }
}
