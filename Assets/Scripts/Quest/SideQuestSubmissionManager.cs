using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideQuestSubmissionManager : MonoBehaviour
{
    public static SideQuestSubmissionManager Instance;

    // Quest information
    private static SideQuestSO _CurrentQuest;
    public MechChromoSO SelectedMech { get; private set; }
    public float SimilarityRate { get; private set; }
    // Actual UI element
    [SerializeField] private GameObject _OneQuestDetailPanel;
    [SerializeField] private GameObject _SelectMechButton;
    [SerializeField] private GameObject _SimilarityPanel;
    [SerializeField] private TextMeshProUGUI _SimilarityRateText;
    [SerializeField] private TextMeshProUGUI _ExpectedRewardMoneyText;
    [SerializeField] private GameObject _ConfirmButton;

    public void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Start()
    {
        _OneQuestDetailPanel.GetComponent<QuestDetailOverlay>().SetOverlay(_CurrentQuest);
        RenderPanel();
    }

    #region Setter
    public static void SetCurrentQuest(SideQuestSO newSideQuest)
    {
        _CurrentQuest = newSideQuest;
    }

    public void SetSelectedMech(MechChromoSO newSelectedMech)
    {
        SelectedMech = newSelectedMech;
    }

    public void SetSimilarityRate(float newSimilarityRate)
    {
        SimilarityRate = newSimilarityRate;
        SimilarityRate = (SimilarityRate < 0) ? 0 : SimilarityRate;
        SimilarityRate = (SimilarityRate > 1) ? 1 : SimilarityRate;
    }
    #endregion

    #region UI Rendering
    public void RenderPanel()
    {
        // Set select mech button and similarity rate display only if it submittable (InProgress or Completable)
        if ((_CurrentQuest.QuestStatus != QuestSO.Status.InProgress) &&
            (_CurrentQuest.QuestStatus != QuestSO.Status.Completable))
        {
            _SelectMechButton.GetComponent<Button>().interactable = false;
            _SimilarityPanel.GetComponent<Image>().color = Color.gray;
            _SimilarityRateText.text = "-";
            _ExpectedRewardMoneyText.text = "-";
        }
        else
        {
            _SelectMechButton.GetComponent<Button>().interactable = true;
            _SimilarityPanel.GetComponent<Image>().color = Color.white;
            _SimilarityRateText.text = (SimilarityRate * 100).ToString("F2");
            _ExpectedRewardMoneyText.text = _CurrentQuest.CalculateExpectedRewardMoney().ToString();
        }
        _RenderConfirmButton();
    }

    // Render confirm button's appearance and function according to the quest status
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
        // Set to do nothing if it's already completed, ideally it should be in this condition but just in case
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
            _ConfirmButton.GetComponent<Button>().interactable = (SelectedMech != null) ? true : false;
            _ConfirmButton.GetComponent<Button>().onClick.AddListener(() => {
                _CurrentQuest.CompleteQuest();
                SceneMng.ReturnToPreviousScene();
            });
        }
    }
    #endregion

    public void OnSelectMechClick()
    {
        Debug.Log("Selecting mech to submit on side quest");
        // Opend selection panel and set _SelectedMech to something
        // WIP
        // Don't forget to refresh UI by calling RenderPanel() after _SelectedMech is set
    }
}
