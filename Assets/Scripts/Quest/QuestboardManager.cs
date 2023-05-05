using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestboardManager : MonoBehaviour
{
    [SerializeField] private GameObject _QuestDetailButtonPrefab;
    [SerializeField] private Transform _UnacquiredQuestHolder;
    [SerializeField] private Transform _AcquiredQuestHolder;

    void Start()
    {
        RefreshQuestboard();
    }

    public void RefreshQuestboard()
    {
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

    // TEMP wrapper function to generate new unacquired side quest
    public void GenerateNewQuest()
    {
        PlayerManager.SideQuestDatabase.ForceGenerateNewQuest();
    }

    private void _OnClickQuestButton(SideQuestSO clickedQuest)
    {
        SideQuestSubmissionManager.SetCurrentQuest(clickedQuest);
        SceneMng.StaticChangeScene("SideQuestSubmission");
    }
}
