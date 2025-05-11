using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI; // Not strictly needed here anymore

// Assuming GameManager and GameStatus exist in your project
// public enum GameStatus { INGAME, DIALOGUE, /* other statuses */ }
// public class GameManager : MonoBehaviour {
//     public static GameManager Instance { get; private set; }
//     public void ChangeGameStatus(GameStatus newStatus) { /* ... */ }
//     private void Awake() { if (Instance == null) Instance = this; else Destroy(gameObject); }
// }

public struct DialogueLineDisplayInfo
{
    public LineType Type;
    public string Text;
    public Sprite Portrait;
    public Sprite SceneImage;
    public string SpeakerName;
    public Sprite DialogueBoxImage;
    public bool IsFirstLineInDialogue;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Assets")]
    [SerializeField] public DialogueData[] dialogueAssets;
    [SerializeField] private bool autoPlay = false;
    [SerializeField] private float autoPlayDelay = 3f;
    private Dictionary<string, DialogueData> dialogueDictionary = new Dictionary<string, DialogueData>();
    
    public DialogueData currentDialogue;
    public int currentLineIndex = 0;
    private int currentSceneImageIndex = 0;
    private Coroutine autoPlayCoroutine;

    public static event Action<DialogueLineDisplayInfo> OnDisplayUpdateRequest;
    public static event Action OnDialogueEnded;

    private void Awake()
    {
        Debug.Log("[DialogueManager] Awake called.");
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[DialogueManager] Instance created.");
            // Optionally, use DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("[DialogueManager] Duplicate instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        BuildDialogueDictionary();
    }

    public void BuildDialogueDictionary()
    {
        Debug.Log("[DialogueManager] BuildDialogueDictionary called.");
        dialogueDictionary.Clear();
        if (dialogueAssets == null)
        {
            Debug.LogWarning("[DialogueManager] dialogueAssets array is null.");
            return;
        }
        Debug.Log($"[DialogueManager] dialogueAssets count: {dialogueAssets.Length}");
        foreach (var data in dialogueAssets)
        {
            if (data == null)
            {
                Debug.LogWarning("[DialogueManager] Found a null DialogueData in dialogueAssets array.");
                continue;
            }
            if (string.IsNullOrEmpty(data.name))
            {
                 Debug.LogWarning($"[DialogueManager] DialogueData asset has a null or empty name (filename). Skipping.");
                 continue;
            }
            if (!dialogueDictionary.ContainsKey(data.name))
            {
                dialogueDictionary.Add(data.name, data);
                Debug.Log($"[DialogueManager] Added DialogueData with key: {data.name}");
            }
            else
            {
                Debug.LogWarning($"[DialogueManager] Duplicate DialogueData key found: {data.name}. Skipping.");
            }
        }
    }

    public void StartDialogue(string dialogueKey)
    {
        Debug.Log($"[DialogueManager] StartDialogue called with key: '{dialogueKey}'");
        if (!dialogueDictionary.TryGetValue(dialogueKey, out currentDialogue))
        {
            Debug.LogError($"[DialogueManager] Dialogue '{dialogueKey}' not found in dictionary!");
            return;
        }
        Debug.Log($"[DialogueManager] Found dialogue: {currentDialogue.name}");

        currentLineIndex = 0;
        currentSceneImageIndex = 0;

        if (GameManager.Instance != null)
        {
            Debug.Log("[DialogueManager] GameManager.Instance is available. Changing game status to DIALOGUE.");
            GameManager.Instance.ChangeGameStatus(GameStatus.DIALOGUE);
        }
        else
        {
            Debug.LogWarning("[DialogueManager] GameManager.Instance is NULL. Cannot change game status.");
        }

        Debug.Log("[DialogueManager] Calling AdvanceDialogue(true) from StartDialogue.");
        AdvanceDialogue(true);
    }

    public bool HasDialogue()
    {
        return currentDialogue != null;
    }

    public void AdvanceDialogue(bool isFirstLine = false)
    {
        Debug.Log($"[DialogueManager] AdvanceDialogue called. CurrentLineIdx: {currentLineIndex}, IsFirst: {isFirstLine}, CurrentDialogue: {(currentDialogue != null ? currentDialogue.name : "NULL")}, AutoPlayCor: {(autoPlayCoroutine != null)}");

        if (autoPlayCoroutine != null)
        {
            Debug.Log("[DialogueManager] Stopping existing autoPlayCoroutine.");
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        if (currentDialogue == null)
        {
            Debug.LogWarning("[DialogueManager] No active dialogue (currentDialogue is NULL) to advance.");
            return;
        }
        if (currentDialogue.dialogueLines == null)
        {
            Debug.LogError($"[DialogueManager] currentDialogue '{currentDialogue.name}' has a NULL dialogueLines list! Cannot advance.");
            EndDialogue(); // Or handle error appropriately
            return;
        }

        Debug.Log($"[DialogueManager] Checking line {currentLineIndex} against dialogueLines count {currentDialogue.dialogueLines.Count}");
        if (currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            DialogueLineEntry currentEntry = currentDialogue.dialogueLines[currentLineIndex];
            if (currentEntry == null)
            {
                Debug.LogError($"[DialogueManager] currentDialogue '{currentDialogue.name}' line {currentLineIndex} (DialogueLineEntry) is NULL! Cannot advance.");
                currentLineIndex++; // Skip this null entry
                AdvanceDialogue(isFirstLine); // Try to advance to next, or it might loop if many are null
                return;
            }

            Debug.Log($"[DialogueManager] Processing Entry - Index: {currentLineIndex}, LineType: {currentEntry.lineType}, Sentence: \"{currentEntry.sentenceText}\", HasPortraitOverride: {currentEntry.portraitOverride != null}, DisplaySceneImageFlag: {currentEntry.displaySceneImage}");

            DialogueLineDisplayInfo displayInfo = new DialogueLineDisplayInfo
            {
                SpeakerName = currentDialogue.speakerName,
                DialogueBoxImage = currentDialogue.dialogueBoxImage,
                IsFirstLineInDialogue = isFirstLine
            };

            if (currentEntry.displaySceneImage || currentEntry.lineType == LineType.SceneImage)
            {
                Debug.Log($"[DialogueManager] --- Scene Image Path Selected for line {currentLineIndex} ---");
                displayInfo.Type = LineType.SceneImage;
                displayInfo.Text = currentEntry.sentenceText;

                Debug.Log($"[DialogueManager] currentDialogue.sceneImages is {(currentDialogue.sceneImages == null ? "NULL" : "Assigned (Count: " + currentDialogue.sceneImages.Count + ")")}");
                Debug.Log($"[DialogueManager] currentSceneImageIndex: {currentSceneImageIndex}");

                if (currentDialogue.sceneImages != null && currentSceneImageIndex < currentDialogue.sceneImages.Count)
                {
                    Sprite sceneSprite = currentDialogue.sceneImages[currentSceneImageIndex];
                    displayInfo.SceneImage = sceneSprite;
                    Debug.Log($"[DialogueManager] Assigning SceneImage: {(sceneSprite != null ? sceneSprite.name : "NULL SPRITE IN LIST")}");
                    currentSceneImageIndex++;
                }
                else
                {
                    displayInfo.SceneImage = null;
                    Debug.LogWarning($"[DialogueManager] Condition for assigning scene image FAILED for line {currentLineIndex}. sceneImages is null, or index out of bounds, or list empty. currentDialogue.sceneImages.Count: {(currentDialogue.sceneImages?.Count)}, currentSceneImageIndex: {currentSceneImageIndex}");
                }
                displayInfo.Portrait = null;
            }
            else // LineType.Text
            {
                Debug.Log($"[DialogueManager] --- Text Path Selected for line {currentLineIndex} ---");
                displayInfo.Type = LineType.Text;
                displayInfo.Text = currentEntry.sentenceText;
                displayInfo.SceneImage = null;

                if (currentEntry.portraitOverride != null)
                {
                    displayInfo.Portrait = currentEntry.portraitOverride;
                    Debug.Log($"[DialogueManager] Using Portrait Override: {currentEntry.portraitOverride.name}");
                }
                else
                {
                    Debug.Log($"[DialogueManager] Attempting to get default portrait for line index {currentLineIndex}.");
                    displayInfo.Portrait = GetPortraitForLine(currentLineIndex);
                }
            }
            
            string portraitName = displayInfo.Portrait != null ? displayInfo.Portrait.name : "None";
            string sceneImageName = displayInfo.SceneImage != null ? displayInfo.SceneImage.name : "None";
            Debug.Log($"[DialogueManager] Invoking OnDisplayUpdateRequest. Type: {displayInfo.Type}, Text: \"{displayInfo.Text}\", Portrait: {portraitName}, SceneImg: {sceneImageName}, Speaker: {displayInfo.SpeakerName}");
            
            OnDisplayUpdateRequest?.Invoke(displayInfo);
            currentLineIndex++;
            Debug.Log($"[DialogueManager] Incremented currentLineIndex to: {currentLineIndex}");

            Debug.Log($"[DialogueManager] AutoPlay is {(autoPlay ? "ON" : "OFF")}.");
            if (autoPlay)
            {
                if (currentLineIndex < currentDialogue.dialogueLines.Count)
                {
                    Debug.Log($"[DialogueManager] Scheduling AutoAdvanceAfterDelay for {autoPlayDelay}s.");
                    autoPlayCoroutine = StartCoroutine(AutoAdvanceAfterDelay(autoPlayDelay));
                }
                else
                {
                    Debug.Log($"[DialogueManager] Last line processed with autoPlay. Scheduling AutoEndAfterDelay for {autoPlayDelay}s.");
                    autoPlayCoroutine = StartCoroutine(AutoEndAfterDelay(autoPlayDelay));
                }
            }
        }
        else
        {
            Debug.Log($"[DialogueManager] No more lines (currentLineIndex {currentLineIndex} >= dialogueLines.Count {currentDialogue.dialogueLines.Count}). Calling EndDialogue.");
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        Debug.Log($"[DialogueManager] EndDialogue called. CurrentDialogue: {(currentDialogue != null ? currentDialogue.name : "NULL")}");
        if (autoPlayCoroutine != null)
        {
            Debug.Log("[DialogueManager] Stopping autoPlayCoroutine in EndDialogue.");
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        OnDialogueEnded?.Invoke();
        Debug.Log("[DialogueManager] OnDialogueEnded invoked.");

        currentDialogue = null;
        currentLineIndex = 0;
        currentSceneImageIndex = 0;
        Debug.Log("[DialogueManager] Cleared currentDialogue and reset indices.");

        if (GameManager.Instance != null)
        {
            Debug.Log("[DialogueManager] GameManager.Instance is available. Changing game status to INGAME.");
            GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
        }
        else
        {
            Debug.LogWarning("[DialogueManager] GameManager.Instance is NULL in EndDialogue. Cannot change game status.");
        }
    }

    private Sprite GetPortraitForLine(int lineIndex)
    {
        Sprite portraitToReturn = null;
        string logMsg = $"[DialogueManager] GetPortraitForLine({lineIndex}). ";
        if (currentDialogue != null && currentDialogue.portraits != null)
        {
            logMsg += $"currentDialogue.portraits Count: {currentDialogue.portraits.Count}. ";
            if (currentDialogue.portraits.Count > 0)
            {
                portraitToReturn = lineIndex < currentDialogue.portraits.Count ? currentDialogue.portraits[lineIndex] : currentDialogue.portraits[currentDialogue.portraits.Count - 1];
                logMsg += $"Returning: {(portraitToReturn != null ? portraitToReturn.name : "NULL SPRITE")}";
            }
            else
            {
                logMsg += "Portraits list is empty.";
            }
        }
        else
        {
            logMsg += "currentDialogue or currentDialogue.portraits is NULL.";
        }
        Debug.Log(logMsg);
        return portraitToReturn;
    }

    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        Debug.Log($"[DialogueManager] AutoAdvanceAfterDelay coroutine WAITING for {delay}s.");
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log($"[DialogueManager] AutoAdvanceAfterDelay coroutine AWAKE. CurrentDialogue: {(currentDialogue != null ? currentDialogue.name : "NULL")}");
        if (currentDialogue != null)
        {
            Debug.Log("[DialogueManager] AutoAdvanceAfterDelay calling AdvanceDialogue().");
            AdvanceDialogue();
        }
         else
        {
            Debug.LogWarning("[DialogueManager] AutoAdvanceAfterDelay: currentDialogue is null, will not advance.");
        }
    }

    private IEnumerator AutoEndAfterDelay(float delay)
    {
        Debug.Log($"[DialogueManager] AutoEndAfterDelay coroutine WAITING for {delay}s.");
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log($"[DialogueManager] AutoEndAfterDelay coroutine AWAKE. CurrentDialogue: {(currentDialogue != null ? currentDialogue.name : "NULL")}");
        if (currentDialogue != null) // This check might be redundant if EndDialogue clears currentDialogue, but good for safety
        {
            Debug.Log("[DialogueManager] AutoEndAfterDelay calling EndDialogue().");
            EndDialogue();
        }
        else
        {
            Debug.LogWarning("[DialogueManager] AutoEndAfterDelay: currentDialogue was already null, dialogue likely ended by other means.");
        }
    }
}