using UnityEngine;

public class DialogueTrigger : InteractableNPC
{
    [SerializeField] protected string dialogueKey;

    public override void Interact()
    {
        // This method is called by PlayerInteraction when the player presses E near this NPC
        DialogueManager.Instance.StartDialogue(dialogueKey);
    }

    public override void ActOnStatus()
    {
        // Example: handle NPC animations or behaviors
    }
}
