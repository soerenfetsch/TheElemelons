using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : InteractableNPC
{
    [Header("Dialogue")]
    protected int nextDialogue = 0;
    public List<DialogueData> basic_dialogues_list;

    public override void Interact()
    {
        // This method is called by PlayerInteraction when the player presses E near this NPC
        GameManager.Instance.ChangeGameStatus(GameStatus.DIALOGUE);
        Debug.Log($"[DialogueTrigger] Interact called on {gameObject.name}. nextDialogue index: {nextDialogue}. HasDialogue: {DialogueManager.Instance != null && DialogueManager.Instance.HasDialogue()}");

        if (GameManager.Instance == null)
        {
            Debug.LogError("[DialogueTrigger] GameManager.Instance is NULL in Interact!");
            return;
        }
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueTrigger] DialogueManager.Instance is NULL in Interact!");
            return;
        }

        // GameManager.Instance.ChangeGameStatus(GameStatus.DIALOGUE); // Moved to DialogueManager.StartDialogue

        if (DialogueManager.Instance.HasDialogue())
        {
            Debug.Log($"[DialogueTrigger] Advancing existing dialogue via DialogueManager on {gameObject.name}.");
            DialogueManager.Instance.AdvanceDialogue();
        }
        else
        {
            string dialogueKey = basic_dialogues_list[nextDialogue++].name;
            Debug.Log($"[DialogueTrigger] Starting new dialogue sequence from {gameObject.name}.");
            if (basic_dialogues_list == null || basic_dialogues_list.Count == 0)
            {
                Debug.LogError($"[DialogueTrigger] basic_dialogues_list is null or empty on {gameObject.name}!");
                GameManager.Instance.ChangeGameStatus(GameStatus.INGAME); // Revert status if dialogue can't start
                return;
            }
            if (nextDialogue >= basic_dialogues_list.Count || nextDialogue < 0)
            {
                Debug.LogWarning($"[DialogueTrigger] nextDialogue index ({nextDialogue}) is out of bounds for basic_dialogues_list (Count: {basic_dialogues_list.Count}) on {gameObject.name}. Resetting to 0.");
                nextDialogue = 0;
                if (basic_dialogues_list.Count == 0)
                { // Double check after reset if list was empty
                    Debug.LogError($"[DialogueTrigger] basic_dialogues_list is empty even after reset on {gameObject.name}!");
                    GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
                    return;
                }
            }

            DialogueData dataToStart = basic_dialogues_list[nextDialogue];
            if (dataToStart == null)
            {
                Debug.LogError($"[DialogueTrigger] DialogueData at basic_dialogues_list[{nextDialogue}] is NULL on {gameObject.name}!");
                GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
                nextDialogue = (nextDialogue + 1) % basic_dialogues_list.Count; // Try to advance for next time, or clamp
                return;
            }

            DialogueManager.Instance.StartDialogue(dataToStart);

            nextDialogue++;
            if (nextDialogue >= basic_dialogues_list.Count)
            {
                Debug.Log($"[DialogueTrigger] Reached end of basic_dialogues_list on {gameObject.name}. Resetting nextDialogue index to 0.");
                nextDialogue = 0;
            }
        }
    }

    public override void ActOnStatus()
    {
        // Example: handle NPC animations or behaviors
    }
}
