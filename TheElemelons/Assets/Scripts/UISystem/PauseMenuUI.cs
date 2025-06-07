using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI elements and functionality of the pause menu.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    /// <summary>
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
    /// </summary>
    private Button mainMenuButton;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes the button components.
    /// </summary>
    private void Start()
    {
        // Initialize button components
        InitializeButtons();
    }

    /// <summary>
    /// Finds and initializes the UI button components and attaches their listeners.
    /// Assumes buttons are named "ContinueGameButton", "SaveGameButton", "OptionsButton", and "MainMenuButton"
    /// as children of this GameObject.
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
    /// Called when the MonoBehaviour will be destroyed.
    /// Removes listeners from buttons to prevent memory leaks.
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
    /// Handles the click event for the "Continue" button.
    /// Resumes the game by changing the game status.
    /// </summary>
    public void OnContinueClicked()
    {
        Debug.Log("Continue Game Clicked");
        // Resume the game and update the game status
        GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
    }

    /// <summary>
    /// Handles the click event for the "Save Game" button.
    /// Saves the current game state.
    /// </summary>
    public void OnSaveGameClicked()
    {
        Debug.Log("Save Game Clicked");
        // Save the game state
        GameManager.Instance.SaveGame();
    }

    /// <summary>
    /// Handles the click event for the "Options" button.
    /// Opens the settings menu by changing the game status.
    /// </summary>
    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        // Open the options menu and update the game status
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

    /// <summary>
    /// Handles the click event for the "Main Menu" button.
    /// Returns the player to the main menu.
    /// </summary>
    public void OnMainMenuClicked()
    {
        Debug.Log("Main Menu Clicked");
        // Return to the main menu and save the game
        GameManager.Instance.GoMainMenu();
    }
}
