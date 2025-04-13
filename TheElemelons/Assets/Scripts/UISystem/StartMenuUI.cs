using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Start Menu UI and handles navigation actions such as starting a new game,
/// continuing a saved game, accessing options, or quitting the game.
/// Implements IUIPanel so that UIManager can control its visibility.
/// </summary>
public class StartMenuUI : MonoBehaviour, IUIPanel
{
    private Button newGameButton;
    private Button continueGameButton;
    private Button optionsButton;
    private Button quitGameButton;

    /// <summary>
    /// Called at startup; initializes the button components and disables the Continue Game button.
    /// </summary>
    private void Start()
    {
        // Initialize button components.
        InitializeButtons();
        // Initially disable the Continue Game button (until a saved game is detected).
        DisableContinueGame();
    }

    /// <summary>
    /// Retrieves the button components and attaches event listeners for each button.
    /// </summary>
    public void InitializeButtons()
    {
        // Get button components from the hierarchy.
        newGameButton = transform.Find("NewGameButton").GetComponent<Button>();
        continueGameButton = transform.Find("ContinueGameButton").GetComponent<Button>();
        optionsButton = transform.Find("OptionsButton").GetComponent<Button>();
        quitGameButton = transform.Find("QuitGameButton").GetComponent<Button>();

        // Attach button listeners.
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueGameButton.onClick.AddListener(OnContinueGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        quitGameButton.onClick.AddListener(OnQuitGameClicked);
    }

    /// <summary>
    /// Removes the attached event listeners when the object is destroyed to avoid memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        newGameButton.onClick.RemoveListener(OnNewGameClicked);
        continueGameButton.onClick.RemoveListener(OnContinueGameClicked);
        optionsButton.onClick.RemoveListener(OnOptionsClicked);
        quitGameButton.onClick.RemoveListener(OnQuitGameClicked);
    }

    /// <summary>
    /// Called when the New Game button is clicked.
    /// Initiates a new game by calling GameManager.Instance.NewGame().
    /// </summary>
    public void OnNewGameClicked()
    {
        Logger.Log("StartMenuUI: New Game Clicked");
        GameManager.Instance.NewGame();
    }

    /// <summary>
    /// Called when the Continue Game button is clicked.
    /// Loads the saved game by calling GameManager.Instance.ContinueFromSave().
    /// </summary>
    public void OnContinueGameClicked()
    {
        Logger.Log("StartMenuUI: Continue Game Clicked");
        // Since we now use an asynchronous load, you might consider calling a new async version:
        GameManager.Instance.ContinueFromSaveAsync();
    }

    /// <summary>
    /// Called when the Options button is clicked.
    /// Transitions to the settings state by calling GameManager.Instance.ClickOptions().
    /// </summary>
    public void OnOptionsClicked()
    {
        Logger.Log("StartMenuUI: Options Clicked");
        GameManager.Instance.ClickOptions();
    }

    /// <summary>
    /// Called when the Quit Game button is clicked.
    /// Saves the game (if needed) and quits the application by calling GameManager.Instance.QuitGame().
    /// </summary>
    public void OnQuitGameClicked()
    {
        Logger.Log("StartMenuUI: Quit Game Clicked");
        GameManager.Instance.QuitGame();
    }

    /// <summary>
    /// Enables the Continue Game button so that the user can resume a saved game.
    /// </summary>
    public void EnableContinueGame()
    {
        continueGameButton.interactable = true;
    }

    /// <summary>
    /// Disables the Continue Game button when no saved data is available.
    /// </summary>
    public void DisableContinueGame()
    {
        continueGameButton.interactable = false;
    }

    /// <summary>
    /// Implements the IUIPanel interface to show the panel.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Implements the IUIPanel interface to hide the panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
