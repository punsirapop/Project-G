using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideQuestSubmissionManager : MonoBehaviour
{
    // Quest information
    private static SideQuestSO _CurrentQuest;
    private MechChromoSO _SelectedMech;
    private float _SimilarityRate;
    // Actual UI element
    [SerializeField] private GameObject _OneQuestDetailPanel;
    [SerializeField] private GameObject _SelectMechButton;
    [SerializeField] private GameObject _SimilarityPanel;
    [SerializeField] private TextMeshProUGUI _SimilarityRateText;
    [SerializeField] private GameObject _ConfirmButton;

    public void Start()
    {
        _OneQuestDetailPanel.GetComponent<QuestDetailOverlay>().SetOverlay(_CurrentQuest);
        RenderPanel();
    }

    public static void SetCurrentQuest(SideQuestSO newSideQuest)
    {
        _CurrentQuest = newSideQuest;
    }

    public void RenderPanel()
    {
        Debug.Log("Side quest status = " + _CurrentQuest.QuestStatus.ToString());
        // Set select mech button and similarity rate display only if it submittable (InProgress or Completable)
        if ((_CurrentQuest.QuestStatus != QuestSO.Status.InProgress) &&
            (_CurrentQuest.QuestStatus != QuestSO.Status.Completable))
        {
            Debug.Log("Render can't submit");
            _SelectMechButton.GetComponent<Button>().interactable = false;
            _SimilarityPanel.GetComponent<Image>().color = Color.gray;
            _SimilarityRateText.text = "-";
        }
        else
        {
            Debug.Log("Render can submit");
            _SelectMechButton.GetComponent<Button>().interactable = true;
            _SimilarityPanel.GetComponent<Image>().color = Color.white;
            _SimilarityRateText.text = _SimilarityRate.ToString("F2");
        }
        _RenderConfirmButton();
    }

    // Render confirm button according to the quest status
    private void _RenderConfirmButton()
    {
        // Set confirm button to accept quest in case if it's unacquired
        if (_CurrentQuest.QuestStatus == QuestSO.Status.Unacquired)
        {
            _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Accept";
            _ConfirmButton.GetComponent<Button>().interactable = true;
            _ConfirmButton.GetComponent<Button>().onClick.AddListener(() => {
                _CurrentQuest.ReceiveQuest();
                SceneMng.ReturnToPreviousScene();
            });
        }
        // Set to abandon quest if it's expired
        else if (_CurrentQuest.QuestStatus == QuestSO.Status.Expired)
        {
            _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Abandon";
            _ConfirmButton.GetComponent<Button>().interactable = true;
            _ConfirmButton.GetComponent<Button>().onClick.AddListener(() => {
                _CurrentQuest.CompleteQuest();
                SceneMng.ReturnToPreviousScene();
            });
        }
        // Set to do nothing if it's already completed
        else if (_CurrentQuest.QuestStatus == QuestSO.Status.Completed)
        {
            _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "-";
            _ConfirmButton.GetComponent<Button>().interactable = false;
        }
        // If it's InProgress or Completable, submit the mech and complete the quest
        else
        {
            _ConfirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Submit";
            // The button is interactable only if there is some mech selected
            _ConfirmButton.GetComponent<Button>().interactable = (_SelectedMech != null) ? true : false;
            _ConfirmButton.GetComponent<Button>().onClick.AddListener(() => {
                _CurrentQuest.CompleteQuest();
                SceneMng.ReturnToPreviousScene();
            });
        }
    }

    public void OnSelectMechClick()
    {
        // Opend selection panel and set _SelectedMech to something
        Debug.Log("Selecting mech to submit on side quest");
    }
}
