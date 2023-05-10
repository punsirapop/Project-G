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
    public int DayLeftBeforeNewQuest => _DayLeftBeforeNewQuest;
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
        // Init the quest for the first time
        Debug.Log("Generate first side quest");
        TimeManager.Date firstDate = new TimeManager.Date();
        firstDate.InitDate();
        GenerateNewQuest(firstDate.AddDay(_MaxQuestDurationDay), _MinCeilingMoneyReward);
        firstDate.InitDate();
        GenerateNewQuest(firstDate.AddDay(_MinQuestDurationDay), _MaxCeilingMoneyReward);
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

    // Return all unacquired side quest
    public List<SideQuestSO> GetAllUnacquiredQuest(bool sortByDate = true)
    {
        List<SideQuestSO> unacquiredQuests = new List<SideQuestSO>();
        foreach (SideQuestSO quest in _SideQuests)
        {
            if (quest.QuestStatus == QuestSO.Status.Unacquired)
            {
                unacquiredQuests.Add(quest);
            }
        }
        if (sortByDate)
        {
            unacquiredQuests.Sort((a, b) => (a.DueDate.CompareDate(b.DueDate)));
        }
        return unacquiredQuests;
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
        // Make sure there is only at most 4 unacquired quests at a time
        List<SideQuestSO> unacquiredQuests = GetAllUnacquiredQuest();
        if (unacquiredQuests.Count <= 4)
        {
            return;
        }
        // If there are more than 4 unacquired quest, delete the exceeding quest
        // Generate random index pool
        int[] poolIndex = new int[unacquiredQuests.Count];
        for (int i = 0; i < unacquiredQuests.Count; i++)
        {
            poolIndex[i] = i;
        }
        // Random picking random distinct quest to delete
        List<SideQuestSO> exceedingQuests = new List<SideQuestSO>();
        foreach (int index in _RandomChoices(poolIndex, poolIndex.Length - 4))
        {
            exceedingQuests.Add(_SideQuests[index]);
        }
        // Remove all exceeding quest from the database
        foreach (SideQuestSO exceedindQuest in exceedingQuests)
        {
            _SideQuests.Remove(exceedindQuest);
        }
    }

    // Return a number of random distinct value from the randomPool equal to the number of randomCount
    private int[] _RandomChoices(int[] randomPool, int randomCount)
    {
        if (randomPool.Length < randomCount)
        {
            return null;
        }
        else if (randomPool.Length == randomCount)
        {
            return randomPool;
        }
        int[] currentRandomPool = randomPool;
        int[] resultPool = new int[randomCount];
        for (int i = 0; i < randomCount; i++)
        {
            // Get new random value in pool
            int newRandomIndex = Random.Range(0, currentRandomPool.Length);
            resultPool[i] = currentRandomPool[newRandomIndex];
            // Remove such value from the pool
            int[] newRandomPool = new int[currentRandomPool.Length - 1];
            for (int j = 0; j < currentRandomPool.Length - 1; j++)
            {
                newRandomPool[j] = (j >= newRandomIndex) ? currentRandomPool[j + 1] : currentRandomPool[j];
            }
            currentRandomPool = newRandomPool;
        }
        return resultPool;
    }

    // Overload method for auto-generate quest
    public void GenerateNewQuestByTime(TimeManager.Date dateBeforeSkip, int skipAmount)
    {
        for (int i = 1; i <= skipAmount; i++)
        {
            _DayLeftBeforeNewQuest--;
            // Calculate the number of quest to be generated
            int unacquiredQuestCount = 0;
            foreach (SideQuestSO sideQuest in _SideQuests)
            {
                if (sideQuest.QuestStatus == QuestSO.Status.Unacquired)
                {
                    unacquiredQuestCount++;
                }
            }
            // If the time's hasn't come, skip to next day
            if (_DayLeftBeforeNewQuest > 0)
            {
                continue;
            }
            // Spawn 2 quests at a time
            for (int j = 0; j < 2; j++)
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
        if (_DayLeftBeforeNewQuest < 0)
        {
            _DayLeftBeforeNewQuest = 0;
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
    public void ForceGenerateNewQuest(TimeManager.Date dueDate, int MaxRewardMoney)
    {
        SideQuestSO newQuest = ScriptableObject.CreateInstance<SideQuestSO>();
        newQuest.SetSideQuest(
            id: _NextSideQuestID,
            name: "Request For Monster",
            briefDescription: "Townsmen request you some robotic monster.",
            fullDescription: "It seems the townsman requests a specific robotic monster from your company. Breed them what they want, give it to them, and get the money!",
            dueDate: dueDate,
            minRewardMoney: MaxRewardMoney/2,
            maxRewardMoney: MaxRewardMoney
            );
        _SideQuests.Add(newQuest);
        _NextSideQuestID++;
    }

    // tmp method for entering with GM start
    public void ForceCompleteQuest()
    {
        foreach (SideQuestSO sideQuest in _SideQuests)
        {
            sideQuest.ForceCompleteQuest();
        }
    }
}
