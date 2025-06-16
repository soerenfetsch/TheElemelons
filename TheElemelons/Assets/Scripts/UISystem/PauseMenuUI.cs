using UnityEngine;
using UnityEngine.UI;

/// <summary>
<<<<<<< Updated upstream
/// Manages the UI elements and functionality of the pause menu.
=======
/// Controls the pause menu UI, including button initialization and event handling.
/// Provides methods for continuing the game, saving, opening options, and returning to the main menu.
>>>>>>> Stashed changes
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    /// <summary>
<<<<<<< Updated upstream
    /// Button to resume the game.
    /// </summary>
    private Button continueButton;
    /// <summary>
    /// Button to save the current game state.
    /// </summary>
    private Button saveGameButton;
    /// <summary>
    /// Button to open the options/settings menu.
    /// </summary>
    private Button optionsButton;
    /// <summary>
    /// Button to return to the main menu.
=======
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
>>>>>>> Stashed changes
    /// </summary>
    private Button mainMenuButton;

    /// <summary>
<<<<<<< Updated upstream
    /// Called when the script instance is being loaded.
    /// Initializes the button components.
=======
    /// Unity Start method. Initializes button references and listeners.
>>>>>>> Stashed changes
    /// </summary>
    private void Start()
    {
        // Initialize button components
        InitializeButtons();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Finds and initializes the UI button components and attaches their listeners.
    /// Assumes buttons are named "ContinueGameButton", "SaveGameButton", "OptionsButton", and "MainMenuButton"
    /// as children of this GameObject.
=======
    /// Finds and assigns button components, and attaches their listeners.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Called when the MonoBehaviour will be destroyed.
    /// Removes listeners from buttons to prevent memory leaks.
=======
    /// Unity OnDestroy method. Removes button listeners to prevent memory leaks.
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    /// Handles the click event for the "Continue" button.
    /// Resumes the game by changing the game status.
=======
    /// Handles the Continue button click. Resumes the game and updates the game state.
>>>>>>> Stashed changes
    /// </summary>
    public void OnContinueClicked()
    {
        Debug.Log("Continue Game Clicked");
        // Resume the game and update the game status
        GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the "Save Game" button.
    /// Saves the current game state.
=======
    /// Handles the Save Game button click. Saves the current game state.
>>>>>>> Stashed changes
    /// </summary>
    public void OnSaveGameClicked()
    {
        Debug.Log("Save Game Clicked");
        // Save the game state
        GameManager.Instance.SaveGame();
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the "Options" button.
    /// Opens the settings menu by changing the game status.
=======
    /// Handles the Options button click. Opens the options menu and updates the game state.
>>>>>>> Stashed changes
    /// </summary>
    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        // Open the options menu and update the game status
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

    /// <summary>
<<<<<<< Updated upstream
    /// Handles the click event for the "Main Menu" button.
    /// Returns the player to the main menu.
=======
    /// Handles the Main Menu button click. Returns to the main menu and saves the game.
>>>>>>> Stashed changes
    /// </summary>
    public void OnMainMenuClicked()
    {
        Debug.Log("Main Menu Clicked");
        // Return to the main menu and save the game
        GameManager.Instance.GoMainMenu();
    }
}
