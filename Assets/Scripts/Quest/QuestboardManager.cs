using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestboardManager : MonoBehaviour
{
    [SerializeField] private GameObject _QuestDetailButtonPrefab;
    [SerializeField] private TextMeshProUGUI _RemainingTimeText;
    [SerializeField] private Transform _UnacquiredQuestHolder;
    [SerializeField] private Transform _AcquiredQuestHolder;

    void Start()
    {
        RefreshQuestboard();
    }

    public void RefreshQuestboard()
    {
        // Refresh remaining time
        int remainingDay = PlayerManager.SideQuestDatabase.DayLeftBeforeNewQuest;
        string suffix = (remainingDay > 1) ? " Days" : " Day";
        _RemainingTimeText.text = remainingDay.ToString() + suffix;
        // Destroy all previous quest button object
        foreach (Transform child in _UnacquiredQuestHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _AcquiredQuestHolder)
        {
            Destroy(child.gameObject);
        }
        // Spawn all quest button
        List<SideQuestSO> allSideQuest = PlayerManager.SideQuestDatabase.GetAllQuest();
        foreach (SideQuestSO sideQuest in allSideQuest)
        {
            Transform placeToSpawn;
            if (sideQuest.QuestStatus == QuestSO.Status.Unacquired)
            {
                placeToSpawn = _UnacquiredQuestHolder;
            }
            else
            {
                placeToSpawn = _AcquiredQuestHolder;
            }
            GameObject newQuestDetailButton = Instantiate(_QuestDetailButtonPrefab, placeToSpawn);
            newQuestDetailButton.GetComponent<QuestDetailButton>().SetQuestButton(sideQuest);
            newQuestDetailButton.GetComponent<Button>().onClick.AddListener(() => _OnClickQuestButton(sideQuest));
        }
    }

    private void _OnClickQuestButton(SideQuestSO clickedQuest)
    {
        SideQuestSubmissionManager.SetCurrentQuest(clickedQuest);
        SceneMng.StaticChangeScene("SideQuestSubmission");
    }
}
