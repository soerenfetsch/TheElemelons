using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the start menu UI, including button initialization and event handling.
/// Provides methods for starting a new game, continuing a saved game, opening options, and quitting the application.
/// </summary>
public class StartMenuUI : MonoBehaviour
{
    /// <summary>
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
    /// </summary>
    private Button quitGameButton;

    /// <summary>
    /// Unity Awake method. Initializes button components and disables the continue button by default.
    /// </summary>
    private void Awake()
    {
        // Initialize button components
        InitializeButtons();
        // Initially disable the continue button
        DisableContinueGame();
    }

    /// <summary>
    /// Finds and assigns button components, and attaches their listeners.
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
    /// Unity OnDestroy method. Removes button listeners to prevent memory leaks.
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
    /// Handles the New Game button click. Starts a new game via the GameManager.
    /// </summary>
    public void OnNewGameClicked()
    {
        Debug.Log("New Game Clicked");
        GameManager.Instance.NewGame();
    }

    /// <summary>
    /// Handles the Continue Game button click. Continues a saved game via the GameManager.
    /// </summary>
    public void OnContinueGameClicked()
    {
        Debug.Log("Continue Game Clicked");
        GameManager.Instance.ContinueSavedGame();
    }

    /// <summary>
    /// Handles the Options button click. Opens the settings menu via the GameManager.
    /// </summary>
    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

    /// <summary>
    /// Handles the Quit Game button click. Quits the application or stops play mode in the editor.
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
