using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _QuestNameText;
    [SerializeField] private TextMeshProUGUI _QuestBriefDescriptionText;
    [SerializeField] private TextMeshProUGUI _QuestStatusText;

    public void SetQuestButton(QuestSO newQuest)
    {
        _QuestNameText.text = newQuest.Name;
        _QuestBriefDescriptionText.text = newQuest.BriefDescription;
        if (newQuest is MainQuestSO)
        {
            _QuestStatusText.text = newQuest.QuestStatus.ToString();
        }
        else if (newQuest is SideQuestSO)
        {
            if ((newQuest.QuestStatus == QuestSO.Status.Unacquired) ||
                (newQuest.QuestStatus == QuestSO.Status.Expired))
            {
                _QuestStatusText.text = newQuest.QuestStatus.ToString();
            }
            else
            {
                _QuestStatusText.text = ((SideQuestSO)newQuest).GetDaysLeftText();
            }
        }
    }
}
