using UnityEngine;
using GameSystem; // Updated namespace

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnGameStateChanged;

    [Header("References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private Player player;
    [SerializeField] private SaveManager saveManager;

    private GameState _currentGameState;
    private SaveGame _currentGameSave;
    public GameState CurrentGameState
    {
        get => _currentGameState;
        private set
        {
            if (_currentGameState != value)
            {
                _currentGameState = value;
                OnGameStateChanged?.Invoke(_currentGameState);
            }
        }
    }

    public bool IsGamePaused => CurrentGameState == GameState.PauseMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Duplicate GameManager detected! Destroying extra instance.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _currentGameSave = saveManager.LoadGame() ?? new SaveGame();
        SetGameState(GameState.MainMenu);
    }

    public void SetGameState(GameState newState)
    {
        if (newState == GameState.MainMenu || newState == GameState.InGame)
        {
            _currentGameSave = saveManager.LoadGame() ?? new SaveGame();
            saveManager.SaveGame(_currentGameSave); // ✅ Auto-save on state change
        }

        CurrentGameState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                uiManager.ShowMainMenu();
                Time.timeScale = 1f;
                break;
            case GameState.InGame:
                uiManager.HideMenus();
                Time.timeScale = 1f;
                break;
            case GameState.PauseMenu:
                uiManager.ShowPauseMenu();
                Time.timeScale = 0f;
                break;
            case GameState.SettingsMenu:
                uiManager.ShowSettingsMenu();
                break;
            case GameState.InDialogue: // ✅ Handle dialogue state
                uiManager.ShowInDialogueUI();
                Time.timeScale = 0f; // Pause game while in dialogue
                break;
        }
    }

    public void TogglePause()
    {
        SetGameState(IsGamePaused ? GameState.InGame : GameState.PauseMenu);
    }
}
