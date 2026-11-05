using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

//Calls the Interact method which is defined as a dialogue script in the case of NPCs.
//https://www.youtube.com/watch?v=eSH9mzcMRqw

public class NPCDrop : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    //public Image portraitImage;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive, talkedTo;

    [SerializeField] private UnityEvent onDialogueEnd;

    public bool CanInteract()
    {
        return !isDialogueActive && !talkedTo;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive && talkedTo)) // If no dialogue data or the game is paused and no dialogue is active, return
            return;

        if (isDialogueActive) // If dialogue is active, call next line
        {
            NextLine();
        }
        else if(!talkedTo) // Otherwise start dialogue
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        talkedTo = true;
        
        nameText.SetText(dialogueData.npcName);
        //portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);
        PauseController.SetPause(true); 

        StartCoroutine(TypeLine());

    }

    void NextLine()
    {
        if (isTyping)
        {
            //Skip typing animation and show the full line
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            //If another line, type next line 
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(dialogueData.typingSpeed);
        }

        isTyping = false;

        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSecondsRealtime(dialogueData.autoProgressDelay);
            NextLine();
        }

    }
    public void EndDialogue() 
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);
        onDialogueEnd.Invoke(); // Drop the quest item
    }
}
