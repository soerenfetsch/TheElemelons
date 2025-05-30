using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the type of dialogue line.
/// </summary>
public enum LineType
{
    Text,
    SceneImage
}

/// <summary>
/// Represents a single entry in a dialogue sequence, which can be text or a scene image trigger.
/// </summary>
[System.Serializable]
public class DialogueLineEntry
{
    [Tooltip("Type of this dialogue entry (Text or SceneImage).")]
    public LineType lineType = LineType.Text;

    [Tooltip("The dialogue text to display. Ignored if lineType is SceneImage and displaySceneImage is true, but can be used for notes.")]
    [TextArea(3, 10)]
    public string sentenceText;

    [Tooltip("Optional: Override the default portrait for this specific line. If null, the system will try to use an indexed portrait from DialogueData.portraits.")]
    public Sprite portraitOverride;

    [Tooltip("If true, and lineType is SceneImage (or if you want to force a scene image even with text), display the next available scene image from DialogueData.sceneImages. The sentenceText might be used as a caption or ignored depending on UI setup.")]
    public bool displaySceneImage = false;
}

/// <summary>
/// Represents a set of dialogue data including speaker name, a list of portraits, sentences (now line entries), scene images, and dialogue box image.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Name of the speaker.")]
    public string speakerName;

    [Tooltip("List of default portrait images for the speaker. Used if a DialogueLineEntry does not have a portraitOverride.")]
    public List<Sprite> portraits = new List<Sprite>(); // Initialized

    [Tooltip("Dialogue Box image.")]
    public Sprite dialogueBoxImage;

    [Tooltip("List of dialogue line entries. Each entry defines text, potential portrait overrides, and scene image triggers.")]
    public List<DialogueLineEntry> dialogueLines = new List<DialogueLineEntry>(); // Initialized

    [Tooltip("List of scene images that can be displayed during the dialogue. Referenced by DialogueLineEntries marked to display a scene image.")]
    public List<Sprite> sceneImages = new List<Sprite>(); // Initialized
}