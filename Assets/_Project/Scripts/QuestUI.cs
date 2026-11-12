using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=sQyj7z9W7kk
//Instantiates quest UI prefabs when a scene is loaded and receives updates from the Collision Detector script. 

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public Image objectiveIconPrefab;
    public GameObject objectiveTextPrefab;

    public Quest currentQuest; // Assign this in the inspector for testing
    public int currentQuestAmount;

    public List<QuestProgress> currentQuests = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < currentQuestAmount; i++)
        {
            currentQuests.Add(new QuestProgress(currentQuest));
        }

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        //Destroy existing quest entries
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        //Build quest entries
        foreach(var quest in currentQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            foreach(var objective in quest.objectives)
            {
                // Image objIcon = Instantiate(objectiveIconPrefab, objectiveList);
                // objIcon.sprite = objective.icon;

                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.currentAmount}/{objective.requiredAmount}";
            }
        }
    
    }

    public void Update()
    {
        for (int i = 0; i < currentQuests.Count; i++)
        {
            var questUI = questListContent.GetChild(i);
            var questProgress = currentQuests[i];

            for (int j = 0; j < questProgress.objectives.Count; j++)
            {
                var objectiveUI = questUI.Find("ObjectiveList").GetChild(j);
                var objectiveProgress = questProgress.objectives[j];

                // Image objIcon = objectiveUI.GetComponent<Image>();
                // objIcon.sprite = objectiveProgress.icon;

                TMP_Text objText = objectiveUI.GetComponent<TMP_Text>();
                objText.text = $"{objectiveProgress.currentAmount}/{objectiveProgress.requiredAmount}";
            }
        }
    }
}
