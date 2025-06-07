using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI elements and functionality of the main start menu.
/// </summary>
public class StartMenuUI : MonoBehaviour
{
    /// <summary>
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
    /// </summary>
    private Button quitGameButton;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes button components and sets the initial state of the continue button.
    /// </summary>
    private void Awake()
    {
        // Initialize button components
        InitializeButtons();
        // Initially disable the continue button
        DisableContinueGame();
    }

    /// <summary>
    /// Finds and initializes the UI button components and attaches their listeners.
    /// Assumes buttons are named "NewGameButton", "ContinueGameButton", "OptionsButton", and "QuitGameButton"
    /// as children of this GameObject.
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
    /// Called when the MonoBehaviour will be destroyed.
    /// Removes listeners from buttons to prevent memory leaks.
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
    /// Handles the click event for the "New Game" button.
    /// Initiates a new game session via the GameManager.
    /// </summary>
    public void OnNewGameClicked()
    {
        Debug.Log("New Game Clicked");
        GameManager.Instance.NewGame();
    }

    /// <summary>
    /// Handles the click event for the "Continue Game" button.
    /// Loads a saved game session via the GameManager.
    /// </summary>
    public void OnContinueGameClicked()
    {
        Debug.Log("Continue Game Clicked");
        GameManager.Instance.ContinueSavedGame();
    }

    /// <summary>
    /// Handles the click event for the "Options" button.
    /// Changes the game state to open the settings menu.
    /// </summary>
    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

    /// <summary>
    /// Handles the click event for the "Quit Game" button.
    /// Exits the application or stops play mode in the editor.
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
