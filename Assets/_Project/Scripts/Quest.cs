using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public List<QuestObjective> objectives;

    internal static void Collect()
    {
        throw new NotImplementedException();
    }

    //Called when the scriptable object is edited
    private void OnValidate()
    {
        if(string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }

}

[Serializable]
public class QuestObjective
{
    public string objectiveID; //Match with item ID to collect, enemy ID to kill, etc.
    public ObjectiveType type;
    public int requiredAmount; //How many items/enemies/locations are needed
    public int currentAmount; //How many items/enemies/locations have been completed
    
    public bool IsCompleted => currentAmount >= requiredAmount;
}

public enum ObjectiveType { CollectItem, DefeatEnemy, ReachLocation, TalkNPC, Custom }

[Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;
 
    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        //Deep copy to avoid modifying the original quest objectives
        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
        
    }

    private CollisionDetector collisionDetector;
    public void UpdateQuest()
    {
        if (objectives[0].objectiveID != collisionDetector.gameObject.GetComponent<Item>().ID) //check if the collided item matches the objective ID
        {
            return;
        } else
        {
            objectives[0].currentAmount += 1; //Increase objective count
        }
    }


    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);
    public string QuestID => quest.questID;
}