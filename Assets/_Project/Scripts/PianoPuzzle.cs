using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PianoPuzzle : MonoBehaviour
{
    [SerializeField]
    private Button[] buttons;
    private int currentNoteIndex = 0;
    void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }

    }

    void OnButtonClicked(Button clickedButton)
    {
        if (clickedButton == buttons[currentNoteIndex])
        {
            currentNoteIndex++;

            if (currentNoteIndex >= buttons.Length)
            {
                UnityEngine.Debug.Log("Puzzle solved");
                //  Door.Open();
                currentNoteIndex = 0;

            }   
        }
        
        else
        {
            UnityEngine.Debug.Log("Reset Note Sequence");
            currentNoteIndex = 0;
        }
     
    }
}
