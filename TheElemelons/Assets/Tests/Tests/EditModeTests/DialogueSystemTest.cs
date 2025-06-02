// using NUnit.Framework;
// using UnityEngine;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine.UI;
// using UnityEngine.TestTools;

// public class DialogueSystemTest
// {
//     private GameObject dialogueManagerGO;
//     private GameObject dialogueUIGO;
//     private DialogueManager dialogueManager;
//     private DialogueUI dialogueUI;

//     // Dummy UI fields
//     private GameObject dialogueBoxPanel;
//     private TextMeshProUGUI nameText;
//     private Image portraitImage;
//     private TextMeshProUGUI dialogueText;
//     private Image dialogueBoxBackgroundImage;
//     private Image sceneImageDisplay;

//     [SetUp]
//     public void Setup()
// {
//     dialogueManagerGO = new GameObject("DialogueManager");
//     dialogueManager = dialogueManagerGO.AddComponent<DialogueManager>();

//     dialogueUIGO = new GameObject("DialogueUI");
//     dialogueUI = dialogueUIGO.AddComponent<DialogueUI>();

//     // Create separate GameObjects for each UI component
//     var dialogueBoxPanelGO = new GameObject("DialogueBoxPanel");
//     dialogueBoxPanelGO.transform.SetParent(dialogueUIGO.transform);
//     dialogueBoxPanel = dialogueBoxPanelGO;

//     var nameTextGO = new GameObject("NameText");
//     nameTextGO.transform.SetParent(dialogueUIGO.transform);
//     nameText = nameTextGO.AddComponent<TextMeshProUGUI>();

//     var portraitImageGO = new GameObject("PortraitImage");
//     portraitImageGO.transform.SetParent(dialogueUIGO.transform);
//     portraitImage = portraitImageGO.AddComponent<Image>();

//     var dialogueTextGO = new GameObject("DialogueText");
//     dialogueTextGO.transform.SetParent(dialogueUIGO.transform);
//     dialogueText = dialogueTextGO.AddComponent<TextMeshProUGUI>();

//     var dialogueBoxBackgroundImageGO = new GameObject("DialogueBoxBackgroundImage");
//     dialogueBoxBackgroundImageGO.transform.SetParent(dialogueUIGO.transform);
//     dialogueBoxBackgroundImage = dialogueBoxBackgroundImageGO.AddComponent<Image>();

//     var sceneImageDisplayGO = new GameObject("SceneImageDisplay");
//     sceneImageDisplayGO.transform.SetParent(dialogueUIGO.transform);
//     sceneImageDisplay = sceneImageDisplayGO.AddComponent<Image>();

//     // Assign via reflection as before...
//     typeof(DialogueUI).GetField("dialogueBoxPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//         .SetValue(dialogueUI, dialogueBoxPanel);
//     typeof(DialogueUI).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//         .SetValue(dialogueUI, nameText);
//     typeof(DialogueUI).GetField("portraitImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//         .SetValue(dialogueUI, portraitImage);
//     typeof(DialogueUI).GetField("dialogueText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//         .SetValue(dialogueUI, dialogueText);
//     typeof(DialogueUI).GetField("dialogueBoxBackgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//         .SetValue(dialogueUI, dialogueBoxBackgroundImage);
//     typeof(DialogueUI).GetField("sceneImageDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//         .SetValue(dialogueUI, sceneImageDisplay);
// }

//     [TearDown]
//     public void Teardown()
//     {
//         Object.DestroyImmediate(dialogueManagerGO);
//         Object.DestroyImmediate(dialogueUIGO);
//     }

//     // --- DialogueLineEntry test helpers ---
//     private DialogueLineEntry MakeEntry(LineType type, string text = null, Sprite sceneImage = null, Sprite portrait = null, Sprite boxOverride = null)
//     {
//         return new DialogueLineEntry
//         {
//             lineType = type,
//             lineSentenceText = text,
//             lineSceneImage = sceneImage,
//             linePortrait = portrait,
//             overrideDialogueBoxImage = boxOverride
//         };
//     }

//     private DialogueData MakeDialogue(params DialogueLineEntry[] lines)
//     {
//         var data = ScriptableObject.CreateInstance<DialogueData>();
//         data.speakerName = "Tester";
//         data.dialogueLines = new List<DialogueLineEntry>(lines);
//         return data;
//     }

//     // --- Test cases ---

//     [Test]
//     public void TextType_WithText_NoSceneImage_DisplaysText()
//     {
//         var entry = MakeEntry(LineType.Text, "Hello world!");
//         var data = MakeDialogue(entry);

//         bool eventFired = false;
//         DialogueManager.OnDialogueUpdate += (line) =>
//         {
//             eventFired = true;
//             Assert.AreEqual(LineType.Text, line.lineType);
//             Assert.AreEqual("Hello world!", line.lineSentenceText);
//         };

//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();

//         Assert.IsTrue(eventFired, "OnDialogueUpdate should fire.");
//     }

//     [Test]
//     public void TextType_WithText_AndSceneImage_IgnoresSceneImage()
//     {
//         var dummySprite = Sprite.Create(Texture2D.blackTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.Text, "Text with image", dummySprite);
//         var data = MakeDialogue(entry);

//         DialogueManager.OnDialogueUpdate += (line) =>
//         {
//             Assert.AreEqual(LineType.Text, line.lineType);
//             Assert.AreEqual("Text with image", line.lineSentenceText);
//             Assert.AreEqual(dummySprite, line.lineSceneImage);
//         };

//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//         // UI should ignore scene image for text lines (would be checked in PlayMode/UI test)
//     }

//     [Test]
//     public void TextType_NoText_NoSceneImage_EmptyText()
//     {
//         var entry = MakeEntry(LineType.Text, "");
//         var data = MakeDialogue(entry);

//         DialogueManager.OnDialogueUpdate += (line) =>
//         {
//             Assert.AreEqual("", line.lineSentenceText);
//         };

//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//     }

//     [Test]
//     public void SceneImageType_WithSceneImage_NoText_DisplaysSceneImage()
//     {
//         var dummySprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.SceneImage, null, dummySprite);
//         var data = MakeDialogue(entry);

//         DialogueManager.OnDialogueUpdate += (line) =>
//         {
//             Assert.AreEqual(LineType.SceneImage, line.lineType);
//             Assert.AreEqual(dummySprite, line.lineSceneImage);
//         };

//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//     }

//     [Test]
//     public void SceneImageType_WithSceneImage_AndText_IgnoresText()
//     {
//         var dummySprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.SceneImage, "Should be ignored", dummySprite);
//         var data = MakeDialogue(entry);

//         DialogueManager.OnDialogueUpdate += (line) =>
//         {
//             Assert.AreEqual(LineType.SceneImage, line.lineType);
//             Assert.AreEqual(dummySprite, line.lineSceneImage);
//             Assert.AreEqual("Should be ignored", line.lineSentenceText);
//         };

//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//         // UI should ignore text for scene image lines (would be checked in PlayMode/UI test)
//     }

//     [Test]
//     public void SceneImageType_NoSceneImage_ShowsWarning()
//     {
//         var entry = MakeEntry(LineType.SceneImage, null, null);
//         var data = MakeDialogue(entry);

//         LogAssert.Expect(LogType.Warning, "[DialogueManager] Null entry at line 0 in 'Tester'. Advancing again.");
//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//     }

//     [Test]
//     public void SceneImageType_NoSceneImage_WithText_ShowsWarning()
//     {
//         var entry = MakeEntry(LineType.SceneImage, "Should not show", null);
//         var data = MakeDialogue(entry);

//         LogAssert.Expect(LogType.Warning, "[DialogueManager] Null entry at line 0 in 'Tester'. Advancing again.");
//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//     }

//     [Test]
//     public void PortraitOverride_IsUsed()
//     {
//         var dummyPortrait = Sprite.Create(Texture2D.redTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.Text, "Portrait override", null, dummyPortrait);
//         var data = MakeDialogue(entry);

//         DialogueManager.OnDialogueUpdate += (line) =>
//         {
//             Assert.AreEqual(dummyPortrait, line.linePortrait);
//         };

//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//     }

//     [Test]
//     public void DialogueBoxOverride_IsUsed()
//     {
//         var dummyBox = Sprite.Create(Texture2D.blackTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.Text, "Box override", null, null, dummyBox);
//         var data = MakeDialogue(entry);

//         DialogueManager.OnDialogueUpdate += (line) =>
//         {
//             Assert.AreEqual(dummyBox, line.overrideDialogueBoxImage);
//         };

//         dialogueManager.StartDialogue(data);
//         dialogueManager.AdvanceDialogue();
//     }

//     [Test]
//     public void NullDialogueLineEntry_IsSkipped()
//     {
//         var data = MakeDialogue(null, MakeEntry(LineType.Text, "After null"));
//         dialogueManager.StartDialogue(data);
//         // Should skip null and process the next valid line
//         dialogueManager.AdvanceDialogue();
//         Assert.AreEqual(1, dialogueManager.currentLineIndex);
//     }

//     [Test]
//     public void DialogueDataWithNoLines_IsRejected()
//     {
//         var data = ScriptableObject.CreateInstance<DialogueData>();
//         data.speakerName = "Empty";
//         data.dialogueLines = new List<DialogueLineEntry>();

//         LogAssert.Expect(LogType.Error, "[DialogueManager] Cannot start dialogue: DialogueData is null, has no lines, or lines list is null.");
//         dialogueManager.StartDialogue(data);
//     }

//     [Test]
//     public void NullDialogueData_IsRejected()
//     {
//         LogAssert.Expect(LogType.Error, "[DialogueManager] Cannot start dialogue: DialogueData is null, has no lines, or lines list is null.");
//         dialogueManager.StartDialogue(null);
//     }
// }