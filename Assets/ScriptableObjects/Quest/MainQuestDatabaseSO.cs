using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/QuestMainDatabase")]
public class MainQuestDatabaseSO : ScriptableObject
{
    [SerializeField] private MainQuestSO[] _MainQuests;
    public MainQuestSO[] MainQuests => _MainQuests;
    private int _CurrentQuestIndex;
    public int CurrentQuestIndex => _CurrentQuestIndex;
    private static bool _IsDayPassed;   // Boolean to control the quest refreshing, new quest available after the day pass
    public static bool IsDayPassed => _IsDayPassed;

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
        Debug.Log("Get current main quest");
        Debug.Log("Index exceed = " + (_CurrentQuestIndex > _MainQuests.Length - 1).ToString());
        Debug.Log("Not _IsDayPassed = " + (!_IsDayPassed).ToString());
        if ((_CurrentQuestIndex > _MainQuests.Length - 1) ||
            (!_IsDayPassed))
        {
            return null;
        }
        return _MainQuests[_CurrentQuestIndex];
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

    // Save - Load
    public void Load(int c, bool b, QuestSO.Status[] s)
    {
        _CurrentQuestIndex = c;
        _IsDayPassed = b;
        for (int i = 0; i < MainQuests.Length; i++)
        {
            MainQuests[i].Load(s[i]);
        }
    }
}
