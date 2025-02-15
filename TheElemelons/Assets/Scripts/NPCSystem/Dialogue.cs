using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed;
    private int index;
    private string[] sentences;

    public void StartDialogue(string[] dialogueSentences)
    {
        sentences = dialogueSentences;
        index = 0;
        textComponent.text = string.Empty;
        gameObject.SetActive(true);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextLine()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        gameObject.SetActive(false);
    }
}