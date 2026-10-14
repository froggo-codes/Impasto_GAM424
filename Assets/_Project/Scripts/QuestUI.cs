using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public RectTransform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    public Quest testQuest; // Assign this in the inspector for testing
    public int testQuestAmount;

    private List<QuestProgress> testQuests = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < testQuestAmount; i++)
        {
            testQuests.Add(new QuestProgress(testQuest));
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
        foreach(var quest in testQuests)
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
