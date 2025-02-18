using UnityEngine;
using GameSystem;
using Castle.Components.DictionaryAdapter;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private Dialogue dialogueUI; // Reference to the Dialogue UI system
    private Dictionary<string, DialogueData> dialogueDictionary; // Fast lookup

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(string dialogueName) // ✅ Accepts Dialogue Name
    {
        if (dialogueUI == null)
        {
            Debug.LogError("Dialogue UI is not assigned in the inspector!");
            return;
        }

        GameManager.Instance.SetGameState(GameState.InDialogue);
        dialogueUI.StartDialogue(dialogueDictionary[dialogueName]); // ✅ UI Handles Displaying
    }

    public void EndDialogue() // ✅ Only needed to restore game state
    {
        GameManager.Instance.SetGameState(GameState.InGame);
    }
}
