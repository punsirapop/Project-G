using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // Quest information
    [SerializeField] private GameObject _UnlockRequirementPrefab;
    private QuestSO _CurrentQuest;

    // Actual UI element for quest list
    [Header("Quest List")]
    [SerializeField] private GameObject _QuestOverlay;
    [SerializeField] private GameObject _QuestListPanel;
    [SerializeField] private Transform _QuestDetailButtonHolder;
    [SerializeField] private GameObject _QuestDetailButtonPrefab;

    // Actual UI element for one quest detail
    [Header("One Quest Detail")]
    [SerializeField] private GameObject _OneQuestDetailPanel;
    [SerializeField] private TextMeshProUGUI _QuestNameText;
    [SerializeField] private TextMeshProUGUI _BriefDesText;
    [SerializeField] private TextMeshProUGUI _FullDesText;
    [SerializeField] private Transform _ProgressHolder;
    [SerializeField] private TextMeshProUGUI _RewardMoneyText;
    [SerializeField] private TextMeshProUGUI _DueDateText;
    [SerializeField] private GameObject _ConfirmButton;


    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        _QuestOverlay.SetActive(false);
    }

    public void OpenQuestOverlay()
    {
        _QuestOverlay.SetActive(true);
        _QuestListPanel.SetActive(true);
        _OneQuestDetailPanel.SetActive(false);
        // Refresh all quest detail button
        foreach (Transform child in _QuestDetailButtonHolder)
        {
            Destroy(child.gameObject);
        }
        // Spawn current main quest
        if (PlayerManager.MainQuestDatabase.GetCurrentQuest() != null)
        {
            GameObject newQuestDetailButton = Instantiate(_QuestDetailButtonPrefab, _QuestDetailButtonHolder);
            newQuestDetailButton.GetComponent<QuestDetailButton>().SetQuestButton(PlayerManager.MainQuestDatabase.GetCurrentQuest());
        }
        ///////////////// looping through all side quests in the future////////////
        //foreach (QuestSO quest in _CurrentQuests)
        //{
        //    GameObject newQuestDetailButton = Instantiate(_QuestDetailButtonPrefab, _QuestDetailButtonHolder);
        //    newQuestDetailButton.GetComponent<QuestDetailButton>().SetQuestButton(quest);
        //}
        /////////////////////////////////////////////////////////////////////
    }

    public void OpenOneQuestDetail(QuestSO clickedQuest)
    {
        _CurrentQuest = clickedQuest;
        _QuestListPanel.SetActive(false);
        _OneQuestDetailPanel.SetActive(true);
        _QuestNameText.text = clickedQuest.Name;
        _BriefDesText.text = clickedQuest.BriefDescription;
        _FullDesText.text = clickedQuest.FullDescription;
        foreach (Transform child in _ProgressHolder)
        {
            Destroy(child.gameObject);
        }
        GameObject newProgress = Instantiate(_UnlockRequirementPrefab, _ProgressHolder);
        newProgress.GetComponent<UnlockRequirementUI>().SetUnlockRequirement(clickedQuest.RequireObject.GetUnlockStatus(), Color.black, 24);
        _RewardMoneyText.text = clickedQuest.RewardMoney.ToString();
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
        // Set confirmation button according to the QuestStatus
        _ConfirmButton.GetComponent<Button>().interactable = true;
        switch (clickedQuest.QuestStatus)
        {
            default:
                _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "-";
                _ConfirmButton.GetComponent<Button>().interactable = false;
                break;
            case QuestSO.Status.Unacquired:
                _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Recieve";
                break;
            case QuestSO.Status.InProgress:
                _ConfirmButton.GetComponent<Button>().interactable = false;
                _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "In Progress";
                break;
            case QuestSO.Status.Completable:
                _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Complete";
                break;
            case QuestSO.Status.Expired:
                _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Abandon";
                break;
        }
    }

    public void OnConfirmClick()
    {
        switch (_CurrentQuest.QuestStatus)
        {
            // Recieving quest
            case QuestSO.Status.Unacquired:
                _CurrentQuest.SetStatus(QuestSO.Status.InProgress);
                PlayerManager.ValidateUnlocking();
                if (_CurrentQuest.IntroDialogue != null)
                {
                    PlayerManager.SetCurrentDialogue(_CurrentQuest.IntroDialogue);
                    SceneMng.StaticChangeScene("Cutscene");
                }
                else
                {
                    OpenQuestOverlay();
                }
                break;
            // Submit completable quest
            case QuestSO.Status.Completable:
                // Give reward
                bool isTransactionSuccess = PlayerManager.GainMoneyIfValid(_CurrentQuest.RewardMoney);
                if (!isTransactionSuccess)
                {
                    return;
                }
                // Give Mech here...
                _CurrentQuest.SetStatus(QuestSO.Status.Completed);
                PlayerManager.ValidateUnlocking();
                if (_CurrentQuest.OutroDialogue != null)
                {
                    PlayerManager.SetCurrentDialogue(_CurrentQuest.OutroDialogue);
                    SceneMng.StaticChangeScene("Cutscene");
                }
                else
                {
                    OpenQuestOverlay();
                }
                break;
            // Submit expired quest
            case QuestSO.Status.Expired:
                _CurrentQuest.SetStatus(QuestSO.Status.Completed);
                PlayerManager.ValidateUnlocking();
                break;
        }
    }
}
