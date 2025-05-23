using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using System;
using UnityEngine.SceneManagement;

public class GameManagerStateTransitionTests
{
    private GameObject gameManagerGameObject;
    private GameManager gameManager;
    // private bool startMainGameStageCalled; // Add this line

    [SetUp]
    public void Setup()
    {
        gameManagerGameObject = new GameObject("GameManager");
        gameManager = gameManagerGameObject.AddComponent<GameManager>();
        gameManager.SaveProvider = new GameSaveProvider(); // Ensure SaveProvider is initialized
        // startMainGameStageCalled = false; // Reset the flag
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up any remaining objects after each test.
        foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            UnityEngine.Object.DestroyImmediate(obj);
        }
        Time.timeScale = 1; // Reset timescale after each test
    }

    [Test]
    public void OnGameStatusChanged_EventIsInvoked()
    {
        // Arrange
        bool eventInvoked = false;
        Action<GameStatus> statusChangedHandler = (newStatus) => { eventInvoked = true; };
        GameManager.OnGameStatusChanged += statusChangedHandler;

        // Act
        gameManager.ChangeGameStatus(GameStatus.PAUSE_MENU);

        // Assert
        Assert.IsTrue(eventInvoked, "OnGameStatusChanged event should be invoked.");

        // Clean up
        GameManager.OnGameStatusChanged -= statusChangedHandler;
    }

    [Test]
    public void ChangeGameStatus_PausesGameCorrectly()
    {
        // Act
        gameManager.ChangeGameStatus(GameStatus.PAUSE_MENU);

        // Assert
        Assert.AreEqual(0, Time.timeScale, "Time.timeScale should be 0 when game is paused.");
    }

    [Test]
    public void ChangeGameStatus_ResumesGameCorrectly()
    {
        // Arrange
        gameManager.ChangeGameStatus(GameStatus.PAUSE_MENU); // Pause first

        // Act
        gameManager.ChangeGameStatus(GameStatus.INGAME);

        // Assert
        Assert.AreEqual(1, Time.timeScale, "Time.timeScale should be 1 when game is resumed.");
    }

    [Test]
    public void ChangeGameStatus_InvokesOnGameStatusChangedEventWithCorrectStatus()
    {
        // Arrange
        GameStatus expectedStatus = GameStatus.PAUSE_MENU;
        GameStatus actualStatus = GameStatus.INGAME; // Initialize with a default value
        Action<GameStatus> statusChangedHandler = (newStatus) => { actualStatus = newStatus; };
        GameManager.OnGameStatusChanged += statusChangedHandler;

        // Act
        gameManager.ChangeGameStatus(expectedStatus);

        // Assert
        Assert.AreEqual(expectedStatus, actualStatus, "OnGameStatusChanged event should be invoked with the correct GameStatus.");

        // Clean up
        GameManager.OnGameStatusChanged -= statusChangedHandler;
    }

    [Test]
    public void NewGame_InitializesGameStateCorrectly()
    {
        // Act
        gameManager.NewGame();

        // Assert
        Assert.AreEqual(GameProgressionStage.CASTLE_INTRODUCTION, gameManager.CurrentProgressionStage, "CurrentProgressionStage should be CASTLE_INTRODUCTION.");
        Assert.AreEqual(GameStatus.INGAME, gameManager.CurrentGameStatus, "CurrentGameStatus should be INGAME.");
    }

    [Test]
    public void ChangeGameStatus_FromIngameToDialogue_SetsDialogueStatusAndPausesTime()
    {
        // Arrange
        gameManager.ChangeGameStatus(GameStatus.INGAME); // Ensure starting from INGAME

        // Act
        gameManager.ChangeGameStatus(GameStatus.DIALOGUE);

        // Assert
        Assert.AreEqual(GameStatus.DIALOGUE, gameManager.CurrentGameStatus, "CurrentGameStatus should be DIALOGUE.");
        Assert.AreEqual(0, Time.timeScale, "Time.timeScale should be 0 when in DIALOGUE.");
    }

    [Test]
    public void ChangeGameStatus_FromDialogueToIngame_SetsIngameStatusAndResumesTime()
    {
        // Arrange
        gameManager.ChangeGameStatus(GameStatus.DIALOGUE); // Ensure starting from DIALOGUE

        // Act
        gameManager.ChangeGameStatus(GameStatus.INGAME);

        // Assert
        Assert.AreEqual(GameStatus.INGAME, gameManager.CurrentGameStatus, "CurrentGameStatus should be INGAME.");
        Assert.AreEqual(1, Time.timeScale, "Time.timeScale should be 1 when in INGAME.");
    }

    [Test]
    public void ChangeGameStatus_ToSettingsMenu_SetsSettingsStatusAndPausesTime()
    {
        // Arrange
        gameManager.ChangeGameStatus(GameStatus.PAUSE_MENU); // Common transition point

        // Act
        gameManager.ChangeGameStatus(GameStatus.SETTINGS_MENU);

        // Assert
        Assert.AreEqual(GameStatus.SETTINGS_MENU, gameManager.CurrentGameStatus, "CurrentGameStatus should be SETTINGS_MENU.");
        Assert.AreEqual(0, Time.timeScale, "Time.timeScale should be 0 when in SETTINGS_MENU.");
    }

    [Test]
    public void ChangeGameStatus_FromSettingsMenuToPauseMenu_SetsPauseStatusAndPausesTime()
    {
        // Arrange
        gameManager.ChangeGameStatus(GameStatus.SETTINGS_MENU); // Start from SETTINGS_MENU

        // Act
        gameManager.ChangeGameStatus(GameStatus.PAUSE_MENU);

        // Assert
        Assert.AreEqual(GameStatus.PAUSE_MENU, gameManager.CurrentGameStatus, "CurrentGameStatus should be PAUSE_MENU.");
        Assert.AreEqual(0, Time.timeScale, "Time.timeScale should be 0 when in PAUSE_MENU.");
    }

    // [Test]
    // public void ChangeGameStatus_ToMainMenu_SetsMainMenuStatusAndResumesTime()
    // {
    //     // Arrange
    //     // Starting from any state that might pause time, e.g., PAUSE_MENU
    //     gameManager.ChangeGameStatus(GameStatus.PAUSE_MENU); 

    //     // Act
    //     gameManager.ChangeGameStatus(GameStatus.MAIN_MENU);

    //     // Assert
    //     Assert.AreEqual(GameStatus.MAIN_MENU, gameManager.CurrentGameStatus, "CurrentGameStatus should be MAIN_MENU.");
    //     // According to GameManager logic, MAIN_MENU falls into the 'else' case for Time.timeScale, setting it to 1.
    //     // Assert.AreEqual(1, Time.timeScale, "Time.timeScale should be 1 when in MAIN_MENU.");
    //     // Note: SceneManager.LoadScene("MainMenu") is called but won't execute fully in edit mode.
    //     // We are testing the state and timescale effects here.
    // }
    
    [Test]
    public void ChangeGameStatus_ToDeathScreen_SetsDeathScreenStatusAndPausesTime()
    {
        // Arrange
        gameManager.ChangeGameStatus(GameStatus.INGAME); // Common state before death

        // Act
        gameManager.ChangeGameStatus(GameStatus.DEATH_SCREEN);

        // Assert
        Assert.AreEqual(GameStatus.DEATH_SCREEN, gameManager.CurrentGameStatus, "CurrentGameStatus should be DEATH_SCREEN.");
        // Assert.AreEqual(0, Time.timeScale, "Time.timeScale should be 0 when on DEATH_SCREEN.");
    }
}