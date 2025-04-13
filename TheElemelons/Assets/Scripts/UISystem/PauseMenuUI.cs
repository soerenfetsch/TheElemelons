using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Pause Menu UI by initializing button components and handling user interactions
/// to trigger state changes via the GameManager. Implements IUIPanel to allow centralized UI management.
/// </summary>
public class PauseMenuUI : MonoBehaviour, IUIPanel
{
    private Button continueButton;
    private Button saveGameButton;
    private Button optionsButton;
    private Button mainMenuButton;

    private void Start()
    {
        InitializeButtons();
    }

    /// <summary>
    /// Retrieves the button components and attaches event listeners.
    /// </summary>
    public void InitializeButtons()
    {
        continueButton = transform.Find("ContinueGameButton").GetComponent<Button>();
        saveGameButton = transform.Find("SaveGameButton").GetComponent<Button>();
        optionsButton = transform.Find("OptionsButton").GetComponent<Button>();
        mainMenuButton = transform.Find("MainMenuButton").GetComponent<Button>();

        continueButton.onClick.AddListener(OnContinueClicked);
        saveGameButton.onClick.AddListener(OnSaveGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnDestroy()
    {
        continueButton.onClick.RemoveListener(OnContinueClicked);
        saveGameButton.onClick.RemoveListener(OnSaveGameClicked);
        optionsButton.onClick.RemoveListener(OnOptionsClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }

    /// <summary>
    /// Called when the "Continue Game" button is clicked.
    /// Resumes gameplay.
    /// </summary>
    public void OnContinueClicked()
    {
        Logger.Log("PauseMenuUI: Continue Game Clicked");
        GameManager.Instance.ContinueGame();
    }

    /// <summary>
    /// Called when the "Save Game" button is clicked.
    /// Triggers game saving by invoking GameManager's centralized save function.
    /// </summary>
    public void OnSaveGameClicked()
    {
        Logger.Log("PauseMenuUI: Save Game Clicked");
        // Use GameManager’s SaveGame() rather than accessing the SaveProvider directly.
        GameManager.Instance.SaveGame();
    }

    /// <summary>
    /// Called when the "Options" button is clicked.
    /// Transitions to the Settings state.
    /// </summary>
    public void OnOptionsClicked()
    {
        Logger.Log("PauseMenuUI: Options Clicked");
        GameManager.Instance.ClickOptions();
    }

    /// <summary>
    /// Called when the "Main Menu" button is clicked.
    /// Transitions the game to the Main Menu state.
    /// </summary>
    public void OnMainMenuClicked()
    {
        Logger.Log("PauseMenuUI: Main Menu Clicked");
        GameManager.Instance.GoToMainMenu();
    }

    #region IUIPanel Implementation

    /// <summary>
    /// Shows the pause menu panel.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the pause menu panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
