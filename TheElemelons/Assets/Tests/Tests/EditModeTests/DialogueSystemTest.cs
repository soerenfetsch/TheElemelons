using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using Moq;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using TMPro;

public class DialogueSystemTests
{
    private DialogueManager dialogueManager;
    private GameObject dialogueManagerGameObject;
    private DialogueData dialogueData;

    [SetUp]
    public void Setup()
    {
        // Create a new GameObject for the DialogueManager
        dialogueManagerGameObject = new GameObject("DialogueManager");
        dialogueManager = dialogueManagerGameObject.AddComponent<DialogueManager>();

        // Create a DialogueData asset
        dialogueData = ScriptableObject.CreateInstance<DialogueData>();
        dialogueData.name = "TestDialogue";
        dialogueData.speakerName = "TestSpeaker";
        dialogueData.sentences = new string[] { "Sentence 1", "Sentence 2" };

        // Assign the DialogueData asset to the DialogueManager
        dialogueManager.dialogueAssets = new DialogueData[] { dialogueData };

        // Build the dialogue dictionary
        dialogueManager.gameObject.AddComponent<Image>();
        dialogueManager.imageObject = dialogueManager.GetComponent<Image>().gameObject;
        dialogueManager.imageObject.SetActive(false);
        dialogueManager.gameObject.AddComponent<DialogueUI>();
        dialogueManager.GetComponent<DialogueUI>().enabled = false;
        dialogueManager.GetComponent<DialogueUI>().gameObject.SetActive(false);
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<TextMeshProUGUI>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<AudioSource>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.GetComponent<DialogueUI>().gameObject.AddComponent<Image>();
        dialogueManager.BuildDialogueDictionary();
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up after each test
        Object.DestroyImmediate(dialogueManagerGameObject);
        Object.DestroyImmediate(dialogueData);
    }

    [Test]
    public void DialogueManager_BuildDialogueDictionary_LoadsDialogueData()
    {
        // Assert
        Assert.IsTrue(dialogueManager.HasDialogue(), "DialogueManager should have dialogue data.");
    }

    [Test]
    public void DialogueManager_StartDialogue_SetsCurrentDialogue()
    {
        // Act
        dialogueManager.StartDialogue("TestDialogue");

        // Assert
        Assert.IsNotNull(dialogueManager.currentDialogue, "Current dialogue should not be null.");
        Assert.AreEqual("TestDialogue", dialogueManager.currentDialogue.name, "Current dialogue name is incorrect.");
    }

    [Test]
    public void DialogueManager_AdvanceDialogue_AdvancesToNextLine()
    {
        // Arrange
        dialogueManager.StartDialogue("TestDialogue");
        int initialLineIndex = dialogueManager.currentLineIndex;

        // Act
        dialogueManager.AdvanceDialogue();

        // Assert
        Assert.AreEqual(initialLineIndex + 1, dialogueManager.currentLineIndex, "Dialogue should advance to the next line.");
    }

    [Test]
    public void DialogueManager_EndDialogue_ClearsCurrentDialogue()
    {
        // Arrange
        dialogueManager.StartDialogue("TestDialogue");

        // Act
        dialogueManager.AdvanceDialogue();
        dialogueManager.AdvanceDialogue();
        // Assert
        Assert.IsNull(dialogueManager.currentDialogue, "Current dialogue should be null after ending.");
    }

    [Test]
    public void DialogueManager_HasDialogue_ReturnsTrueWhenDialogueIsActive()
    {
        // Arrange
        dialogueManager.StartDialogue("TestDialogue");

        // Act
        bool hasDialogue = dialogueManager.HasDialogue();

        // Assert
        Assert.IsTrue(hasDialogue, "HasDialogue should return true when dialogue is active.");
    }

    [Test]
    public void DialogueManager_HasDialogue_ReturnsFalseWhenDialogueIsNotActive()
    {
        // Act
        bool hasDialogue = dialogueManager.HasDialogue();

        // Assert
        Assert.IsFalse(hasDialogue, "HasDialogue should return false when dialogue is not active.");
    }
}