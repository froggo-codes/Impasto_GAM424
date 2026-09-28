using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]   
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines; // Array to determine if each line should auto-progress after a delay
    public float autoProgressDelay = 1.5f; // Delay before auto-progressing to the next line 
    public float typingSpeed = 0.05f; // Time delay between each character when typing out the dialogue
    public AudioClip voiceSound; // Sound effect for typing
    public float voicePitch = 1f;
}
