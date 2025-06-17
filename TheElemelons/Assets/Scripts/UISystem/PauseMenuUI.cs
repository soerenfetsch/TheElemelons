using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the pause menu UI, including button initialization and event handling.
/// Provides methods for continuing the game, saving, opening options, and returning to the main menu.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the Continue Game button.
    /// </summary>
    private Button continueButton;

    /// <summary>
    /// Reference to the Save Game button.
    /// </summary>
    private Button saveGameButton;

    /// <summary>
    /// Reference to the Options button.
    /// </summary>
    private Button optionsButton;

    /// <summary>
    /// Reference to the Main Menu button.
    /// </summary>
    private Button mainMenuButton;

    /// <summary>
    /// Unity Start method. Initializes button references and listeners.
    /// </summary>
    private void Start()
    {
        // Initialize button components
        InitializeButtons();
    }

    /// <summary>
    /// Finds and assigns button components, and attaches their listeners.
    /// </summary>
    public void InitializeButtons()
    {
        // Get button components
        continueButton = transform.Find("ContinueGameButton").GetComponent<Button>();
        saveGameButton = transform.Find("SaveGameButton").GetComponent<Button>();
        optionsButton = transform.Find("OptionsButton").GetComponent<Button>();
        mainMenuButton = transform.Find("MainMenuButton").GetComponent<Button>();

        // Attach button listeners
        continueButton.onClick.AddListener(OnContinueClicked);
        saveGameButton.onClick.AddListener(OnSaveGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    /// <summary>
    /// Unity OnDestroy method. Removes button listeners to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        // Remove listeners to avoid memory leaks
        continueButton.onClick.RemoveListener(OnContinueClicked);
        saveGameButton.onClick.RemoveListener(OnSaveGameClicked);
        optionsButton.onClick.RemoveListener(OnOptionsClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }

    /// <summary>
    /// Handles the Continue button click. Resumes the game and updates the game state.
    /// </summary>
    public void OnContinueClicked()
    {
        Debug.Log("Continue Game Clicked");
        // Resume the game and update the game status
        GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
    }

    /// <summary>
    /// Handles the Save Game button click. Saves the current game state.
    /// </summary>
    public void OnSaveGameClicked()
    {
        Debug.Log("Save Game Clicked");
        // Save the game state
        GameManager.Instance.SaveGame();
    }

    /// <summary>
    /// Handles the Options button click. Opens the options menu and updates the game state.
    /// </summary>
    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        // Open the options menu and update the game status
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

    /// <summary>
    /// Handles the Main Menu button click. Returns to the main menu and saves the game.
    /// </summary>
    public void OnMainMenuClicked()
    {
        Debug.Log("Main Menu Clicked");
        // Return to the main menu and save the game
        GameManager.Instance.GoMainMenu();
    }
}
