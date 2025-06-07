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

    [Tooltip("List of default portrait images for the speaker. Used if a DialogueLineEntry does not have a portraitOverride.")]
    public Sprite linePortrait;

    [Tooltip("The dialogue text to display.")]
    [TextArea(3, 10)]
    public string lineSentenceText;

    [Tooltip("List of scene images that can be displayed during the dialogue. Referenced by DialogueLineEntries marked to display a scene image.")]
    public Sprite lineSceneImage;

    [Tooltip("Override Options for character interaction.")]
    public Sprite overrideDialogueBoxImage;
}

/// <summary>
/// Represents a set of dialogue data including speaker name, a list of portraits, sentences (now line entries), scene images, and dialogue box image.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Tooltip("toggle for dark background and larger image")]
    public bool isSlideShow;
    [Tooltip("Dialogue Box image.")]
    public Sprite dialogueBoxImage;

    [Tooltip("Name of the speaker.")]
    public string speakerName;

    [Tooltip("List of default portraits for the speaker. Used if a DialogueLineEntry does not have a portraitOverride.")]
    public Sprite portraitImage;

    [Tooltip("List of scene images for the dialogue.")]
    public Sprite sceneImage;

    [Tooltip("List of dialogue line entries. Each entry defines text, potential portrait overrides, and scene image triggers.")]
    public List<DialogueLineEntry> dialogueLines = new List<DialogueLineEntry>(); // Initialized
}