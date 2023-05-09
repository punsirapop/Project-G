using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/QuestMainDatabase")]
public class MainQuestDatabaseSO : ScriptableObject
{
    [SerializeField] private MainQuestSO[] _MainQuests;
    private int _CurrentQuestIndex;
    private static bool _IsDayPassed;   // Boolean to control the quest refreshing, new quest available after the day pass

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
        _CurrentQuestIndex = 0;
        _IsDayPassed = true;
        foreach (QuestSO quest in _MainQuests)
        {
            quest.SetStatus(QuestSO.Status.Unacquired);
        }
    }

    public void PassDay()
    {
        _IsDayPassed = true;
    }

    public static void WaitForDay()
    {
        _IsDayPassed = false;
    }

    public MainQuestSO GetCurrentQuest()
    {
        // If a day didn't since last quest complete, don't give any quest yet
        if (!_IsDayPassed)
        {
            return null;
        }
        // If ran out of quest, return nothing
        if (_CurrentQuestIndex > _MainQuests.Length - 1)
        {
            return null;
        }
        return _MainQuests[_CurrentQuestIndex];
    }

    public MainQuestSO GetQuestProgress()
    {
        // All quest ran out, return last quest
        if (_CurrentQuestIndex > _MainQuests.Length - 1)
        {
            return _MainQuests[_MainQuests.Length - 1];
        }
        // Don't skip day yet, return last completed quest
        if (!_IsDayPassed)
        {
            return _MainQuests[_CurrentQuestIndex - 1];
        }
        // Skip day already and there is some quest, return current quest
        return GetCurrentQuest();

    }

    public void ValidateAllQuestStatus()
    {
        // If all quest is completed, do nothing
        if (_CurrentQuestIndex > _MainQuests.Length - 1)
        {
            return;
        }
        for (int i = 0; i < _MainQuests.Length; i++)
        {
            _MainQuests[i].ValdiateStatus();
        }
        // If current quest is completed, move on next quest
        if (_MainQuests[_CurrentQuestIndex].QuestStatus == QuestSO.Status.Completed)
        {
            _CurrentQuestIndex++;
        }
    }

    // tmp method for entering with GM start
    public void ForceCompleteQuest()
    {
        for (int i = 0; i < _MainQuests.Length; i++)
        {
            _MainQuests[i].ForceCompleteQuest();
        }
        _CurrentQuestIndex = _MainQuests.Length - 1;
    }
}
