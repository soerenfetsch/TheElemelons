using NUnit.Framework;
using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using System;
using UnityEngine.SceneManagement;

public class GameManagerStateTransitionTests
{
    private GameObject gameManagerGameObject;
    private GameObject uiManagerObject;
    private GameManager gameManager;
    private bool startMainGameStageCalled; // Add this line

    [SetUp]
    public void Setup()
    {
        gameManagerGameObject = new GameObject("GameManager");
        gameManager = gameManagerGameObject.AddComponent<GameManager>();
        gameManager.SaveProvider = new GameSaveProvider(); // Ensure SaveProvider is initialized
        startMainGameStageCalled = false; // Reset the flag
        uiManagerObject = new GameObject("UIManager");
        uiManagerObject.AddComponent<UIManager>();
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
}