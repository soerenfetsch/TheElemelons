using NUnit.Framework;
using UnityEngine;

public class DialogueSystemTest
{
    private DialogueManager manager;
    private DialogueData data;

    [SetUp]
    public void Setup()
    {
        var go = new GameObject();
        manager = go.AddComponent<DialogueManager>();
        data = ScriptableObject.CreateInstance<DialogueData>();
        data.name = "TestDialogue";
        data.speakerName = "Test";
        data.dialogueLines = new System.Collections.Generic.List<DialogueLineEntry>
        {
            new DialogueLineEntry { lineType = LineType.Text, sentenceText = "Hello" },
            new DialogueLineEntry { lineType = LineType.SceneImage, displaySceneImage = true }
        };
        data.sceneImages = new System.Collections.Generic.List<Sprite> { null };
        manager.dialogueAssets = new DialogueData[] { data };
        manager.BuildDialogueDictionary();
    }

    [Test]
    public void StartDialogue_SetsCurrentDialogue()
    {
        manager.StartDialogue("TestDialogue");
        Assert.IsNotNull(manager.currentDialogue);
        Assert.AreEqual("TestDialogue", manager.currentDialogue.name);
    }

    [Test]
    public void AdvanceDialogue_AdvancesLineIndex()
    {
        manager.StartDialogue("TestDialogue");
        int initialIndex = manager.currentLineIndex;
        manager.AdvanceDialogue();
        Assert.AreEqual(initialIndex + 1, manager.currentLineIndex);
    }

    [Test]
    public void EndDialogue_ClearsCurrentDialogue()
    {
        manager.StartDialogue("TestDialogue");
        manager.AdvanceDialogue();
        manager.AdvanceDialogue();
        Assert.IsNull(manager.currentDialogue);
    }
}