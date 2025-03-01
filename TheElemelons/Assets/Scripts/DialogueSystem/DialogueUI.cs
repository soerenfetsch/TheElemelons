using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the UI elements for displaying dialogues, including speaker name, portrait, and text.
/// Provides a smoother, more varied typewriter effect.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;  
    [SerializeField] private Image portrait;
    [SerializeField] private Image dialogueBoxImage;     
    [SerializeField] private TextMeshProUGUI textComponent;

    [Header("Typing Settings")]
    [Tooltip("Base delay between letters (in seconds).")]
    [SerializeField] private float baseTypingSpeed = 0.03f;
    [Tooltip("Maximum random offset from baseTypingSpeed (in seconds).")]
    [SerializeField] private float randomTypingOffset = 0.01f;
    [Tooltip("Optional short pause after finishing each sentence.")]
    [SerializeField] private float linePause = 0.5f;

    [Header("Sound Settings")]
    [Tooltip("Optional typing sound played per letter.")]
    [SerializeField] private AudioSource typingSound;

    private bool isSkipping;
    private bool isTyping;

    private void Awake()
    {
        // Hide the UI at the start
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Starts displaying the dialogue using the provided DialogueData.
    /// </summary>
    /// <param name="dialogueData">The data containing speaker name, portrait, and sentences.</param>
    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null)
        {
            Debug.LogWarning("No DialogueData provided to DialogueUI.");
            return;
        }

        // Set speaker name and portrait sprite
        nameText.text = dialogueData.speakerName;
        if (dialogueData.portrait != null && dialogueData.dialogueBoxImage != null)
        {
            dialogueBoxImage.sprite = dialogueData.dialogueBoxImage;
            portrait.sprite = dialogueData.portrait;
            dialogueBoxImage.gameObject.SetActive(true);
            portrait.gameObject.SetActive(true);
        }
        else
        {
            dialogueBoxImage.gameObject.SetActive(false);
            portrait.gameObject.SetActive(false);
        }

        // Clear text and show the UI
        textComponent.text = string.Empty;
        gameObject.SetActive(true);

        // Start typing out all lines
        StartCoroutine(TypeAllLines(dialogueData.sentences));
    }

    /// <summary>
    /// Types out each line in the dialogue one by one, with a short pause between lines.
    /// </summary>
    private IEnumerator TypeAllLines(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            EndDialogue();
            yield break;
        }

        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(line));
            yield return new WaitForSeconds(linePause);
        }

        EndDialogue();
    }

    /// <summary>
    /// Types out a single line character by character, with optional random delay and skipping.
    /// </summary>
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        isSkipping = false;
        textComponent.text = string.Empty;

        foreach (char letter in line)
        {
            // Check if the player pressed a skip key
            if (isSkipping)
            {
                // Instantly fill the rest of the line
                textComponent.text = line;
                break;
            }

            // Add one character
            textComponent.text += letter;

            // Play a small typing sound if assigned
            if (typingSound != null && !typingSound.isPlaying)
            {
                typingSound.Play();
            }

            // Wait for a short delay with random offset
            float randomDelay = baseTypingSpeed + Random.Range(-randomTypingOffset, randomTypingOffset);
            yield return new WaitForSeconds(Mathf.Max(0f, randomDelay));
        }

        isTyping = false;
    }

    /// <summary>
    /// Called externally if you want to skip the current line or speed up typing.
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping)
        {
            isSkipping = true;
        }
    }

    /// <summary>
    /// Ends the current dialogue, hiding the UI.
    /// </summary>
    public void EndDialogue()
    {
        textComponent.text = string.Empty;
        gameObject.SetActive(false);
    }
}
