using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDetailOverlay : MonoBehaviour
{
    [Header("Basic Information")]
    [SerializeField] private GameObject _UnlockRequirementPrefab;
    [SerializeField] private TextMeshProUGUI _QuestNameText;
    [SerializeField] private TextMeshProUGUI _BriefDesText;
    [SerializeField] private TextMeshProUGUI _FullDesText;
    [SerializeField] private TextMeshProUGUI _DueDateText;
    [Header("Main Quest Progress")]
    [SerializeField] private GameObject _MainQuestProgressHolder;
    [SerializeField] private Transform _ProgressHolder;
    [SerializeField] private TextMeshProUGUI _RewardMoneyText;
    [SerializeField] private TextMeshProUGUI _RewardMechText;
    [Header("Side Quest Progress")]
    [SerializeField] private GameObject _SideQuestProgressHolder;
    [SerializeField] private TextMeshProUGUI _RewardMinMaxMoneyText;
    [SerializeField] private MechCanvasDisplay _MechDisplay;
    [SerializeField] private TextMeshProUGUI _MechDetails;

    public void SetOverlay(QuestSO clickedQuest)
    {
        _QuestNameText.text = clickedQuest.Name;
        _BriefDesText.text = clickedQuest.BriefDescription;
        _FullDesText.text = clickedQuest.FullDescription;
        // Refresh progress if it's main quest
        if (clickedQuest is MainQuestSO)
        {
            MainQuestSO mainQuest = (MainQuestSO)clickedQuest;
            _MainQuestProgressHolder.SetActive(true);
            _SideQuestProgressHolder.SetActive(false);
            // Display progress
            foreach (Transform child in _ProgressHolder)
            {
                Destroy(child.gameObject);
            }
            GameObject newProgress = Instantiate(_UnlockRequirementPrefab, _ProgressHolder);
            newProgress.GetComponent<UnlockRequirementUI>().SetUnlockRequirement(mainQuest.RequireObject.GetUnlockStatus(), Color.black, 24);
            // Display reward
            _RewardMoneyText.text = mainQuest.RewardMoney.ToString();
            _RewardMechText.text = mainQuest.RewardMechs.Length.ToString();
        }
        else if (clickedQuest is SideQuestSO)
        {
            SideQuestSO sideQuest = (SideQuestSO)clickedQuest;
            _MainQuestProgressHolder.SetActive(false);
            _SideQuestProgressHolder.SetActive(true);
            // Display wanted mech
            // do something with sideQuest.WantedMech; here...
            _MechDisplay.SetChromo(sideQuest.WantedMech);
            _MechDetails.text = string.Join("\n\n", $"Head: {sideQuest.WantedMech.Head}",
                "Body: " + string.Join("/",sideQuest.WantedMech.Body),
                $"Acc: {sideQuest.WantedMech.Acc}");
            // Display reward
            _RewardMinMaxMoneyText.text = sideQuest.MinRewardMoney.ToString() + " to " + sideQuest.MaxRewardMoney.ToString();
        }
        // Show due date
        // If there is no due date (all info is 0), show no date
        if (clickedQuest.DueDate.ToDay() == 0)
        {
            _DueDateText.text = "-";
            _DueDateText.color = Color.gray;
        }
        // Expired quest
        else if (clickedQuest.QuestStatus == QuestSO.Status.Expired)
        {
            _DueDateText.text = clickedQuest.DueDate.ShowDate() + " (expired)";
            _DueDateText.color = Color.red;
        }
        // Normal InProgress quest, show date with remaining time
        else
        {
            int dayRemain = clickedQuest.DueDate.CompareDate(PlayerManager.CurrentDate);
            string suffix = (dayRemain <= 1) ? " day left)" : " days left)";
            _DueDateText.text = clickedQuest.DueDate.ShowDate() + " (" + dayRemain.ToString() + suffix;
            _DueDateText.color = Color.black;
        }
    }
}
