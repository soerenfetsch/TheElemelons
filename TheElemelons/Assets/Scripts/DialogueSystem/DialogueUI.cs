using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles the UI for displaying dialogue including speaker name, portrait, and typewriter effect for dialogue text.
/// This class listens to events from DialogueManager to update the dialogue content.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBoxImage;

    [Header("Typing Settings")]
    [Tooltip("Base delay between letters (in seconds).")]
    [SerializeField] private float baseTypingSpeed = 0.03f;
    [Tooltip("Random offset to add variation to typing speed (in seconds).")]
    [SerializeField] private float randomTypingOffset = 0.01f;
    [Tooltip("Pause after each line is typed (in seconds).")]
    [SerializeField] private float linePause = 0.5f;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource typingSound;

    private bool isTyping;
    private bool isSkipping;

    private void OnEnable()
    {
        // Subscribe to dialogue events from DialogueManager.
        DialogueManager.OnDialogueStarted += ShowDialogue;
        DialogueManager.OnDialogueProgressed += ShowNextLine;
        DialogueManager.OnDialogueEnded += HideDialogue;
    }

    private void OnDisable()
    {
        // Unsubscribe from dialogue events.
        DialogueManager.OnDialogueStarted -= ShowDialogue;
        DialogueManager.OnDialogueProgressed -= ShowNextLine;
        DialogueManager.OnDialogueEnded -= HideDialogue;
    }

    /// <summary>
    /// Called when a dialogue starts. Sets the speaker name and portrait, then shows the dialogue UI.
    /// </summary>
    /// <param name="speakerName">The name of the dialogue speaker.</param>
    /// <param name="speakerPortrait">The portrait image for the speaker.</param>
    private void ShowDialogue(string speakerName, Sprite speakerPortrait)
    {
        gameObject.SetActive(true);
        nameText.text = speakerName;
        if (speakerPortrait != null)
        {
            portrait.sprite = speakerPortrait;
            portrait.gameObject.SetActive(true);
        }
        else
        {
            portrait.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when a new line should be displayed. Stops any previous typewriter coroutine and starts a new one.
    /// </summary>
    /// <param name="line">The dialogue line to display.</param>
    private void ShowNextLine(string line)
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine(line));
    }

    /// <summary>
    /// Hides the dialogue UI.
    /// </summary>
    private void HideDialogue()
    {
        dialogueText.text = string.Empty;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Types out the provided line letter by letter with a typewriter effect.
    /// Allows the player to skip the effect.
    /// </summary>
    /// <param name="line">The dialogue line to type out.</param>
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        isSkipping = false;
        dialogueText.text = "";

        foreach (char letter in line)
        {
            if (isSkipping)
            {
                dialogueText.text = line;
                break;
            }

            dialogueText.text += letter;

            // Play typing sound if available.
            if (typingSound != null && !typingSound.isPlaying)
            {
                typingSound.Play();
            }

            float delay = baseTypingSpeed + Random.Range(-randomTypingOffset, randomTypingOffset);
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, delay));
        }

        isTyping = false;
        // Optional: wait after the line is fully displayed before the next action.
        yield return new WaitForSecondsRealtime(linePause);
    }

    /// <summary>
    /// Allows external calls (e.g., from an input manager) to skip the current typewriter effect.
    /// If typing is in progress, it immediately shows the full line; otherwise, it signals to show the next line.
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping)
        {
            isSkipping = true;
        }
        else
        {
            DialogueManager.Instance.ShowNextLine();
        }
    }
}
