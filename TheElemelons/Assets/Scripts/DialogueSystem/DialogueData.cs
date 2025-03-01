using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a set of dialogue data including speaker name, portrait, and sentences.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Name of the speaker.")]
    public string speakerName;  // Store the speaker name

    [Tooltip("Portrait image of the speaker.")]
    public Sprite portrait;  // Store the speaker portrait

    [Tooltip("Dialogue Box image.")]
    public Sprite dialogueBoxImage;  // Store the dialogue box image

    [Tooltip("Array of sentences in the dialogue.")]
    [TextArea(3, 10)]
    public string[] sentences; // Store dialogue text
}
