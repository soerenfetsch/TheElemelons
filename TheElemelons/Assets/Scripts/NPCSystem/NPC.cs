using UnityEngine;

public class NPC : MonoBehaviour
{
    public DialogueData dialogueData; // Assign this in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.EndDialogue();
        }
    }
}