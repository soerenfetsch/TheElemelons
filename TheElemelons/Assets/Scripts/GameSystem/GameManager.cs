using UnityEngine;
using System;

/// <summary>
/// Enumeration of possible game states.
/// </summary>
public enum GameState
{
    MainMenu,
    InGame,
    Paused,
    SettingsMenu,
    Dialogue
}

/// <summary>
/// Manages the overall game state, progression, and transitions.
/// This class is implemented as a Singleton and persists across scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton

    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }

    #endregion

    #region Public Properties and Events

    /// <summary>
    /// Gets the current game state.
    /// </summary>
    public GameState CurrentState { get; private set; }

    /// <summary>
    /// Event for broadcasting state changes to interested subscribers.
    /// </summary>
    public event Action<GameState> OnGameStateChanged;

    /// <summary>
    /// Reference to the GameProgression data asset (assigned in the Inspector).
    /// </summary>
    public GameProgression CurrentProgress;

    /// <summary>
    /// Flag indicating whether the player is currently engaged in combat.
    /// This can be used to prevent dialogue from starting mid-combat.
    /// </summary>
    public bool isInCombat = false;
    // TODO: Better combat detection logic can be implemented here.

    #endregion

    #region Unity Callbacks

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the Singleton instance and ensures persistence.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// Sets the initial game state.
    /// </summary>
    private void Start()
    {
        // For testing purposes, start the game in the InGame state.
        // Uncomment the next line if you want to start in the Main Menu instead.
        // ChangeGameState(GameState.MainMenu);
        ChangeGameState(GameState.InGame);
    }

    /// <summary>
    /// OnEnable is called when the object becomes enabled and active.
    /// Subscribes to the DialogueManager's dialogue completion event.
    /// </summary>
    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueComplete += OnDialogueCompleteHandler;
        }
    }

    /// <summary>
    /// OnDisable is called when the behaviour becomes disabled.
    /// Unsubscribes from the DialogueManager's dialogue completion event.
    /// </summary>
    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueComplete -= OnDialogueCompleteHandler;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Changes the current game state and notifies subscribers.
    /// </summary>
    /// <param name="newState">The new state to transition into.</param>
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Game State Changed: " + newState);
        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Saves the game and transitions to the Main Menu state.
    /// </summary>
    public void GoToMainMenu()
    {
        GameSaveProvider.Instance.SaveGame();
        ChangeGameState(GameState.MainMenu);
    }

    /// <summary>
    /// Saves the game and quits the application.
    /// </summary>
    public void QuitGame()
    {
        GameSaveProvider.Instance.SaveGame();
        Application.Quit();
    }

    /// <summary>
    /// Pauses the game by changing the state to Paused.
    /// </summary>
    public void PauseGame()
    {
        ChangeGameState(GameState.Paused);
    }

    /// <summary>
    /// Resumes the game by changing the state to InGame.
    /// </summary>
    public void ResumeGame()
    {
        ChangeGameState(GameState.InGame);
    }

    /// <summary>
    /// Starts a new game by resetting saved progress and switching to InGame state.
    /// </summary>
    public void StartNewGame()
    {
        GameSaveProvider.Instance.ResetSave();
        ChangeGameState(GameState.InGame);
    }

    /// <summary>
    /// Continues a game by loading saved progress and switching to InGame state.
    /// </summary>
    public void ContinueGame()
    {
        GameSaveProvider.Instance.LoadGame();
        ChangeGameState(GameState.InGame);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handler for when dialogue is completed.
    /// Resumes gameplay by setting the state to InGame.
    /// </summary>
    private void OnDialogueCompleteHandler()
    {
        ResumeGame();
    }

    #endregion
}