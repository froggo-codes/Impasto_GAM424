using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    public Quest currentQuest; // Assign this in the inspector for testing
    public int currentQuestAmount;

    private List<QuestProgress> currentQuests = new();

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
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGO.GetComponent<TMP_Text>();
                objText.text = $"{objective.currentAmount}/{objective.requiredAmount}";
            }
        }
    }
}
