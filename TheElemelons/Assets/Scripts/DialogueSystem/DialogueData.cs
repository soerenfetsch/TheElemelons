using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Represents a set of dialogue data including speaker name, a list of portraits, and sentences.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    [Tooltip("Name of the speaker.")]
    public string speakerName;

    [Tooltip("List of portrait images of the speaker.")]
    public List<Sprite> portraits;

    [Tooltip("Dialogue Box image.")]
    public Sprite dialogueBoxImage;

    [Tooltip("Array of sentences in the dialogue.")]
    [TextArea(3, 10)]
    public string[] sentences;
}