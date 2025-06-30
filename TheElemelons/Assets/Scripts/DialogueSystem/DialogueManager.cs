using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the flow of dialogue sequences, including starting, advancing, and ending dialogues.
/// Handles broadcasting dialogue events to the UI and other systems.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the DialogueManager.
    /// </summary>
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Assets")]
    /// <summary>
    /// If true, dialogue will auto-advance after a delay.
    /// </summary>
    [SerializeField] private bool autoPlay = false;

    /// <summary>
    /// Delay (in seconds) before auto-advancing to the next dialogue line.
    /// </summary>
    [SerializeField] private float autoPlayDelay = 5f;

    /// <summary>
    /// The currently active dialogue data.
    /// </summary>
    public DialogueData currentDialogue;

    /// <summary>
    /// The index of the current line in the dialogue.
    /// </summary>
    public int currentLineIndex = 0;

    // Reference to the currently running auto-play coroutine.
    private Coroutine autoPlayCoroutine;

    /// <summary>
    /// Event fired when a new dialogue sequence starts.
    /// </summary>
    public static event Action<DialogueData> OnDialogueStart;

    /// <summary>
    /// Event fired when the current dialogue line changes.
    /// </summary>
    public static event Action<DialogueLineEntry> OnDialogueUpdate;

    /// <summary>
    /// Event fired when the dialogue sequence ends.
    /// </summary>
    public static event Action OnDialogueEnded;

    // Unity Awake callback. Sets up the singleton instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Starts a new dialogue sequence.
    /// </summary>
    /// <param name="dialogueToStart">The DialogueData asset to start.</param>
    public void StartDialogue(DialogueData dialogueToStart)
    {
        if (dialogueToStart == null || dialogueToStart.dialogueLines == null || dialogueToStart.dialogueLines.Count == 0)
        {
            Debug.LogError("[DialogueManager] Cannot start dialogue: DialogueData is null, has no lines, or lines list is null.");
            return;
        }

        currentDialogue = dialogueToStart;
        currentLineIndex = -1; // Reset for the new dialogue

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.DIALOGUE);
        }
        else
        {
            Debug.LogWarning("[DialogueManager] GameManager.Instance is NULL. Cannot change game status.");
        }

        OnDialogueStart?.Invoke(currentDialogue);
    }

    /// <summary>
    /// Returns true if a dialogue is currently active.
    /// </summary>
    public bool HasDialogue()
    {
        return currentDialogue != null;
    }

    /// <summary>
    /// Advances to the next line in the current dialogue sequence.
    /// Ends the dialogue if there are no more lines.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (currentDialogue == null) return;

        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        currentLineIndex++;

        if (currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            ProcessCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    // Processes the current dialogue line, invokes update events, and handles auto-play if enabled.
    private void ProcessCurrentLine()
    {
        DialogueLineEntry currentEntry = currentDialogue.dialogueLines[currentLineIndex];
        if (currentEntry == null)
        {
            Debug.LogWarning($"[DialogueManager] Null entry at line {currentLineIndex} in '{currentDialogue.name}'. Advancing again.");
            AdvanceDialogue(); // Skip null entries
            return;
        }
        OnDialogueUpdate?.Invoke(currentEntry);

        if (autoPlay)
        {
            float delay = autoPlayDelay;
            // Potentially add logic here if different line types need different auto-advance delays
            autoPlayCoroutine = StartCoroutine(AutoAdvanceOrEndAfterDelay(delay));
        }
    }

    /// <summary>
    /// Ends the dialogue sequence and resets state.
    /// </summary>
    private void EndDialogue()
    {
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        OnDialogueEnded?.Invoke();
        currentDialogue = null;
        currentLineIndex = -1; // Reset line index for next dialogue
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }
        else
        {
            Debug.LogWarning("[DialogueManager] GameManager.Instance is NULL in EndDialogue. Cannot change game status.");
        }
    }

    // Coroutine to auto-advance or end the dialogue after a delay.
    private IEnumerator AutoAdvanceOrEndAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count - 1)
        {
            AdvanceDialogue();
        }
        else
        {
            EndDialogue();
        }
    }
}