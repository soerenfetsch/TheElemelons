using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class StartMenuUITests
{
    private GameObject startMenuObject;
    private StartMenuUI startMenuUI;
    private Button newGameButton;
    private Button continueGameButton;
    private Button optionsButton;
    private Button quitGameButton;
    
    [SetUp]
    public void Setup()
    {
        // Create a new GameObject and attach StartMenuUI
        startMenuObject = new GameObject();
        startMenuUI = startMenuObject.AddComponent<StartMenuUI>();

        // Create mock buttons
        newGameButton = CreateButton("NewGameButton");
        continueGameButton = CreateButton("ContinueGameButton");
        optionsButton = CreateButton("OptionsButton");
        quitGameButton = CreateButton("QuitGameButton");

        // Initialize the UI script (mimicking Unity lifecycle)
        startMenuUI.InitializeButtons();

        // Initially disable the continue button
        startMenuUI.DisableContinueGame();
    }

    private Button CreateButton(string buttonName)
    {
        GameObject buttonObj = new GameObject(buttonName);
        buttonObj.AddComponent<Button>();
        buttonObj.transform.SetParent(startMenuObject.transform); // Set as child

        return buttonObj.GetComponent<Button>();
    }

    [Test]
    public void ContinueGameButton_InitiallyDisabled()
    {
        Assert.IsFalse(continueGameButton.interactable, "Continue Game button should be disabled initially.");
    }

    [Test]
    public void EnableContinueGame_MakesButtonInteractable()
    {
        startMenuUI.EnableContinueGame();
        Assert.IsTrue(continueGameButton.interactable, "Continue Game button should be enabled.");
    }

    [Test]
    public void DisableContinueGame_MakesButtonNonInteractable()
    {
        startMenuUI.EnableContinueGame(); // Enable first
        startMenuUI.DisableContinueGame(); // Then disable
        Assert.IsFalse(continueGameButton.interactable, "Continue Game button should be disabled again.");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(startMenuObject);
    }
}
