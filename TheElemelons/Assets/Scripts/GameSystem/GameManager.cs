using System;
using UnityEngine;
using UnityEngine.SceneManagement; // For accessing the active scene name

/// <summary>
/// Controls the overall game flow via a modular state machine.
/// Manages transitions between states (classes implementing IGameState) 
/// and delegates persistence operations to the IGameSaveProvider.
/// Exposes a global instance (singleton) for other systems to access the current game state.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Currently active state object.
    /// </summary>
    public IGameState CurrentState { get; private set; }

    /// <summary>
    /// Event triggered when the game state changes.
    /// Subscribers can react based on the new state.
    /// </summary>
    public event Action<IGameState> OnStateChanged;

    /// <summary>
    /// Global transition hook fired before exiting the current state.
    /// </summary>
    public event Action<IGameState> OnPreStateExit;

    /// <summary>
    /// Global transition hook fired after entering the new state.
    /// </summary>
    public event Action<IGameState> OnPostStateEnter;

    /// <summary>
    /// Save provider for game persistence.
    /// </summary>
    public IGameSaveProvider SaveProvider { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Fallback: assign SaveProvider if not provided externally.
            if (SaveProvider == null)
            {
                SaveProvider = FindObjectOfType<GameSaveProvider>();
                if (SaveProvider == null)
                {
                    Logger.LogError("GameSaveProvider not found in scene.");
                }
            }

            // Initialize with Main Menu state.
            ChangeState(new MainMenuState());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Delegate per-frame state updates.
        CurrentState?.UpdateState();
    }

    /// <summary>
    /// Changes the current game state to the new state.
    /// Fires pre and post transition events for global transition effects.
    /// </summary>
    /// <param name="newState">The new state object to transition into.</param>
    public void ChangeState(IGameState newState)
    {
        // Avoid redundant transitions if the new state is of the same type.
        if (CurrentState?.GetType() == newState.GetType())
            return;

        // Fire pre-exit hook before leaving current state.
        OnPreStateExit?.Invoke(CurrentState);

        // Exit the current state.
        CurrentState?.Exit();
        
        // Update the current state.
        CurrentState = newState;
        Logger.Log($"GameManager changed state to: {newState.GetType().Name}");
        
        // Fire the state changed event.
        OnStateChanged?.Invoke(newState);

        // Enter the new state.
        CurrentState.Enter();

        // Fire post-enter hook after the new state is active.
        OnPostStateEnter?.Invoke(CurrentState);
    }

    /// <summary>
    /// Gathers current runtime data from various subsystems and updates the GameSaveData instance.
    /// Replace placeholders with actual subsystem calls as your game evolves.
    /// </summary>
    /// <returns>The updated GameSaveData representing current game state.</returns>
    private GameSaveData GatherCurrentGameData()
    {
        // Retrieve the current GameSaveData from the provider.
        GameSaveData currentData = SaveProvider?.CurrentSaveData;
        if (currentData == null)
        {
            Logger.LogError("Current save data instance is null.");
            return null;
        }

        // Example placeholder values:
        int currentDungeon = 1;             // Replace with: DungeonManager.Instance.CurrentDungeonIndex;
        int collectedElemelons = 10;          // Replace with: PlayerManager.Instance.CollectedElemelons;
        Vector2 playerPosition = Vector2.zero; // Replace with: PlayerController.Instance.CurrentPosition;
        string currentScene = SceneManager.GetActiveScene().name;

        // Update the save data with the gathered runtime values.
        currentData.UpdateFromRuntimeData(currentDungeon, collectedElemelons, playerPosition, currentScene);

        return currentData;
    }

    #region High-Level Game Flow Methods

    /// <summary>
    /// Saves the current game data and transitions to the Main Menu state.
    /// </summary>
    public void GoToMainMenu()
    {
        GameSaveData currentData = GatherCurrentGameData();
        SaveProvider?.SaveGame(currentData);
        ChangeState(new MainMenuState());
    }

    /// <summary>
    /// Saves the current game data and quits the application.
    /// </summary>
    public void QuitGame()
    {
        GameSaveData currentData = GatherCurrentGameData();
        SaveProvider?.SaveGame(currentData);
        Application.Quit();
    }

    /// <summary>
    /// Transitions to the Settings state.
    /// </summary>
    public void ClickOptions()
    {
        ChangeState(new SettingsState());
    }

    /// <summary>
    /// Pauses the game by transitioning to the Pause state.
    /// </summary>
    public void PauseGame()
    {
        ChangeState(new PauseState());
    }

    /// <summary>
    /// Resumes gameplay by transitioning to the In-Game state.
    /// </summary>
    public void ContinueGame()
    {
        ChangeState(new InGameState());
    }

    /// <summary>
    /// Resets the save data and starts a new game by transitioning to the In-Game state.
    /// </summary>
    public void NewGame()
    {
        SaveProvider?.ResetSave();
        ChangeState(new InGameState());
    }

    /// <summary>
    /// Loads saved game data and transitions to the In-Game state asynchronously.
    /// </summary>
    public async void ContinueFromSaveAsync()
    {
        GameSaveData data = await SaveProvider.LoadGameAsync();
        // TODO: Apply loaded data (update player position, restore map layout, etc.)
        ChangeState(new InGameState());
    }

    /// <summary>
    /// Triggers saving of the current game data.
    /// </summary>
    public void SaveGame()
    {
        GameSaveData currentData = GatherCurrentGameData();
        SaveProvider?.SaveGame(currentData);
    }

    #endregion
}
