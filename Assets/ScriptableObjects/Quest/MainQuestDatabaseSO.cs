using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/MainQuestDatabase")]
public class MainQuestDatabaseSO : ScriptableObject
{
    [SerializeField] private QuestSO[] _MainQuests;
    private int _CurrentQuestIndex;

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
        foreach (QuestSO quest in _MainQuests)
        {
            quest.SetStatus(QuestSO.Status.Unacquired);
        }
    }

    public QuestSO GetCurrentQuest()
    {
        if (_CurrentQuestIndex > _MainQuests.Length - 1)
        {
            return null;
        }
        return _MainQuests[_CurrentQuestIndex];
    }

    public void ValidateAllQuestStatus()
    {
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
}
