using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressRenderer : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _DateLayoutGroup;
    [SerializeField] private HorizontalLayoutGroup _JigsawLayoutGroup;
    [SerializeField] private TextMeshProUGUI _DateText;
    [SerializeField] private TextMeshProUGUI _QuestText;
    [SerializeField] private TextMeshProUGUI _BronzeJigsawText;
    [SerializeField] private TextMeshProUGUI _SilverJigsawText;
    [SerializeField] private TextMeshProUGUI _GoldJigsawText;

    public void RenderProgressPanel()
    {
        // Show date and time passed since the beginning
        TimeManager.Date startDate = new TimeManager.Date();
        startDate.InitDate();
        int dayPassed = PlayerManager.CurrentDate.CompareDate(startDate);
        string dayPrefix = (dayPassed > 1) ? " days passed)" : " day passed)";
        _DateText.text = PlayerManager.CurrentDate.ShowDate() + " (" + dayPassed.ToString() + dayPrefix;
        // Show current main quest in progress
        MainQuestSO currentQuest = PlayerManager.MainQuestDatabase.GetQuestProgress();
        string questProgress = currentQuest.Name + " (" + currentQuest.QuestStatus.ToString() + ")";
        // Highlight the text color according to the quest status
        Color questTextColor = new Color();
        if ((currentQuest.QuestStatus == QuestSO.Status.Unacquired) ||
            (currentQuest.QuestStatus == QuestSO.Status.Expired))
        {
            questTextColor = Color.red;
        }
        else if ((currentQuest.QuestStatus == QuestSO.Status.InProgress) ||
            (currentQuest.QuestStatus == QuestSO.Status.Completable))
        {
            questTextColor = Color.green;
        }
        else if (currentQuest.QuestStatus == QuestSO.Status.Completed)
        {
            questTextColor = Color.yellow;
        }
        _QuestText.text = questProgress;
        _QuestText.color = questTextColor;
        // Show the jigsaw collection progress
        int bronzeSuccess = 0;
        int bronzeFail = 0;
        int silverSuccess = 0;
        int silverFail = 0;
        int goldSuccess = 0;
        int goldFail = 0;
        foreach (JigsawTraySO tray in HallOfFameManager.Instance.AllJigsawTray)
        {
            foreach (JigsawPieceGroupSO group in tray.AllJigsawPieceGroup)
            {
                foreach (JigsawPieceSO piece in group.JigsawPieces)
                {
                    switch (piece.Level)
                    {
                        case JigsawLevel.Copper:
                            bronzeSuccess += piece.SuccessCount;
                            bronzeFail += piece.FailCount;
                            break;
                        case JigsawLevel.Silver:
                            silverSuccess += piece.SuccessCount;
                            silverFail += piece.FailCount;
                            break;
                        case JigsawLevel.Gold:
                            goldSuccess += piece.SuccessCount;
                            goldFail += piece.FailCount;
                            break;
                    }
                }
            }
        }
        _BronzeJigsawText.text = bronzeSuccess.ToString() + "/" + bronzeFail.ToString();
        _SilverJigsawText.text = silverSuccess.ToString() + "/" + silverFail.ToString();
        _GoldJigsawText.text = goldSuccess.ToString() + "/" + goldFail.ToString();
        // Re-align elements in the progress layout by re-enable the layout group
        //_DateLayoutGroup.enabled = true;
        //_DateLayoutGroup.enabled = false;
        //_DateLayoutGroup.enabled = true;
        //_JigsawLayoutGroup.enabled = true;
        //_JigsawLayoutGroup.enabled = false;
        //_JigsawLayoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_DateLayoutGroup.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(_JigsawLayoutGroup.GetComponent<RectTransform>());
    }
}
