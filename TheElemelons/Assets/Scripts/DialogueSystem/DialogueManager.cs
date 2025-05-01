using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Assets")]
    [SerializeField] public DialogueData[] dialogueAssets;
    [SerializeField] private bool autoPlay = false;
    [SerializeField] private float autoPlayDelay = 3f; // Delay (in seconds) before auto-advancing a line
    private Dictionary<string, DialogueData> dialogueDictionary = new Dictionary<string, DialogueData>();
    public DialogueData currentDialogue;
    public int currentLineIndex = 0;
    private Coroutine autoPlayCoroutine;

    // Events for dialogue flow.
    // OnDialogueStarted: speaker name, initial portrait, dialogue box image.
    public static event Action<string, Sprite, Sprite> OnDialogueStarted;
    // OnDialogueLineChanged: current line text, current portrait.
    public static event Action<string, Sprite> OnDialogueLineChanged;
    // OnDialogueEnded: dialogue finished.
    public static event Action OnDialogueEnded;

    public GameObject imageObject;

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

    public void BuildDialogueDictionary()
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
    /// Starts the dialogue sequence using the provided dialogue key.
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

    public Boolean HasDialogue()
    {
        if (currentDialogue == null)
        { return false; }
        return true;
    }

    /// <summary>
    /// Advances the dialogue to the next line. If no more lines remain, ends the dialogue.
    /// </summary>
    public void AdvanceDialogue()
    {
        // Cancel any existing auto-play coroutine.
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

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

            // Schedule auto-advance if enabled.
            if (autoPlay)
            {
                autoPlayCoroutine = StartCoroutine(AutoAdvanceAfterDelay(autoPlayDelay));
            }
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
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }
        OnDialogueEnded?.Invoke();

        if (currentDialogue.imageSprite != null && !imageObject.activeSelf)
        {
            imageObject.SetActive(true);
            imageObject.GetComponent<Image>().sprite = currentDialogue.imageSprite;
        }
        else
        {
            imageObject.SetActive(false);
            currentDialogue = null;
        }

    }

    private Sprite GetPortrait(int index)
    {
        if (currentDialogue != null && currentDialogue.portraits != null && currentDialogue.portraits.Count > 0)
        {
            return index < currentDialogue.portraits.Count ? currentDialogue.portraits[index] : currentDialogue.portraits[currentDialogue.portraits.Count - 1];
        }
        return null;
    }

    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        // Advance dialogue automatically if still in progress.
        if (currentDialogue != null && currentLineIndex < currentDialogue.sentences.Length)
        {
            AdvanceDialogue();
        }
    }
}
