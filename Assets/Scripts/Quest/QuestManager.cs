using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // Quest information
    //[SerializeField] private GameObject _UnlockRequirementPrefab;
    private QuestSO _CurrentClickedQuest;

    // Actual UI element for quest list
    [Header("Quest List")]
    [SerializeField] private GameObject _QuestOverlay;
    [SerializeField] private GameObject _QuestListPanel;
    [SerializeField] private Transform _QuestDetailButtonHolder;
    [SerializeField] private GameObject _QuestDetailButtonPrefab;

    // Actual UI element for one quest detail
    [Header("One Quest Detail")]
    [SerializeField] private GameObject _OneQuestDetailPanel;
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
        MainQuestSO currentMainQuest = PlayerManager.MainQuestDatabase.GetCurrentQuest();
        if (currentMainQuest != null)
        {
            GameObject newQuestDetailButton = Instantiate(_QuestDetailButtonPrefab, _QuestDetailButtonHolder);
            newQuestDetailButton.GetComponent<QuestDetailButton>().SetQuestButton(currentMainQuest);
            newQuestDetailButton.GetComponent<Button>().onClick.AddListener(() => QuestManager.Instance.OpenOneQuestDetail(currentMainQuest));
        }
        // Spawn all side quest
        foreach (QuestSO sideQuest in PlayerManager.SideQuestDatabase.GetAllAcquiredQuest())
        {
            GameObject newQuestDetailButton = Instantiate(_QuestDetailButtonPrefab, _QuestDetailButtonHolder);
            newQuestDetailButton.GetComponent<QuestDetailButton>().SetQuestButton(sideQuest);
            newQuestDetailButton.GetComponent<Button>().onClick.AddListener(() => QuestManager.Instance.OpenOneQuestDetail(sideQuest));
        }
    }

    // Show the detail of the clicked quest
    public void OpenOneQuestDetail(QuestSO clickedQuest)
    {
        _CurrentClickedQuest = clickedQuest;
        _QuestListPanel.SetActive(false);
        _OneQuestDetailPanel.SetActive(true);
        _OneQuestDetailPanel.GetComponent<QuestDetailOverlay>().SetOverlay(clickedQuest);
        // Set confirmation button according to the QuestStatus
        _ConfirmButton.GetComponent<Button>().interactable = true;
        if (clickedQuest is MainQuestSO)
        {
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
        else if (clickedQuest is SideQuestSO)
        {
            _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go To Submission";
        }
    }

    // Manipulate the quest upon clicking confirm button in one quest detail page
    // Doing thing like receive, complete or abandom quest
    public void OnConfirmClick()
    {
        if (_CurrentClickedQuest is MainQuestSO)
        {
            switch (_CurrentClickedQuest.QuestStatus)
            {
                // Recieving quest
                case QuestSO.Status.Unacquired:
                    _CurrentClickedQuest.ReceiveQuest();
                    OpenQuestOverlay();
                    break;
                // Submit completable quest
                case QuestSO.Status.Completable:
                    _CurrentClickedQuest.CompleteQuest();
                    OpenQuestOverlay();
                    break;
                // Submit expired quest
                case QuestSO.Status.Expired:
                    _CurrentClickedQuest.SetStatus(QuestSO.Status.Completed);
                    PlayerManager.ValidateUnlocking();
                    break;
            }
        }
        else if (_CurrentClickedQuest is SideQuestSO)
        {
            SideQuestSubmissionManager.SetCurrentQuest((SideQuestSO)_CurrentClickedQuest);
            SceneMng.StaticChangeScene("SideQuestSubmission");
        }
    }
}
