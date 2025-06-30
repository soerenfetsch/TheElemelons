using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

    /// <summary>
    /// Handles the display and animation of dialogue UI elements, including dialogue box, speaker name, portraits, scene images, and typing effects.
    /// Listens to <see cref="DialogueManager"/> events to update the UI.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("UI References - Dialogue Box")]
        /// <summary>
        /// The panel containing the dialogue box UI.
        /// </summary>
        [SerializeField] private GameObject dialogueBoxPanel;

        /// <summary>
        /// The text field for the speaker's name.
        /// </summary>
        [SerializeField] private TextMeshProUGUI nameText;

        /// <summary>
        /// The image for the speaker's portrait.
        /// </summary>
        [SerializeField] private Image portraitImage;

        /// <summary>
        /// The text field for the dialogue line.
        /// </summary>
        [SerializeField] public TextMeshProUGUI dialogueText;

        /// <summary>
        /// The background image for the dialogue box.
        /// </summary>
        [SerializeField] private Image dialogueBoxBackgroundImage;

        [Header("UI References - Scene Image")]
        /// <summary>
        /// The image used to display scene images during dialogue.
        /// </summary>
        [SerializeField] private Image sceneImageDisplay;

        /// <summary>
        /// The black background for slideshow-style dialogue.
        /// </summary>
        [SerializeField] private GameObject blackBg;

        [Header("Typing Settings")]
        /// <summary>
        /// The base typing speed for the typewriter effect (seconds per character).
        /// </summary>
        [SerializeField] private float baseTypingSpeed = 0.03f;

        /// <summary>
        /// The random offset added to the typing speed for a more natural effect.
        /// </summary>
        [SerializeField] private float randomTypingOffset = 0.01f;

        /// <summary>
        /// The pause after typing a line before advancing.
        /// </summary>
        [SerializeField] private float linePauseAfterTyping = 0.5f;

        [Header("Sound Settings")]
        /// <summary>
        /// The audio source for typing sound effects.
        /// </summary>
        [SerializeField] private AudioSource typingSound;

        // Indicates if the typewriter effect is currently running.
        private bool isTyping;
        // Indicates if the user is skipping the typewriter effect.
        private bool isSkipping;
        // Reference to the currently running typing coroutine.
        private Coroutine typingCoroutine;

        /// <summary>
        /// Registers event handlers and initializes UI state.
        /// </summary>
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

        /// <summary>
        /// Unregisters event handlers.
        /// </summary>
        private void OnDestroy()
        {
            DialogueManager.OnDialogueStart -= HandleDialogueStart;
            DialogueManager.OnDialogueUpdate -= HandleDialogueUpdate;
            DialogueManager.OnDialogueEnded -= HandleDialogueEnded;
        }

        // Handles the start of a new dialogue sequence.
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

        // Handles updates to the dialogue line, including overrides and UI state.
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
            ToggleDialogueOnOff(currentLine.lineSentenceText != null && currentLine.lineSentenceText.Trim() != "", currentLine);
            ToggleImageOnOff(currentLine.lineSceneImage != null, currentLine);
            blackBg.SetActive(DialogueManager.Instance.currentDialogue.isSlideShow);
        }

        // Shows or hides the dialogue box and related UI elements.
        void ToggleDialogueOnOff(bool onOff, DialogueLineEntry currentLine)
        {
            if (dialogueBoxPanel != null) dialogueBoxPanel.SetActive(onOff);
            Debug.Log("dialog box panel" + dialogueBoxPanel.activeSelf.ToString() + dialogueBoxPanel.gameObject.name);
            // Ensure visibility of potentially updated/retained elements
            if (nameText != null) nameText.gameObject.SetActive(!string.IsNullOrEmpty(nameText.text));
            if (portraitImage != null) portraitImage.gameObject.SetActive(portraitImage.sprite != null && onOff);
            if (dialogueBoxBackgroundImage != null) dialogueBoxBackgroundImage.gameObject.SetActive(dialogueBoxBackgroundImage.sprite != null && onOff);
            dialogueText.gameObject.SetActive(onOff);
            if (dialogueText != null)
            {
                if (!string.IsNullOrEmpty(currentLine.lineSentenceText))
                {
                    typingCoroutine = StartCoroutine(TypeLine(currentLine.lineSentenceText));
                }
                else
                {
                    dialogueText.text = "";
                    isTyping = false;
                }
            }
        }

        // Shows or hides the scene image and adjusts its size/position for slideshow or normal mode.
        void ToggleImageOnOff(bool onOff, DialogueLineEntry currentLine)
        {
            sceneImageDisplay.gameObject.SetActive(onOff);
            if (sceneImageDisplay != null)
            {
                if (currentLine.lineSceneImage != null)
                {
                    sceneImageDisplay.sprite = currentLine.lineSceneImage;
                }
                else if (DialogueManager.Instance != null && DialogueManager.Instance.currentDialogue != null
                        && DialogueManager.Instance.currentDialogue.sceneImage != null)
                {
                    // Fallback to global scene image
                    sceneImageDisplay.sprite = DialogueManager.Instance.currentDialogue.sceneImage;
                }
                else
                {
                    sceneImageDisplay.gameObject.SetActive(false);
                    }
            }
            isTyping = false;
            if (DialogueManager.Instance.currentDialogue.isSlideShow)
            {
                sceneImageDisplay.rectTransform.localPosition = new Vector3(0, 0, 0);
                sceneImageDisplay.rectTransform.sizeDelta = new Vector2(1820.448f, 996.5786f);
            }
            else
            {
                sceneImageDisplay.rectTransform.sizeDelta = new Vector2(709.2789f, 523.658f);
                sceneImageDisplay.rectTransform.localPosition = new Vector2(0, 73.055f);
            }
        }

        // Handles the end of a dialogue sequence, hiding all dialogue UI.
        private void HandleDialogueEnded()
        {
            StopExistingTyping();
            if (dialogueBoxPanel != null) dialogueBoxPanel.SetActive(false);
            if (sceneImageDisplay != null) sceneImageDisplay.gameObject.SetActive(false);
            if (blackBg != null) blackBg.SetActive(false);
        }

        // Stops any currently running typing coroutine and resets typing/skipping state.
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

        /// <summary>
        /// Coroutine for typewriter effect: types out the given line character by character.
        /// </summary>
        /// <param name="line">The dialogue line to type out.</param>
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

        /// <summary>
        /// Skips the current typewriter effect or advances to the next dialogue line.
        /// </summary>
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