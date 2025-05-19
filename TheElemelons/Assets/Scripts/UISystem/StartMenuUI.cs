using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    private Button newGameButton;
    private Button continueGameButton;
    private Button optionsButton;
    private Button quitGameButton;

    private void Awake()
    {
        // Initialize button components
        InitializeButtons();
        // Initially disable the continue button
        DisableContinueGame();
    }

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

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        newGameButton.onClick.RemoveListener(OnNewGameClicked);
        continueGameButton.onClick.RemoveListener(OnContinueGameClicked);
        optionsButton.onClick.RemoveListener(OnOptionsClicked);
        quitGameButton.onClick.RemoveListener(OnQuitGameClicked);
    }

    public void OnNewGameClicked()
    {
        Debug.Log("New Game Clicked");
        GameManager.Instance.NewGame();
    }

    public void OnContinueGameClicked()
    {
        Debug.Log("Continue Game Clicked");
        GameManager.Instance.ContinueSavedGame();
    }

    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

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
    /// Enables the Continue Game button (when a saved game is detected).
    /// </summary>
    public void EnableContinueGame()
    {
        continueGameButton.interactable = true;
    }

    /// <summary>
    /// Disables the Continue Game button (if no save data is found).
    /// </summary>
    public void DisableContinueGame()
    {
        continueGameButton.interactable = false;
    }
}
