using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBoxImage;

    [Header("Typing Settings")]
    [SerializeField] private float baseTypingSpeed = 0.03f;
    [SerializeField] private float randomTypingOffset = 0.01f;
    [SerializeField] private float linePause = 0.5f;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource typingSound;

    private bool isTyping;
    private bool isSkipping;

    private void Awake()
    {
        // Subscribe to dialogue events (using Awake so subscriptions occur even if inactive).
        DialogueManager.OnDialogueStarted += ShowDialogue;
        DialogueManager.OnDialogueLineChanged += ShowNextLine;
        DialogueManager.OnDialogueEnded += HideDialogue;

        // Ensure the UI is disabled by default.
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueStarted -= ShowDialogue;
        DialogueManager.OnDialogueLineChanged -= ShowNextLine;
        DialogueManager.OnDialogueEnded -= HideDialogue;
    }

    /// <summary>
    /// Called when dialogue starts. Activates the UI and sets speaker name, initial portrait, and dialogue box image.
    /// </summary>
    private void ShowDialogue(string speakerName, Sprite initialPortrait, Sprite boxImage)
    {
        gameObject.SetActive(true);
        nameText.text = speakerName;
        
        if (initialPortrait != null)
        {
            portrait.sprite = initialPortrait;
            portrait.gameObject.SetActive(true);
        }
        else
        {
            portrait.gameObject.SetActive(false);
        }
        
        if (boxImage != null)
        {
            dialogueBoxImage.sprite = boxImage;
            dialogueBoxImage.gameObject.SetActive(true);
        }
        else
        {
            dialogueBoxImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when a new dialogue line is ready. Stops previous typing and starts the typewriter effect.
    /// </summary>
    private void ShowNextLine(string line, Sprite currentPortrait)
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine(line, currentPortrait));
    }

    private IEnumerator TypeLine(string line, Sprite currentPortrait)
    {
        isTyping = true;
        isSkipping = false;
        dialogueText.text = "";

        // Update portrait for this line.
        if (currentPortrait != null)
        {
            portrait.sprite = currentPortrait;
            portrait.gameObject.SetActive(true);
        }

        foreach (char letter in line)
        {
            if (isSkipping)
            {
                dialogueText.text = line;
                break;
            }

            dialogueText.text += letter;
            if (typingSound != null && !typingSound.isPlaying)
            {
                typingSound.Play();
            }
            float delay = baseTypingSpeed + Random.Range(-randomTypingOffset, randomTypingOffset);
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, delay));
        }

        isTyping = false;
        yield return new WaitForSecondsRealtime(linePause);
    }

    /// <summary>
    /// Hides the dialogue UI when dialogue ends.
    /// </summary>
    private void HideDialogue()
    {
        dialogueText.text = "";
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called externally (via input) to skip the current line's typewriter effect,
    /// or if the line is fully displayed, to request the DialogueManager to advance.
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping)
        {
            isSkipping = true;
        }
        else
        {
            DialogueManager.Instance.AdvanceDialogue();
        }
    }
}
