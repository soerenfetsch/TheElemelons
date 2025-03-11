using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages dialogue interactions in a centralized way.
/// Stores multiple DialogueData assets in a dictionary for fast lookup and communicates
/// with the DialogueUI to display dialogue. Also interacts with the GameManager to pause gameplay during dialogue.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the DialogueManager.
    /// </summary>
    public static DialogueManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Dialogue Assets")]
    [SerializeField] private DialogueData[] dialogueAssets; 
    // Drag your DialogueData assets (e.g., "Dialogue_King-Introduction") here in the Inspector.

    // Internal dictionary for quick dialogue lookup.
    private Dictionary<string, DialogueData> dialogueDictionary = new Dictionary<string, DialogueData>();

    // Event that signals dialogue completion.
    public event Action OnDialogueComplete;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Sets up the singleton instance and builds the dialogue dictionary.
    /// </summary>
    private void Awake()
    {
        // Singleton pattern implementation.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keep it across scenes.
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        BuildDialogueDictionary();
    }

    /// <summary>
    /// Builds the dialogue dictionary from the serialized array of DialogueData assets.
    /// Each asset's name is used as the key.
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
    /// Starts a dialogue sequence by looking up the DialogueData using its key (the asset's name)
    /// and instructing the DialogueUI to display it. Also notifies the GameManager to pause the game.
    /// </summary>
    /// <param name="dialogueKey">The unique key (name) of the DialogueData asset to display.</param>
    public void StartDialogue(string dialogueKey)
    {
        if (!dialogueDictionary.ContainsKey(dialogueKey))
        {
            Debug.LogError($"Dialogue '{dialogueKey}' not found in the dictionary!");
            return;
        }
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI is not assigned!");
            return;
        }

        dialogueUI.StartDialogue(dialogueDictionary[dialogueKey]);
    }

    /// <summary>
    /// Ends the active dialogue sequence by instructing the DialogueUI to hide the dialogue UI.
    /// Also notifies the GameManager to resume gameplay.
    /// </summary>
    public void EndDialogue()
    {
        if (dialogueUI == null)
        {
            Debug.LogError("DialogueUI is not assigned!");
            return;
        }
        dialogueUI.EndDialogue();
        OnDialogueComplete?.Invoke();
    }

    public void SkipTyping()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SkipTyping();
        }
    }

}