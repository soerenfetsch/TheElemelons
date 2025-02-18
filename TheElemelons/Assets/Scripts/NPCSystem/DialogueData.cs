using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string speakerName;  // ✅ Store the speaker name
    public SpriteRenderer portrait;  // ✅ Store the speaker portrait
    [TextArea(3, 10)] public string[] sentences; // ✅ Store dialogue text
}
