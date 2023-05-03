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
        _QuestStatusText.text = newQuest.QuestStatus.ToString();
        GetComponent<Button>().onClick.AddListener(() => QuestManager.Instance.OpenOneQuestDetail(newQuest));
    }
}
