using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Assets")]
    [SerializeField] private DialogueData[] dialogueAssets;
    private Dictionary<string, DialogueData> dialogueDictionary = new Dictionary<string, DialogueData>();

    private DialogueData currentDialogue;
    private int currentLineIndex = 0;

    // Events for dialogue flow.
    // OnDialogueStarted: speaker name, first portrait, dialogue box image.
    public static event Action<string, Sprite, Sprite> OnDialogueStarted;
    // OnDialogueLineChanged: current line text, current portrait.
    public static event Action<string, Sprite> OnDialogueLineChanged;
    // OnDialogueEnded: dialogue finished.
    public static event Action OnDialogueEnded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally, use DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        BuildDialogueDictionary();
    }

    private void BuildDialogueDictionary()
    {
        foreach (var data in dialogueAssets)
        {
            if (data != null && !dialogueDictionary.ContainsKey(data.name))
            {
                dialogueDictionary.Add(data.name, data);
                Debug.Log($"Added DialogueData with key: {data.name}");
            }
        }
    }

    /// <summary>
    /// Starts the dialogue sequence using the dialogue key.
    /// </summary>
    public void StartDialogue(string dialogueKey)
    {
        if (!dialogueDictionary.TryGetValue(dialogueKey, out currentDialogue))
        {
            Debug.LogError($"Dialogue '{dialogueKey}' not found!");
            return;
        }

        currentLineIndex = 0;
        // Get initial portrait (if available)
        Sprite initialPortrait = GetPortrait(currentLineIndex);
        // Fire event to start dialogue
        OnDialogueStarted?.Invoke(currentDialogue.speakerName, initialPortrait, currentDialogue.dialogueBoxImage);
        // Immediately show first line
        AdvanceDialogue();
    }

    /// <summary>
    /// Advances the dialogue to the next line. If no more lines, ends the dialogue.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (currentDialogue == null)
        {
            Debug.LogWarning("No active dialogue to advance.");
            return;
        }

        if (currentLineIndex < currentDialogue.sentences.Length)
        {
            string line = currentDialogue.sentences[currentLineIndex];
            Sprite currentPortrait = GetPortrait(currentLineIndex);
            OnDialogueLineChanged?.Invoke(line, currentPortrait);
            currentLineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// Ends the dialogue sequence.
    /// </summary>
    private void EndDialogue()
    {
        OnDialogueEnded?.Invoke();
        currentDialogue = null;
    }

    private Sprite GetPortrait(int index)
    {
        if (currentDialogue != null && currentDialogue.portraits != null && currentDialogue.portraits.Count > 0)
        {
            return index < currentDialogue.portraits.Count ? currentDialogue.portraits[index] : currentDialogue.portraits[currentDialogue.portraits.Count - 1];
        }
        return null;
    }
}
