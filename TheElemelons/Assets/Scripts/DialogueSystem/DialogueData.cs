using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the type of dialogue line (e.g., text or scene image).
/// </summary>
public enum LineType
{
    /// <summary>
    /// Standard text line.
    /// </summary>
    Text,
    /// <summary>
    /// Line that triggers a scene image display.
    /// </summary>
    SceneImage
}

/// <summary>
/// Represents a single entry in a dialogue sequence, which can be text or a scene image trigger.
/// </summary>
[System.Serializable]
public class DialogueLineEntry
{
    /// <summary>
    /// Portrait image for this line. If not set, uses the default portrait from the DialogueData.
    /// </summary>
    [Tooltip("List of default portrait images for the speaker. Used if a DialogueLineEntry does not have a portraitOverride.")]
    public Sprite linePortrait;

    /// <summary>
    /// The dialogue text to display for this line.
    /// </summary>
    [Tooltip("The dialogue text to display.")]
    [TextArea(3, 10)]
    public string lineSentenceText;

    /// <summary>
    /// Scene image to display for this line, if any.
    /// </summary>
    [Tooltip("List of scene images that can be displayed during the dialogue. Referenced by DialogueLineEntries marked to display a scene image.")]
    public Sprite lineSceneImage;

    /// <summary>
    /// Optional override for the dialogue box image for this line.
    /// </summary>
    [Tooltip("Override Options for character interaction.")]
    public Sprite overrideDialogueBoxImage;
}

/// <summary>
/// Represents a set of dialogue data including speaker name, portraits, dialogue lines, scene images, and dialogue box image.
/// Used as a ScriptableObject for dialogue sequences.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    /// <summary>
    /// If true, enables dark background and larger image for slideshow-style dialogue.
    /// </summary>
    [Tooltip("toggle for dark background and larger image")]
    public bool isSlideShow;

    /// <summary>
    /// The default dialogue box image for this dialogue.
    /// </summary>
    [Tooltip("Dialogue Box image.")]
    public Sprite dialogueBoxImage;

    /// <summary>
    /// Name of the speaker for this dialogue.
    /// </summary>
    [Tooltip("Name of the speaker.")]
    public string speakerName;

    /// <summary>
    /// Default portrait image for the speaker. Used if a DialogueLineEntry does not have a portrait override.
    /// </summary>
    [Tooltip("List of default portraits for the speaker. Used if a DialogueLineEntry does not have a portraitOverride.")]
    public Sprite portraitImage;

    /// <summary>
    /// Default scene image for the dialogue. Used if a DialogueLineEntry does not have a scene image override.
    /// </summary>
    [Tooltip("List of scene images for the dialogue.")]
    public Sprite sceneImage;

    /// <summary>
    /// List of dialogue line entries. Each entry defines text, potential portrait overrides, and scene image triggers.
    /// </summary>
    [Tooltip("List of dialogue line entries. Each entry defines text, potential portrait overrides, and scene image triggers.")]
    public List<DialogueLineEntry> dialogueLines = new List<DialogueLineEntry>();
}