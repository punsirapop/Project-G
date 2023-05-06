using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/QuestSideDatabase")]
public class SideQuestDatabaseSO : ScriptableObject
{
    [SerializeField] private List<SideQuestSO> _SideQuests;
    private int _NextSideQuestID;
    public int NextSideQuestID => _NextSideQuestID;
    private int _DayLeftBeforeNewQuest;
    [SerializeField] private int _PeriodDayBeforeNewQuest;
    [SerializeField] private int _MinQuestDurationDay;
    [SerializeField] private int _MaxQuestDurationDay;
    [SerializeField] private int _MinCeilingMoneyReward;
    [SerializeField] private int _MaxCeilingMoneyReward;

    private void OnEnable()
    {
        SaveManager.OnReset += Reset;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= Reset;
    }

    public void Reset()
    {
        _SideQuests = new List<SideQuestSO>();
        _NextSideQuestID = 0;
        _DayLeftBeforeNewQuest = _PeriodDayBeforeNewQuest;
    }

    // Return all side quest in database
    public List<SideQuestSO> GetAllQuest(bool sortByDate = true)
    {
        if (sortByDate)
        {
            _SideQuests.Sort((a, b) => (a.DueDate.CompareDate(b.DueDate)));
        }
        return _SideQuests;
    }

    // Return all acquired side quest
    public List<SideQuestSO> GetAllAcquiredQuest(bool sortByDate=true)
    {
        List<SideQuestSO> acquiredQuests = new List<SideQuestSO>();
        foreach (SideQuestSO quest in _SideQuests)
        {
            if ((quest.QuestStatus == QuestSO.Status.InProgress) ||
                (quest.QuestStatus == QuestSO.Status.Completable) ||
                (quest.QuestStatus == QuestSO.Status.Expired))
            {
                acquiredQuests.Add(quest);
            }
        }
        if (sortByDate)
        {
            acquiredQuests.Sort((a, b) => (a.DueDate.CompareDate(b.DueDate)));
        }
        return acquiredQuests;
    }

    public void ValidateAllQuestStatus()
    {
        // Validate all side quests
        foreach (SideQuestSO sideQuest in _SideQuests)
        { 
            sideQuest.ValdiateStatus();
        }
        // Adjust quest in database
        List<SideQuestSO> questsToDelete = new List<SideQuestSO>();
        foreach (SideQuestSO sideQuest in _SideQuests)
        {
            // Remove all completed quests
            if (sideQuest.QuestStatus == QuestSO.Status.Completed)
            {
                questsToDelete.Add(sideQuest);
            }
            // Remove all unacquired quest that expired
            else if ((sideQuest.QuestStatus == QuestSO.Status.Unacquired) && sideQuest.IsTimeOut())
            {
                questsToDelete.Add(sideQuest);
            }
        }
        // Delete quest
        foreach (SideQuestSO questToDelete in questsToDelete)
        {
            _SideQuests.Remove(questToDelete);
        }
    }
    
    // Overload method for auto-generate quest
    public void GenerateNewQuestByTime(TimeManager.Date dateBeforeSkip, int skipAmount)
    {
        for (int i = 1; i <= skipAmount; i++)
        {
            _DayLeftBeforeNewQuest--;
            // Generate new quest
            if (_DayLeftBeforeNewQuest <= 0)
            {
                // Calculate duration
                int newQuestDuration = Random.Range(_MinQuestDurationDay, _MaxQuestDurationDay);
                // Calculate max money reward, more duration, less money bonus
                float bonusRatio = 1f - ((float)(newQuestDuration - _MinQuestDurationDay)) / ((float)(_MaxQuestDurationDay - _MinQuestDurationDay));
                int maxRewardMoney = _MinCeilingMoneyReward + (int)(bonusRatio * (_MaxCeilingMoneyReward - _MinCeilingMoneyReward));
                // Generate new quest
                GenerateNewQuest(
                    dateBeforeSkip.DupeDate().AddDay(i + newQuestDuration), 
                    maxRewardMoney);
                _DayLeftBeforeNewQuest = _PeriodDayBeforeNewQuest;
            }
        }
    }

    public void GenerateNewQuest(TimeManager.Date dueDate, int maxRewardMoney)
    {
        SideQuestSO newQuest = ScriptableObject.CreateInstance<SideQuestSO>();
        newQuest.SetSideQuest(
            id: _NextSideQuestID,
            name: "Request For Monster",
            briefDescription: "Townsmen request you some robotic monster.",
            fullDescription: "It seems the townsman requests a specific robotic monster from your company. Breed them what they want, give it to them, and get the money!",
            dueDate: dueDate,
            minRewardMoney: Mathf.RoundToInt(maxRewardMoney/2),
            maxRewardMoney: maxRewardMoney
            );
        _SideQuests.Add(newQuest);
        _NextSideQuestID++;
    }

    // Tmp mthod for immediately generate quest in QuestBoard scene
    public void ForceGenerateNewQuest()
    {
        SideQuestSO newQuest = ScriptableObject.CreateInstance<SideQuestSO>();
        newQuest.SetSideQuest(
            id: _NextSideQuestID,
            name: "Request For Monster",
            briefDescription: "Townsmen request you some robotic monster.",
            fullDescription: "It seems the townsman requests a specific robotic monster from your company. Breed them what they want, give it to them, and get the money!",
            dueDate: PlayerManager.CurrentDate.DupeDate().AddMonth(1),
            minRewardMoney: 100,
            maxRewardMoney: 500
            );
        _SideQuests.Add(newQuest);
        _NextSideQuestID++;
    }
}
