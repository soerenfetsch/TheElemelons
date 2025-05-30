using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the type of dialogue line.
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
    [Tooltip("Image will be shown at the end.")]
    public Sprite imageSprite;
}