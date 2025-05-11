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
        Debug.Log($"[DialogueUI] Awake on GameObject: {this.name}");
        DialogueManager.OnDisplayUpdateRequest += HandleDisplayUpdate;
        DialogueManager.OnDialogueEnded += HideDialogueOverall;
        Debug.Log("[DialogueUI] Subscribed to DialogueManager events.");

        // Ensure the UI elements are in a known state initially.
        if (dialogueBoxPanel != null) 
        {
            Debug.Log($"[DialogueUI] Initializing dialogueBoxPanel '{dialogueBoxPanel.name}' to inactive.");
            dialogueBoxPanel.SetActive(false);
        } else { Debug.LogWarning("[DialogueUI] dialogueBoxPanel is NOT assigned in Awake!");}

        if (sceneImageDisplay != null) 
        {
            Debug.Log($"[DialogueUI] Initializing sceneImageDisplay '{sceneImageDisplay.name}' GameObject to inactive.");
            sceneImageDisplay.gameObject.SetActive(false);
        } else { Debug.LogWarning("[DialogueUI] sceneImageDisplay is NOT assigned in Awake!");}
        
        Debug.Log($"[DialogueUI] Initializing DialogueUI GameObject '{this.name}' to inactive.");
        gameObject.SetActive(false);
        Debug.Log("[DialogueUI] Awake: Initial UI elements hidden. DialogueUI GameObject deactivated.");
    }

    private void OnDestroy()
    {
        Debug.Log($"[DialogueUI] OnDestroy on GameObject: {this.name}");
        DialogueManager.OnDisplayUpdateRequest -= HandleDisplayUpdate;
        DialogueManager.OnDialogueEnded -= HideDialogueOverall;
        Debug.Log("[DialogueUI] Unsubscribed from DialogueManager events.");
    }

    private void HandleDisplayUpdate(DialogueLineDisplayInfo info)
    {
        string portraitName = info.Portrait != null ? info.Portrait.name : "None";
        string sceneImageName = info.SceneImage != null ? info.SceneImage.name : "None";
        string dialogueBoxImageName = info.DialogueBoxImage != null ? info.DialogueBoxImage.name : "None";

        Debug.Log($"[DialogueUI] HandleDisplayUpdate RECEIVED. Type: {info.Type}, Text: \"{info.Text}\", Portrait: {portraitName}, SceneImg: {sceneImageName}, Speaker: {info.SpeakerName}, DB Img: {dialogueBoxImageName}, FirstLine: {info.IsFirstLineInDialogue}");

        Debug.Log($"[DialogueUI] Activating DialogueUI GameObject '{this.name}'. Current activeSelf: {gameObject.activeSelf}");
        gameObject.SetActive(true);
        Debug.Log($"[DialogueUI] DialogueUI GameObject '{this.name}' activeSelf is now: {gameObject.activeSelf}");


        if (typingCoroutine != null)
        {
            Debug.Log("[DialogueUI] Stopping existing typing coroutine.");
            StopCoroutine(typingCoroutine);
            typingCoroutine = null; // Clear the reference
            isTyping = false;
        }
        isSkipping = false;

        Debug.Log($"[DialogueUI] Updating Name: {info.SpeakerName}. nameText null? {nameText == null}");
        if (nameText != null)
        {
            if (info.IsFirstLineInDialogue || nameText.text != info.SpeakerName)
            {
                 nameText.text = info.SpeakerName;
                 Debug.Log($"[DialogueUI] Set nameText to: {info.SpeakerName}");
            }
        } else { Debug.LogWarning("[DialogueUI] nameText is NULL!");}
        
        Debug.Log($"[DialogueUI] Updating DialogueBoxBG. dialogueBoxBackgroundImage null? {dialogueBoxBackgroundImage == null}. info.DialogueBoxImage null? {info.DialogueBoxImage == null}.");
        if (dialogueBoxBackgroundImage != null)
        {
            if (info.DialogueBoxImage != null)
            {
                dialogueBoxBackgroundImage.sprite = info.DialogueBoxImage;
                bool targetActiveState = info.Type == LineType.Text;
                Debug.Log($"[DialogueUI] Set dialogueBoxBackgroundImage sprite. Setting active: {targetActiveState}");
                dialogueBoxBackgroundImage.gameObject.SetActive(targetActiveState);
            }
            else
            {
                Debug.Log("[DialogueUI] info.DialogueBoxImage is NULL. Setting dialogueBoxBackgroundImage active: false");
                dialogueBoxBackgroundImage.gameObject.SetActive(false);
            }
        } else { Debug.LogWarning("[DialogueUI] dialogueBoxBackgroundImage is NULL!");}


        if (info.Type == LineType.SceneImage)
        {
            Debug.Log($"[DialogueUI] --- Scene Image Path --- dialogueBoxPanel null? {dialogueBoxPanel == null}. sceneImageDisplay null? {sceneImageDisplay == null}");
            if (dialogueBoxPanel != null)
            {
                Debug.Log($"[DialogueUI] Setting dialogueBoxPanel '{dialogueBoxPanel.name}' active: false");
                dialogueBoxPanel.SetActive(false);
            } else { Debug.LogWarning("[DialogueUI] dialogueBoxPanel is NULL for Scene Image Path!");}
            
            if (sceneImageDisplay != null)
            {
                if (info.SceneImage != null)
                {
                    Debug.Log($"[DialogueUI] Setting sceneImageDisplay.sprite to {info.SceneImage.name}. Setting sceneImageDisplay.gameObject '{sceneImageDisplay.name}' active: true");
                    sceneImageDisplay.sprite = info.SceneImage;
                    sceneImageDisplay.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"[DialogueUI] info.SceneImage is NULL for Scene Image Path. Setting sceneImageDisplay.gameObject '{sceneImageDisplay.name}' active: false");
                    sceneImageDisplay.gameObject.SetActive(false);
                }
            } else { Debug.LogWarning("[DialogueUI] sceneImageDisplay is NULL for Scene Image Path!");}
        }
        else // LineType.Text
        {
            Debug.Log($"[DialogueUI] --- Text Path --- sceneImageDisplay null? {sceneImageDisplay == null}. dialogueBoxPanel null? {dialogueBoxPanel == null}. nameText null? {nameText == null}. dialogueText null? {dialogueText == null}. portraitImage null? {portraitImage == null}");
            if (sceneImageDisplay != null)
            {
                Debug.Log($"[DialogueUI] Setting sceneImageDisplay.gameObject '{sceneImageDisplay.name}' active: false");
                sceneImageDisplay.gameObject.SetActive(false);
            } else { Debug.LogWarning("[DialogueUI] sceneImageDisplay is NULL for Text Path!");}

            if (dialogueBoxPanel != null)
            {
                Debug.Log($"[DialogueUI] Setting dialogueBoxPanel '{dialogueBoxPanel.name}' active: true");
                dialogueBoxPanel.SetActive(true);
            } else { Debug.LogWarning("[DialogueUI] dialogueBoxPanel is NULL for Text Path!");}

            if (nameText != null) {
                Debug.Log($"[DialogueUI] Setting nameText '{nameText.name}' active: true");
                nameText.gameObject.SetActive(true);
            } // Name text already set above

            if (dialogueText != null) {
                dialogueText.text = ""; // Clear previous text
            } else { Debug.LogWarning("[DialogueUI] dialogueText is NULL for Text Path!");}


            if (portraitImage != null)
            {
                if (info.Portrait != null)
                {
                    portraitImage.sprite = info.Portrait;
                    Debug.Log($"[DialogueUI] Portrait: {info.Portrait.name}. Setting portraitImage.gameObject '{portraitImage.name}' active: true");
                    portraitImage.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log($"[DialogueUI] No Portrait for this line. Setting portraitImage.gameObject '{portraitImage.name}' active: false");
                    portraitImage.gameObject.SetActive(false);
                }
            } else { Debug.LogWarning("[DialogueUI] portraitImage is NULL for Text Path!");}
            
            if (dialogueText != null && !string.IsNullOrEmpty(info.Text))
            {
                 Debug.Log($"[DialogueUI] Starting TypeLine coroutine with text: \"{info.Text}\"");
                 typingCoroutine = StartCoroutine(TypeLine(info.Text));
            }
            else if (dialogueText != null) 
            {
                Debug.Log($"[DialogueUI] Text is null or empty for TypeLine. Text: \"{info.Text}\". Setting text to empty, isTyping=false.");
                dialogueText.text = "";
                isTyping = false; 
            }
        }
        Debug.Log("[DialogueUI] HandleDisplayUpdate FINISHED.");
    }

    private IEnumerator TypeLine(string line)
    {
        Debug.Log($"[DialogueUI] TypeLine coroutine START for line: \"{line}\"");
        isTyping = true;
        isSkipping = false;
        if (dialogueText != null) dialogueText.text = ""; else { Debug.LogError("[DialogueUI] TypeLine: dialogueText is NULL!"); yield break; }


        foreach (char letter in line.ToCharArray())
        {
            if (isSkipping)
            {
                Debug.Log("[DialogueUI] TypeLine: Skipping to end of line.");
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
        Debug.Log($"[DialogueUI] TypeLine coroutine FINISHED for line: \"{line}\". Waiting for linePauseAfterTyping: {linePauseAfterTyping}s");
        yield return new WaitForSecondsRealtime(linePauseAfterTyping);
        Debug.Log($"[DialogueUI] TypeLine linePauseAfterTyping FINISHED for line: \"{line}\"");
    }

    private void HideDialogueOverall()
    {
        Debug.Log($"[DialogueUI] HideDialogueOverall called on GameObject: {this.name}");
        if (typingCoroutine != null)
        {
            Debug.Log("[DialogueUI] Stopping typingCoroutine in HideDialogueOverall.");
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            isTyping = false;
        }
        if (dialogueText != null) { dialogueText.text = ""; Debug.Log("[DialogueUI] Cleared dialogueText."); }
        
        if (dialogueBoxPanel != null) { dialogueBoxPanel.SetActive(false); Debug.Log("[DialogueUI] Set dialogueBoxPanel inactive."); }
        else { Debug.LogWarning("[DialogueUI] HideDialogueOverall: dialogueBoxPanel is NULL."); }

        if (sceneImageDisplay != null) { sceneImageDisplay.gameObject.SetActive(false); Debug.Log("[DialogueUI] Set sceneImageDisplay inactive."); }
        else { Debug.LogWarning("[DialogueUI] HideDialogueOverall: sceneImageDisplay is NULL."); }
        
        Debug.Log($"[DialogueUI] Setting DialogueUI GameObject '{this.name}' inactive.");
        gameObject.SetActive(false);
        Debug.Log("[DialogueUI] HideDialogueOverall FINISHED.");
    }

    public void SkipOrAdvance()
    {
        Debug.Log($"[DialogueUI] SkipOrAdvance called. isTyping: {isTyping}, isSkipping: {isSkipping}");
        if (isTyping && !isSkipping)
        {
            Debug.Log("[DialogueUI] SkipOrAdvance: Setting isSkipping = true.");
            isSkipping = true;
        }
        else if (!isTyping) 
        {
            Debug.Log("[DialogueUI] SkipOrAdvance: Not typing. Requesting DialogueManager.AdvanceDialogue()");
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