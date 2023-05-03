using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // Quest database
    [SerializeField] private QuestSO[] _CurrentQuests;

    // Actual UI element for quest list
    [SerializeField] private GameObject _QuestOverlay;
    [SerializeField] private GameObject _QuestListPanel;
    [SerializeField] private Transform _QuestDetailButtonHolder;
    [SerializeField] private GameObject _QuestDetailButtonPrefab;

    // Actual UI element for one quest detail
    [SerializeField] private GameObject _OneQuestDetailPanel;

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
        ///////////////// looping through all quest in the future////////////
        foreach (QuestSO quest in _CurrentQuests)
        {
            GameObject newQuestDetailButton = Instantiate(_QuestDetailButtonPrefab, _QuestDetailButtonHolder);
            newQuestDetailButton.GetComponent<QuestDetailButton>().SetQuestButton(quest);
        }
        /////////////////////////////////////////////////////////////////////
    }

    public void OpenOneQuestDetail(QuestSO clickedQuest)
    {
        _QuestOverlay.SetActive(true);
        _QuestListPanel.SetActive(false);
        _OneQuestDetailPanel.SetActive(true);
    }
}
