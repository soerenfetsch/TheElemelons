using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the UI elements for displaying dialogues, including speaker name, portrait, and text.
/// Provides a typewriter effect and updates the speaker's portrait according to the dialogue progression.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    #region Serialized Fields

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
    [Tooltip("Optional short pause after finishing each sentence (in seconds).")]
    [SerializeField] private float linePause = 0.5f;

    [Header("Sound Settings")]
    [Tooltip("Optional typing sound played per letter.")]
    [SerializeField] private AudioSource typingSound;

    #endregion

    #region Private Fields

    private bool isSkipping;
    private bool isTyping;

    // Optionally, store the current dialogue data if needed for more advanced logic.
    private DialogueData currentDialogueData;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        // Hide the dialogue UI at the start.
        gameObject.SetActive(false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts displaying the dialogue using the provided DialogueData.
    /// </summary>
    /// <param name="dialogueData">The DialogueData containing speaker name, list of portraits, and sentences.</param>
    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null)
        {
            Debug.LogWarning("No DialogueData provided to DialogueUI.");
            return;
        }

        currentDialogueData = dialogueData;

        // Set the speaker name.
        nameText.text = dialogueData.speakerName;

        // Set the dialogue box image and default portrait if available.
        if (dialogueData.dialogueBoxImage != null)
        {
            dialogueBoxImage.sprite = dialogueData.dialogueBoxImage;
            dialogueBoxImage.gameObject.SetActive(true);
        }
        else
        {
            dialogueBoxImage.gameObject.SetActive(false);
        }

        // If there is at least one portrait, set the first one.
        if (dialogueData.portraits != null && dialogueData.portraits.Count > 0)
        {
            portrait.sprite = dialogueData.portraits[0];
            portrait.gameObject.SetActive(true);
        }
        else
        {
            portrait.gameObject.SetActive(false);
        }

        // Clear text and show the dialogue UI.
        textComponent.text = string.Empty;
        gameObject.SetActive(true);

        // Start typing out all dialogue lines.
        StartCoroutine(TypeAllLines(dialogueData.sentences, dialogueData));
    }

    /// <summary>
    /// Ends the dialogue by hiding the dialogue UI.
    /// </summary>
    public void EndDialogue()
    {
        textComponent.text = string.Empty;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called externally to skip the current line or accelerate the typewriter effect.
    /// </summary>
    public void SkipTyping()
    {
        if (isTyping)
        {
            isSkipping = true;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Types out each line of the dialogue one by one, updating the portrait for each line.
    /// </summary>
    /// <param name="lines">Array of dialogue sentences.</param>
    /// <param name="dialogueData">The DialogueData used for the dialogue.</param>
    private IEnumerator TypeAllLines(string[] lines, DialogueData dialogueData)
    {
        if (lines == null || lines.Length == 0)
        {
            EndDialogue();
            yield break;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            // Update the portrait based on the current sentence index.
            // If there are more sentences than portraits, use the last portrait.
            if (dialogueData.portraits != null && dialogueData.portraits.Count > 0)
            {
                int portraitIndex = i < dialogueData.portraits.Count ? i : dialogueData.portraits.Count - 1;
                portrait.sprite = dialogueData.portraits[portraitIndex];
            }

            yield return StartCoroutine(TypeLine(lines[i]));
            yield return new WaitForSecondsRealtime(linePause);
        }

        EndDialogue();
    }

    /// <summary>
    /// Types out a single line of dialogue character by character, with a small random delay.
    /// </summary>
    /// <param name="line">The dialogue line to type out.</param>
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        isSkipping = false;
        textComponent.text = string.Empty;

        foreach (char letter in line)
        {
            // If skipping is triggered, immediately display the full line.
            if (isSkipping)
            {
                textComponent.text = line;
                break;
            }

            textComponent.text += letter;

            // Play typing sound if assigned.
            if (typingSound != null && !typingSound.isPlaying)
            {
                typingSound.Play();
            }

            // Wait for a short delay with a random offset (using WaitForSeconds, assuming timeScale is not 0).
            float randomDelay = baseTypingSpeed + Random.Range(-randomTypingOffset, randomTypingOffset);
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, randomDelay));
        }

        isTyping = false;
    }

    #endregion
}
