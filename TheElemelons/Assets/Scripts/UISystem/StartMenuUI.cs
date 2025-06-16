using UnityEngine;
using UnityEngine.UI;

/// <summary>
<<<<<<< Updated upstream
/// Manages the UI elements and functionality of the main start menu.
=======
/// Controls the start menu UI, including button initialization and event handling.
/// Provides methods for starting a new game, continuing a saved game, opening options, and quitting the application.
>>>>>>> Stashed changes
/// </summary>
public class StartMenuUI : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
    /// Button to start a new game.
    /// </summary>
    private Button newGameButton;
    /// <summary>
    /// Button to continue a previously saved game.
    /// </summary>
    private Button continueGameButton;
    /// <summary>
    /// Button to open the options/settings menu.
    /// </summary>
    private Button optionsButton;
    /// <summary>
    /// Button to quit the game application.
=======
    /// Reference to the New Game button.
    /// </summary>
    private Button newGameButton;

    /// <summary>
    /// Reference to the Continue Game button.
    /// </summary>
    private Button continueGameButton;

    /// <summary>
    /// Reference to the Options button.
    /// </summary>
    private Button optionsButton;

    /// <summary>
    /// Reference to the Quit Game button.
>>>>>>> Stashed changes
    /// </summary>
    private Button quitGameButton;

    /// <summary>
<<<<<<< Updated upstream
    /// Called when the script instance is being loaded.
    /// Initializes button components and sets the initial state of the continue button.
=======
    /// Unity Awake method. Initializes button components and disables the continue button by default.
>>>>>>> Stashed changes
    /// </summary>
    private void Awake()
    {
        // Initialize button components
        InitializeButtons();
        // Initially disable the continue button
        DisableContinueGame();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Finds and initializes the UI button components and attaches their listeners.
    /// Assumes buttons are named "NewGameButton", "ContinueGameButton", "OptionsButton", and "QuitGameButton"
    /// as children of this GameObject.
=======
    /// Finds and assigns button components, and attaches their listeners.
>>>>>>> Stashed changes
    /// </summary>
    public void InitializeButtons()
    {
        // Get button components
        newGameButton = transform.Find("NewGameButton").GetComponent<Button>();
        continueGameButton = transform.Find("ContinueGameButton").GetComponent<Button>();
        optionsButton = transform.Find("OptionsButton").GetComponent<Button>();
        quitGameButton = transform.Find("QuitGameButton").GetComponent<Button>();

        // Attach button listeners
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueGameButton.onClick.AddListener(OnContinueGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        quitGameButton.onClick.AddListener(OnQuitGameClicked);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Called when the MonoBehaviour will be destroyed.
    /// Removes listeners from buttons to prevent memory leaks.
=======
    /// Unity OnDestroy method. Removes button listeners to prevent memory leaks.
>>>>>>> Stashed changes
    /// </summary>
    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        newGameButton.onClick.RemoveListener(OnNewGameClicked);
        continueGameButton.onClick.RemoveListener(OnContinueGameClicked);
        optionsButton.onClick.RemoveListener(OnOptionsClicked);
        quitGameButton.onClick.RemoveListener(OnQuitGameClicked);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the "New Game" button.
    /// Initiates a new game session via the GameManager.
=======
    /// Handles the New Game button click. Starts a new game via the GameManager.
>>>>>>> Stashed changes
    /// </summary>
    public void OnNewGameClicked()
    {
        Debug.Log("New Game Clicked");
        GameManager.Instance.NewGame();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the "Continue Game" button.
    /// Loads a saved game session via the GameManager.
=======
    /// Handles the Continue Game button click. Continues a saved game via the GameManager.
>>>>>>> Stashed changes
    /// </summary>
    public void OnContinueGameClicked()
    {
        Debug.Log("Continue Game Clicked");
        GameManager.Instance.ContinueSavedGame();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the "Options" button.
    /// Changes the game state to open the settings menu.
=======
    /// Handles the Options button click. Opens the settings menu via the GameManager.
>>>>>>> Stashed changes
    /// </summary>
    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the "Quit Game" button.
    /// Exits the application or stops play mode in the editor.
=======
    /// Handles the Quit Game button click. Quits the application or stops play mode in the editor.
>>>>>>> Stashed changes
    /// </summary>
    public void OnQuitGameClicked()
    {
        Debug.Log("Quit Game Clicked");
    #if UNITY_EDITOR
        // Stop play mode if in the editor
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // Quit the application if built
        Application.Quit();
    #endif
    }

    /// <summary>
    /// Enables the Continue Game button, making it interactable.
    /// This is typically called when a saved game is detected.
    /// </summary>
    public void EnableContinueGame()
    {
        continueGameButton.interactable = true;
    }

    /// <summary>
    /// Disables the Continue Game button, making it non-interactable.
    /// This is typically called if no save data is found.
    /// </summary>
    public void DisableContinueGame()
    {
        continueGameButton.interactable = false;
    }
}
