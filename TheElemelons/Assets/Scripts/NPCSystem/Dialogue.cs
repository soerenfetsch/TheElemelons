using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI nameText; // Display the speaker's name
    public SpriteRenderer portrait; // Display the speaker's portrait
    public TextMeshProUGUI textComponent;
    public float typingSpeed;
    private int index;
    private string[] sentences;
    private bool isTyping;

    public void StartDialogue(DialogueData dialogueData) // ✅ Accepts DialogueData
    {
        nameText.text = dialogueData.speakerName; // ✅ Display the speaker's name
        portrait.sprite = dialogueData.portrait.sprite; // ✅ Display the speaker's portrait
        sentences = dialogueData.sentences;
        index = 0;
        textComponent.text = string.Empty;
        gameObject.SetActive(true);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        textComponent.text = string.Empty;

        foreach (char letter in sentences[index].ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void NextLine() // ✅ Handles Progressing to the Next Line
    {
        if (isTyping) return; // Prevent skipping while text is typing

        if (index < sentences.Length - 1)
        {
            index++;
            StopAllCoroutines();
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        textComponent.text = string.Empty;
        gameObject.SetActive(false);
        DialogueManager.Instance.EndDialogue(); // ✅ Notify manager when done
    }

    private void OnEnable()
    { 
        InputManager.AdvanceDialogue += NextLine; // ✅ Subscribe to Input Event
    }

    private void OnDisable()
    {
        InputManager.AdvanceDialogue -= NextLine; // ✅ Unsubscribe from Input Event
    }
}
