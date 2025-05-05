using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    private Button continueButton;
    private Button saveGameButton;
    private Button optionsButton;
    private Button mainMenuButton;

    private void Start()
    {
        // Initialize button components
        InitializeButtons();
    }

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

    private void OnDestroy()
    {
        // Remove listeners to avoid memory leaks
        continueButton.onClick.RemoveListener(OnContinueClicked);
        saveGameButton.onClick.RemoveListener(OnSaveGameClicked);
        optionsButton.onClick.RemoveListener(OnOptionsClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }

    public void OnContinueClicked()
    {
        Debug.Log("Continue Game Clicked");
        // Resume the game and update the game state
        GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
    }

    public void OnSaveGameClicked()
    {
        Debug.Log("Save Game Clicked");
        // Save the game state
        GameManager.Instance.SaveProvider.SaveGame(new GameSaveData
        {
            progressionStage = GameManager.Instance.CurrentProgressionStage,
            // Add other relevant data to save here
        });
    }

    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        // Open the options menu and update the game state
        GameManager.Instance.ChangeGameStatus(GameStatus.SETTINGS_MENU);
    }

    public void OnMainMenuClicked()
    {
        Debug.Log("Main Menu Clicked");
        // Return to the main menu and save the game
        GameManager.Instance.ChangeGameStatus(GameStatus.MAIN_MENU);
    }
}
