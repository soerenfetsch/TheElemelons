using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float typingSpeed;
    private int index;
    public string[] sentences;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == sentences[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = sentences[index];
            }
        }
    }

    public void StartDialogue()
    {
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