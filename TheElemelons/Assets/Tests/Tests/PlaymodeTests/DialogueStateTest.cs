using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class DialogueStatePlayModeTests
{
    private GameObject gameManagerGameObject;
    private GameManager gameManager;
    private GameObject uiManagerGameObject;
    private UIManager uiManager;
    private GameObject inputManagerGameObject;
    private InputManager inputManager;
    private GameObject dialogueTriggerGameObject;
    private DialogueTrigger dialogueTrigger;
    private GameObject playerGameObject;
    private GameObject dialogueUIGameObject;
    private DialogueUI dialogueUI;

    [SetUp]
    public void Setup()
    {
        // Load the test scene
        SceneManager.LoadScene("GameManagerTestScene");

        // Find the necessary GameObjects in the scene
        gameManagerGameObject = GameObject.Find("GameManager");
        uiManagerGameObject = GameObject.Find("UIManager");
        inputManagerGameObject = GameObject.Find("Player");
        dialogueTriggerGameObject = GameObject.Find("NPC"); // Assuming you have an NPC in the scene
        playerGameObject = GameObject.Find("Player");
        dialogueUIGameObject = GameObject.Find("DialogueUI");

        // Get the components from the GameObjects
        gameManager = gameManagerGameObject.GetComponent<GameManager>();
        uiManager = uiManagerGameObject.GetComponent<UIManager>();
        inputManager = inputManagerGameObject.GetComponent<InputManager>();
        dialogueTrigger = dialogueTriggerGameObject.GetComponent<DialogueTrigger>();
        dialogueUI = dialogueUIGameObject.GetComponent<DialogueUI>();

        // Ensure that the components are not null
        Assert.IsNotNull(gameManager, "GameManager not found in the scene.");
        Assert.IsNotNull(uiManager, "UIManager not found in the scene.");
        Assert.IsNotNull(inputManager, "InputManager not found in the scene.");
        Assert.IsNotNull(dialogueTrigger, "DialogueTrigger not found in the scene.");
        Assert.IsNotNull(dialogueUI, "DialogueUI not found in the scene.");
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the scene after each test
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            UnityEngine.Object.DestroyImmediate(obj);
        }
    }

    [UnityTest]
    public IEnumerator Dialogue_EntersDialogueStateAndReturnsToIngame()
    {
        // Arrange
        // Ensure the game starts in the INGAME state
        gameManager.ChangeGameStatus(GameStatus.INGAME);
        yield return null;

        // Get the initial state of the Ingame panel
        bool ingamePanelInitialState = uiManager.ingamePanel.activeSelf;

        // Act
        // Simulate interacting with the dialogue trigger
        dialogueTrigger.Interact();
        yield return null;

        // Assert
        // Verify that the game status is now DIALOGUE
        Assert.AreEqual(GameStatus.DIALOGUE, gameManager.CurrentGameStatus, "Game status should be DIALOGUE.");

        // Verify that the Ingame panel is disabled
        Assert.IsFalse(uiManager.ingamePanel.activeSelf, "Ingame panel should be disabled during dialogue.");

        // Simulate advancing through the dialogue until it ends
        while (DialogueManager.Instance.HasDialogue())
        {
            DialogueManager.Instance.AdvanceDialogue();
            yield return null;
        }

        // Wait for the dialogue to end
        yield return new WaitForSeconds(0.5f);

        // Assert
        // Verify that the game status returns to INGAME after the dialogue ends
        Assert.AreEqual(GameStatus.INGAME, gameManager.CurrentGameStatus, "Game status should return to INGAME after dialogue.");

        // Verify that the Ingame panel is enabled again
        Assert.AreEqual(ingamePanelInitialState, uiManager.ingamePanel.activeSelf, "Ingame panel should be enabled after dialogue.");
    }

    [UnityTest]
    public IEnumerator DialogueUI_DisplaysCorrectDialogueLines()
    {
        // Arrange
        // Ensure the game starts in the INGAME state
        gameManager.ChangeGameStatus(GameStatus.INGAME);
        yield return null;

        // Simulate interacting with the dialogue trigger
        dialogueTrigger.Interact();
        yield return null;

        // Get the DialogueData from the DialogueTrigger
        DialogueData dialogue = dialogueTrigger.basic_dialogues_list[0];

        // Act & Assert
        // Verify that each dialogue line is displayed correctly
        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            // Get the expected dialogue line
            string expectedLine = dialogue.sentences[i];

            // Wait for the dialogue line to be displayed
            yield return new WaitForSeconds(0.1f);

            // Get the actual dialogue line from the UI
            string actualLine = dialogueUI.dialogueText.text;

            // Assert that the actual line matches the expected line
            Assert.AreEqual(expectedLine, actualLine, $"Dialogue line {i} is incorrect.");

            // Advance to the next dialogue line
            DialogueManager.Instance.AdvanceDialogue();
            yield return null;
        }
    }
}