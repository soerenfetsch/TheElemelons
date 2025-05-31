using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Assets")]
    [SerializeField] private bool autoPlay = false;
    [SerializeField] private float autoPlayDelay = 3f;    
    public DialogueData currentDialogue;
    public int currentLineIndex = 0;
    private Coroutine autoPlayCoroutine;

    public static event Action<DialogueData> OnDialogueStart;
    public static event Action<DialogueLineEntry> OnDialogueUpdate;
    public static event Action OnDialogueEnded;

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

    public bool HasDialogue()
    {
        return currentDialogue != null;
    }

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
    /// Ends the dialogue sequence.
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
        
        if (GameManager.Instance != null) // ----------------------- change to previous status -> have to update gamemanager for that
        {
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }
        else
        {
            Debug.LogWarning("[DialogueManager] GameManager.Instance is NULL in EndDialogue. Cannot change game status.");
        }
    }

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