using UnityEngine;

public class Song : MonoBehaviour
{
    [SerializeField] private GameObject song;
    [SerializeField] private QuestUI questUI;
    [SerializeField] private int objective;


    void Start()
    {
        song.SetActive(false);
    }

    void Update()
    {
        ShowSong();
    }

    public void ShowSong()
    {   
        if (questUI.currentQuests[0].objectives[objective].currentAmount >= questUI.currentQuests[0].objectives[objective].requiredAmount)
        {
            song.SetActive(true);
        }
        else return;
    }
}
