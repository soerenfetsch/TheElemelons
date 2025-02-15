using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public Dialogue dialogueUI; // Reference to the DialogueUI component
    private DialogueData currentDialogueData;
    private int currentLineIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        currentDialogueData = dialogueData;
        currentLineIndex = 0;
        dialogueUI.gameObject.SetActive(true);
        dialogueUI.StartDialogue(dialogueData.sentences);

        // Notify GameManager that a dialogue has started
        GameManager.Instance.SetGameState(GameState.InDialogue);
    }

    public void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentDialogueData.sentences.Length)
        {
            dialogueUI.NextLine();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        dialogueUI.gameObject.SetActive(false);
        currentDialogueData = null;

        // Notify GameManager that the dialogue has ended
        GameManager.Instance.SetGameState(GameState.Playing);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Example: Progress dialogue on mouse click
        {
            if (currentDialogueData != null)
            {
                NextLine();
            }
        }
    }
}