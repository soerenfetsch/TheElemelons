using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References - Dialogue Box")]
    [SerializeField] private GameObject dialogueBoxPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image portraitImage;
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueBoxBackgroundImage;

    [Header("UI References - Scene Image")]
    [SerializeField] private Image sceneImageDisplay;

    [Header("Typing Settings")]
    [SerializeField] private float baseTypingSpeed = 0.03f;
    [SerializeField] private float randomTypingOffset = 0.01f;
    [SerializeField] private float linePauseAfterTyping = 0.5f;

    [Header("Sound Settings")]
    [SerializeField] private AudioSource typingSound;

    private bool isTyping;
    private bool isSkipping;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        DialogueManager.OnDialogueStart += HandleDialogueStart;
        DialogueManager.OnDialogueUpdate += HandleDialogueUpdate;
        DialogueManager.OnDialogueEnded += HandleDialogueEnded;

        // Initial state
        gameObject.SetActive(true);
        if (dialogueBoxPanel != null) dialogueBoxPanel.SetActive(false);
        if (sceneImageDisplay != null) sceneImageDisplay.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueStart -= HandleDialogueStart;
        DialogueManager.OnDialogueUpdate -= HandleDialogueUpdate;
        DialogueManager.OnDialogueEnded -= HandleDialogueEnded;
    }

    private void HandleDialogueStart(DialogueData dialogueData)
    {
        // Set global/default UI elements from DialogueData
        if (nameText != null)
        {
            nameText.text = dialogueData.speakerName;
            // Visibility of nameText will be handled by HandleDialogueUpdate based on line type
        }
        if (portraitImage != null)
        {
            portraitImage.sprite = dialogueData.portraitImage;
            // Visibility of portraitImage will be handled by HandleDialogueUpdate
        }
        if (dialogueBoxBackgroundImage != null)
        {
            dialogueBoxBackgroundImage.sprite = dialogueData.dialogueBoxImage;
            // Visibility of dialogueBoxBackgroundImage will be handled by HandleDialogueUpdate
        }
        if (sceneImageDisplay != null)
        {
            sceneImageDisplay.sprite = dialogueData.sceneImage;
            // Visibility of sceneImageDisplay will be handled by HandleDialogueUpdate
        }

        // Ensure panels are in a known state before the first line update
        if (dialogueBoxPanel != null) dialogueBoxPanel.SetActive(false);
        if (sceneImageDisplay != null) sceneImageDisplay.gameObject.SetActive(false);

        // Tell DialogueManager to process and send the first line's data
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AdvanceDialogue();
        }
        else
        {
            Debug.LogError("[DialogueUI] DialogueManager.Instance is null in HandleDialogueStart.");
        }
    }

    private void HandleDialogueUpdate(DialogueLineEntry currentLine)
    {
        StopExistingTyping();

        // Apply overrides from currentLine if they exist
        // If an override is not present, the UI element retains its previous state (set by HandleDialogueStart or a prior HandleDialogueUpdate)

        // Dialogue Box Image Override
        if (currentLine.overrideDialogueBoxImage != null)
        {
            if (dialogueBoxBackgroundImage != null)
            {
                dialogueBoxBackgroundImage.sprite = currentLine.overrideDialogueBoxImage;
            }
        }
        else if (DialogueManager.Instance != null && DialogueManager.Instance.currentDialogue != null)
        {
            // fallback to global dialogue box image
            dialogueBoxBackgroundImage.sprite = DialogueManager.Instance.currentDialogue.dialogueBoxImage;
        }

        // Portrait Override
        if (currentLine.linePortrait != null)
        {
            if (portraitImage != null)
            {
                portraitImage.sprite = currentLine.linePortrait;
            }
        }
        else if (DialogueManager.Instance != null && DialogueManager.Instance.currentDialogue != null)
        {
            // fallback to global portrait
            portraitImage.sprite = DialogueManager.Instance.currentDialogue.portraitImage;
        }
        if (sceneImageDisplay != null)
        {
            sceneImageDisplay.sprite = currentLine.lineSceneImage; // Scene image is set directly from the line entry
        }
        else if (DialogueManager.Instance != null && DialogueManager.Instance.currentDialogue != null)
        {
            // fallback to global scene image
            sceneImageDisplay.sprite = DialogueManager.Instance.currentDialogue.sceneImage;
        }

        // Handle display based on LineType
        if (currentLine.lineType == LineType.Text)
        {
            if (dialogueBoxPanel != null) dialogueBoxPanel.SetActive(true);
            if (sceneImageDisplay != null) sceneImageDisplay.gameObject.SetActive(false);

            // Ensure visibility of potentially updated/retained elements
            if (nameText != null) nameText.gameObject.SetActive(!string.IsNullOrEmpty(nameText.text));
            if (portraitImage != null) portraitImage.gameObject.SetActive(portraitImage.sprite != null);
            if (dialogueBoxBackgroundImage != null) dialogueBoxBackgroundImage.gameObject.SetActive(dialogueBoxBackgroundImage.sprite != null);

            if (dialogueText != null)
            {
                if (!string.IsNullOrEmpty(currentLine.lineSentenceText))
                {
                    dialogueText.gameObject.SetActive(true);
                    typingCoroutine = StartCoroutine(TypeLine(currentLine.lineSentenceText));
                }
                else
                {
                    dialogueText.text = "";
                    dialogueText.gameObject.SetActive(false);
                    isTyping = false;
                }
            }
        }
        else if (currentLine.lineType == LineType.SceneImage)
        {
            if (dialogueBoxPanel != null) dialogueBoxPanel.SetActive(false);
            if (sceneImageDisplay != null)
            {
                if (currentLine.lineSceneImage != null)
                {
                    sceneImageDisplay.sprite = currentLine.lineSceneImage;
                    sceneImageDisplay.gameObject.SetActive(true);
                }
                else if (DialogueManager.Instance != null && DialogueManager.Instance.currentDialogue != null
                        && DialogueManager.Instance.currentDialogue.sceneImage != null)
                {
                    // Fallback to global scene image
                    sceneImageDisplay.sprite = DialogueManager.Instance.currentDialogue.sceneImage;
                    sceneImageDisplay.gameObject.SetActive(true);
                }
                else
                {
                    sceneImageDisplay.gameObject.SetActive(false);
                    Debug.LogWarning("[DialogueUI] SceneImage type line has no scene images or first image is null.");
                }
            }
            isTyping = false;
        }
    }

    private void HandleDialogueEnded()
    {
        StopExistingTyping();
        if (dialogueBoxPanel != null) dialogueBoxPanel.SetActive(false);
        if (sceneImageDisplay != null) sceneImageDisplay.gameObject.SetActive(false);
    }

    private void StopExistingTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null; // Clear the reference
            isTyping = false;
        }
        isSkipping = false;
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        isSkipping = false;
        if (dialogueText != null) dialogueText.text = ""; else { Debug.LogError("[DialogueUI] TypeLine: dialogueText is NULL!"); yield break; }


        foreach (char letter in line)
        {
            if (isSkipping)
            {
                if (dialogueText != null) dialogueText.text = line;
                break;
            }

            if (dialogueText != null) dialogueText.text += letter;
            if (typingSound != null && letter != ' ')
            {
                // typingSound.Play(); // Play can be spammy, consider PlayOneShot or a custom handler
            }
            float delay = baseTypingSpeed + UnityEngine.Random.Range(-randomTypingOffset, randomTypingOffset);
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, delay));
        }

        isTyping = false;
        // isSkipping = false; // Resetting skip flag here might be too early if player holds skip. Usually reset on new line.
        yield return new WaitForSecondsRealtime(linePauseAfterTyping);
    }

    public void SkipOrAdvance() // -------------- move this functionality to DialogueManager? note: this might be deprecated if DialogueManager handles skipping/advancing
    {
        if (isTyping && !isSkipping)
        {
            isSkipping = true;
        }
        else if (!isTyping) 
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.HasDialogue())
            {
                 DialogueManager.Instance.AdvanceDialogue();
            }
            else
            {
                Debug.LogWarning("[DialogueUI] SkipOrAdvance: DialogueManager.Instance is null or has no active dialogue.");
            }
        }
         else
        {
             Debug.Log("[DialogueUI] SkipOrAdvance: Currently typing AND already skipping, or some other state. No action.");
        }
    }
}