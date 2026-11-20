using UnityEngine;

public class Song : MonoBehaviour
{
    [SerializeField] private GameObject song;
    [SerializeField] private QuestUI questUI;
    [SerializeField] private int objective;
    [SerializeField] private GameObject pianoUI;


    void Start()
    {
        song.SetActive(false);
    }

    void Update()
    {
        if (pianoUI.activeInHierarchy)
        {
            ShowSong();
        }
       
    }

    public void ShowSong()
    {   
        var obj = questUI.currentQuests[0].objectives[objective];
        Debug.Log("Song check: " + obj.currentAmount + " / " + obj.requiredAmount);

        if (obj.currentAmount >= obj.requiredAmount)
        {
            song.SetActive(true);
        }
        else return;
    }
}
