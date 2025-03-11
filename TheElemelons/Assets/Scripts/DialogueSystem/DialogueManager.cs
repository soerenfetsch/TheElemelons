using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized manager for handling dialogue logic.
/// Loads dialogue data from ScriptableObjects and triggers events for UI updates,
/// allowing the dialogue system to be modular and decoupled from direct UI control.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    // Singleton instance.
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Assets")]
    [SerializeField] private DialogueData[] dialogueAssets; // Assign DialogueData assets via Inspector.

    // Dictionary for fast dialogue lookup.
    private Dictionary<string, DialogueData> dialogueDictionary = new Dictionary<string, DialogueData>();

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;

    // Events for dialogue flow.
    public static event Action<string, Sprite> OnDialogueStarted;    // Fires when dialogue begins (speaker name, initial portrait)
    public static event Action<string> OnDialogueProgressed;           // Fires for each new line of dialogue
    public static event Action OnDialogueEnded;                        // Fires when dialogue completes

    private void Awake()
    {
        // Ensure a single instance exists.
        if (Instance == null)
        {
            Instance = this;
            // Optionally: DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        BuildDialogueDictionary();
    }

    /// <summary>
    /// Builds a dictionary of dialogue assets for fast lookup using the asset's name as the key.
    /// </summary>
    private void BuildDialogueDictionary()
    {
        foreach (var data in dialogueAssets)
        {
            if (data != null && !dialogueDictionary.ContainsKey(data.name))
            {
                dialogueDictionary.Add(data.name, data);
            }
        }
    }

    /// <summary>
    /// Initiates a dialogue sequence by looking up the DialogueData using its key.
    /// Triggers the OnDialogueStarted event with the speaker’s name and initial portrait, then displays the first line.
    /// </summary>
    /// <param name="dialogueKey">The unique key (name) of the DialogueData asset.</param>
    public void StartDialogue(string dialogueKey)
    {
        if (!dialogueDictionary.TryGetValue(dialogueKey, out currentDialogue))
        {
            Debug.LogError($"Dialogue '{dialogueKey}' not found!");
            return;
        }

        currentLineIndex = 0;
        Sprite initialPortrait = GetPortrait(currentLineIndex);
        OnDialogueStarted?.Invoke(currentDialogue.speakerName, initialPortrait);
        ShowNextLine();
    }

    /// <summary>
    /// Displays the next line of dialogue. If all lines have been shown, ends the dialogue.
    /// </summary>
    public void ShowNextLine()
    {
        if (currentDialogue == null)
        {
            Debug.LogWarning("No active dialogue to progress.");
            return;
        }

        if (currentLineIndex < currentDialogue.sentences.Length)
        {
            string line = currentDialogue.sentences[currentLineIndex];
            OnDialogueProgressed?.Invoke(line);
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// Ends the current dialogue sequence and triggers the OnDialogueEnded event.
    /// </summary>
    private void EndDialogue()
    {
        OnDialogueEnded?.Invoke();
        currentDialogue = null;
    }

    /// <summary>
    /// Returns the appropriate portrait for the current dialogue line.
    /// If there are fewer portraits than lines, the last portrait is used.
    /// </summary>
    /// <param name="index">The current line index.</param>
    /// <returns>The portrait sprite for the given index.</returns>
    private Sprite GetPortrait(int index)
    {
        if (currentDialogue != null && currentDialogue.portraits != null && currentDialogue.portraits.Count > 0)
        {
            return index < currentDialogue.portraits.Count ? currentDialogue.portraits[index] : currentDialogue.portraits[currentDialogue.portraits.Count - 1];
        }
        return null;
    }
}
