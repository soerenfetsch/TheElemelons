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
        if (DialogueManager.Instance.HasDialogue())
        {
            DialogueManager.Instance.AdvanceDialogue();
        }
        else
        {
            //string dialogueKey = basic_dialogues_list[nextDialogue++].name;
            Debug.Log($"[DialogueTrigger] Starting new dialogue sequence from {gameObject.name}.");
            if (basic_dialogues_list == null || basic_dialogues_list.Count == 0)
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.INGAME); // Revert status if dialogue can't start
                return;
            }

            DialogueData dataToStart = basic_dialogues_list[nextDialogue];
            if (dataToStart == null)
            {
                GameManager.Instance.ChangeGameStatus(GameStatus.INGAME);
                IncNextDialogue(); // Try to advance for next time, or clamp
                return;
            }

            DialogueManager.Instance.StartDialogue(dataToStart);

            IncNextDialogue();
        }
    }
    public virtual void IncNextDialogue()
    {
        nextDialogue = (nextDialogue + 1) % basic_dialogues_list.Count;
    }

    public override void ActOnStatus()
    {
        // Example: handle NPC animations or behaviors
    }
}
