using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

/// <summary>
/// Central manager controlling game state transitions, save/load functionality, and initialization.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Definitions & Declarations

    /// <summary>
    /// Singleton instance of GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Current active game state.
    /// </summary>
    public IGameState CurrentState { get; private set; }

    public event Action<IGameState> OnStateChanged;
    public event Action<IGameState> OnPreStateExit;
    public event Action<IGameState> OnPostStateEnter;

    public IGameSaveProvider SaveProvider { get; private set; }

    private UIManager uiManager;
    private InputManager inputManager;

    #endregion

    #region Initialization

    /// <summary>
    /// Unity Awake method, sets up singleton instance, subsystems, and initial state.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSubsystems();
            ChangeState(new MainMenuState());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes key subsystems required by GameManager.
    /// </summary>
    private void InitializeSubsystems()
    {
        SaveProvider = FindObjectOfType<GameSaveProvider>() as IGameSaveProvider
                       ?? throw new Exception("GameSaveProvider not found.");

        uiManager = FindObjectOfType<UIManager>()
                    ?? throw new Exception("UIManager not found.");

        inputManager = FindObjectOfType<InputManager>()
                       ?? throw new Exception("InputManager not found.");
    }

    #endregion

    #region Core State Logic

    /// <summary>
    /// Unity Update method, delegates update to current state.
    /// </summary>
    private void Update()
    {
        CurrentState?.UpdateState();
    }

    /// <summary>
    /// Changes game state with proper entry and exit handling.
    /// </summary>
    /// <param name="newState">New state to transition to.</param>
    public void ChangeState(IGameState newState)
    {
        if (CurrentState?.GetType() == newState.GetType())
            return;

        OnPreStateExit?.Invoke(CurrentState);
        CurrentState?.Exit();

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);

        CurrentState.Enter();
        OnPostStateEnter?.Invoke(newState);
    }

    #endregion

    #region Public API

    /// <summary>
    /// Returns to main menu, saving current progress.
    /// </summary>
    public void GoToMainMenu()
    {
        SaveGame();
        ChangeState(new MainMenuState());
    }

    /// <summary>
    /// Continues current gameplay session.
    /// </summary>
    public void ContinueGame()
    {
        ChangeState(new InGameState());
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        ChangeState(new PauseState());
    }

    /// <summary>
    /// Loads game asynchronously from disk, then enters gameplay state.
    /// </summary>
    // public async void ContinueFromSaveAsync()
    // {
    //     uiManager.ShowLoadingScreen(true);
    //     try
    //     {
    //         await ContinueFromSaveAsyncInternal();
    //     }
    //     catch (Exception e)
    //     {
    //         Logger.LogError($"Error loading game: {e.Message}");
    //         ChangeState(new MainMenuState());
    //     }
    //     finally
    //     {
    //         uiManager.ShowLoadingScreen(false);
    //     }
    // }

    /// <summary>
    /// Starts a fresh game session.
    /// </summary>
    public void NewGame()
    {
        SaveProvider.ResetSave();
        ResetRuntimeData();

        DungeonGenerator dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        if (dungeonGenerator == null)
            throw new Exception("DungeonGenerator not found in scene.");

        dungeonGenerator.GenerateDungeon();
        ChangeState(new InGameState());
    }

    /// <summary>
    /// Opens settings menu.
    /// </summary>
    public void ClickOptions()
    {
        ChangeState(new SettingsState());
    }

    /// <summary>
    /// Saves current progress and quits the application.
    /// </summary>
    public void QuitGame()
    {
        SaveGame();
        Application.Quit();
    }

    /// <summary>
    /// Saves current game progress to disk.
    /// </summary>
    public void SaveGame()
    {
        var data = GatherCurrentGameData();
        SaveProvider.SaveGame(data);
    }

    #endregion

    #region Helpers & Private

    /// <summary>
    /// Collects runtime data into a saveable format.
    /// </summary>
    /// <returns>Filled GameSaveData.</returns>
    private GameSaveData GatherCurrentGameData()
    {
        var currentData = SaveProvider.CurrentSaveData
                          ?? throw new Exception("CurrentSaveData is null");

        // Get the current dungeon index from DungeonGenerator instead of MapManager
        DungeonGenerator dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        int dungeonIndex = dungeonGenerator != null ? dungeonGenerator.CurrentDungeonIndex : 0;
        
        Vector2 playerPos = Player.Instance.transform.position;
        
        currentData.UpdateFromRuntimeData(dungeonIndex, playerPos);
        return currentData;
    }

    private async Task ContinueFromSaveAsyncInternal()
    {
        var data = await SaveProvider.LoadGameAsync();
        if (data == null)
            throw new Exception("Loaded save data is null.");

        DungeonGenerator dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        if (dungeonGenerator == null)
            throw new Exception("DungeonGenerator not found in scene.");

        dungeonGenerator.GenerateDungeonFromData(data.MapData);

        Player player = FindObjectOfType<Player>();
        player.RestoreFromSave(data.PlayerData);

        BuffDebuffManager buffManager = FindObjectOfType<BuffDebuffManager>();
        buffManager.RestoreBuffs(data.BuffsData);

        ChangeState(new InGameState());
    }


    /// <summary>
    /// Resets runtime data for new game initialization.
    /// </summary>
    private void ResetRuntimeData()
    {
        Player.Instance.ResetPlayer();
        MapManager.Instance.ClearCurrentMap();
        BuffDebuffManager.Instance.ClearEffects(Player.Instance.gameObject);
    }

    #endregion
}
