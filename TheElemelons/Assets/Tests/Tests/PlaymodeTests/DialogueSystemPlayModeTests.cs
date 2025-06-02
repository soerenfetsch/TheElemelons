// using System.Collections;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
// using TMPro;
// using UnityEngine.UI;

// public class DialogueSystemPlaymodeTests
// {
//     private GameObject dialogueManagerGO;
//     private GameObject dialogueUIGO;
//     private DialogueManager dialogueManager;
//     private DialogueUI dialogueUI;

//     private GameObject dialogueBoxPanel;
//     private TextMeshProUGUI nameText;
//     private Image portraitImage;
//     private TextMeshProUGUI dialogueText;
//     private Image dialogueBoxBackgroundImage;
//     private Image sceneImageDisplay;

//     [UnitySetUp]
//     public IEnumerator Setup()
//     {
//         dialogueManagerGO = new GameObject("DialogueManager");
//         dialogueManager = dialogueManagerGO.AddComponent<DialogueManager>();

//         dialogueUIGO = new GameObject("DialogueUI");
//         dialogueUI = dialogueUIGO.AddComponent<DialogueUI>();

//         dialogueBoxPanel = new GameObject("DialogueBoxPanel");
//         dialogueBoxPanel.transform.SetParent(dialogueUIGO.transform);
//         nameText = dialogueUIGO.AddComponent<TextMeshProUGUI>();
//         portraitImage = dialogueUIGO.AddComponent<Image>();
//         dialogueText = dialogueUIGO.AddComponent<TextMeshProUGUI>();
//         dialogueBoxBackgroundImage = dialogueUIGO.AddComponent<Image>();
//         sceneImageDisplay = dialogueUIGO.AddComponent<Image>();

//         // Assign private fields via reflection
//         typeof(DialogueUI).GetField("dialogueBoxPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//             .SetValue(dialogueUI, dialogueBoxPanel);
//         typeof(DialogueUI).GetField("nameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//             .SetValue(dialogueUI, nameText);
//         typeof(DialogueUI).GetField("portraitImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//             .SetValue(dialogueUI, portraitImage);
//         typeof(DialogueUI).GetField("dialogueText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//             .SetValue(dialogueUI, dialogueText);
//         typeof(DialogueUI).GetField("dialogueBoxBackgroundImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//             .SetValue(dialogueUI, dialogueBoxBackgroundImage);
//         typeof(DialogueUI).GetField("sceneImageDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
//             .SetValue(dialogueUI, sceneImageDisplay);

//         yield return null;
//     }

//     [UnityTearDown]
//     public IEnumerator Teardown()
//     {
//         Object.Destroy(dialogueManagerGO);
//         Object.Destroy(dialogueUIGO);
//         yield return null;
//     }

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
//         data.dialogueLines = new System.Collections.Generic.List<DialogueLineEntry>(lines);
//         return data;
//     }

//     [UnityTest]
//     public IEnumerator TextType_ShowsText_HidesSceneImage()
//     {
//         var entry = MakeEntry(LineType.Text, "Hello UI!");
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null; // Let UI update
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.IsTrue(dialogueBoxPanel.activeSelf, "Dialogue box should be visible.");
//         Assert.IsTrue(dialogueText.text.Contains("Hello UI!"), "Dialogue text should be shown.");
//         Assert.IsFalse(sceneImageDisplay.gameObject.activeSelf, "Scene image should be hidden.");
//     }

//     [UnityTest]
//     public IEnumerator SceneImageType_ShowsSceneImage_HidesDialogueBox()
//     {
//         var dummySprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.SceneImage, null, dummySprite);
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null;
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.IsFalse(dialogueBoxPanel.activeSelf, "Dialogue box should be hidden.");
//         Assert.IsTrue(sceneImageDisplay.gameObject.activeSelf, "Scene image should be visible.");
//         Assert.AreEqual(dummySprite, sceneImageDisplay.sprite, "Scene image should be set.");
//     }

//     [UnityTest]
//     public IEnumerator SceneImageType_WithText_IgnoresText()
//     {
//         var dummySprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.SceneImage, "Should not show", dummySprite);
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null;
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.IsFalse(dialogueBoxPanel.activeSelf, "Dialogue box should be hidden.");
//         Assert.IsTrue(sceneImageDisplay.gameObject.activeSelf, "Scene image should be visible.");
//         Assert.AreEqual(dummySprite, sceneImageDisplay.sprite, "Scene image should be set.");
//         Assert.IsTrue(string.IsNullOrEmpty(dialogueText.text) || dialogueText.text == "", "Dialogue text should be empty.");
//     }

//     [UnityTest]
//     public IEnumerator TextType_WithSceneImage_IgnoresSceneImage()
//     {
//         var dummySprite = Sprite.Create(Texture2D.blackTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.Text, "Text with image", dummySprite);
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null;
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.IsTrue(dialogueBoxPanel.activeSelf, "Dialogue box should be visible.");
//         Assert.IsTrue(dialogueText.text.Contains("Text with image"), "Dialogue text should be shown.");
//         Assert.IsFalse(sceneImageDisplay.gameObject.activeSelf, "Scene image should be hidden.");
//     }

//     [UnityTest]
//     public IEnumerator PortraitOverride_SetsPortraitImage()
//     {
//         var dummyPortrait = Sprite.Create(Texture2D.redTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.Text, "Portrait override", null, dummyPortrait);
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null;
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.AreEqual(dummyPortrait, portraitImage.sprite, "Portrait image should be set to override.");
//     }

//     [UnityTest]
//     public IEnumerator DialogueBoxOverride_SetsBoxImage()
//     {
//         var dummyBox = Sprite.Create(Texture2D.blackTexture, new Rect(0,0,1,1), Vector2.zero);
//         var entry = MakeEntry(LineType.Text, "Box override", null, null, dummyBox);
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null;
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.AreEqual(dummyBox, dialogueBoxBackgroundImage.sprite, "Dialogue box background should be set to override.");
//     }

//     [UnityTest]
//     public IEnumerator SceneImageType_NoSceneImage_ShowsNothing()
//     {
//         var entry = MakeEntry(LineType.SceneImage, null, null);
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null;
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.IsFalse(dialogueBoxPanel.activeSelf, "Dialogue box should be hidden.");
//         Assert.IsFalse(sceneImageDisplay.gameObject.activeSelf, "Scene image should be hidden.");
//     }

//     [UnityTest]
//     public IEnumerator TextType_NoText_ShowsEmptyText()
//     {
//         var entry = MakeEntry(LineType.Text, "");
//         var data = MakeDialogue(entry);

//         dialogueManager.StartDialogue(data);
//         yield return null;
//         dialogueManager.AdvanceDialogue();
//         yield return new WaitForSeconds(0.1f);

//         Assert.IsTrue(dialogueBoxPanel.activeSelf, "Dialogue box should be visible.");
//         Assert.IsTrue(string.IsNullOrEmpty(dialogueText.text), "Dialogue text should be empty.");
//     }
// }