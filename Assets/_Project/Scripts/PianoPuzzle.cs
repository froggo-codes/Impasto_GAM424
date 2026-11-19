using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;   // ◆ NEW (CALE): needed for UnityEvent

public class PianoPuzzle : MonoBehaviour
{
    // public Door door;                          // ◆ REMOVED (CALE): replaced by onSolved
    [SerializeField] private UnityEvent onSolved; // ◆ NEW (CALE): what happens when the song is played (door, bird, anything)

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
        if (!enabled) return;   // ◆ NEW (CALE): button listeners still fire on disabled components, so check

        if (clickedButton == buttons[currentNoteIndex])
        {
            currentNoteIndex++;

            if (currentNoteIndex >= buttons.Length)
            {
                UnityEngine.Debug.Log("Puzzle solved");
                onSolved.Invoke();      // ◆ CHANGED (CALE): was door.Open()
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