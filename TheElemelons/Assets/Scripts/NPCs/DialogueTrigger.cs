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
            string dialogueKey = basic_dialogues_list[nextDialogue++].name;
            DialogueManager.Instance.StartDialogue(dialogueKey);
            if (nextDialogue >= basic_dialogues_list.Count)
            {
                nextDialogue = 0;

            }
        }
    }

    public override void ActOnStatus()
    {
        // Example: handle NPC animations or behaviors
    }
}
