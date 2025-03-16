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
        // TODO: Add logic to resume the game
    }

    public void OnSaveGameClicked()
    {
        Debug.Log("Save Game Clicked");
        // TODO: Add logic to save the game
    }

    public void OnOptionsClicked()
    {
        Debug.Log("Options Clicked");
        // TODO: Add logic to open the options menu
    }

    public void OnMainMenuClicked()
    {
        Debug.Log("Main Menu Clicked");
        // TODO: Add logic to return to the main menu
    }
}
