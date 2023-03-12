using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionButtonManager : MonoBehaviour
{
    public static SelectionButtonManager Instance;

    [SerializeField] private GameObject _GroupButton;
    [SerializeField] private GameObject _GroupButtonLock;
    [SerializeField] private GameObject _ChanceButton;
    [SerializeField] private GameObject _ChanceButtonLock;
    [SerializeField] private GameObject _RankButton;
    [SerializeField] private GameObject _RankButtonLock;
    [SerializeField] private GameObject _WheelButton;
    [SerializeField] private GameObject _WheelButtonLock;
    [SerializeField] private GameObject _InverseFitnessButton;
    [SerializeField] private GameObject _InverseFitnessButtonLock;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _GroupButton.GetComponentInChildren<Button>().onClick.AddListener(() => SetButtons());
        _ChanceButton.GetComponentInChildren<Button>().onClick.AddListener(() => SetButtons());
        _RankButton.GetComponentInChildren<Button>().onClick.AddListener(() => SetButtons());
        _WheelButton.GetComponentInChildren<Button>().onClick.AddListener(() => SetButtons());
        _InverseFitnessButton.GetComponentInChildren<Button>().onClick.AddListener(() => SetButtons());
    }

    // Control the interactable of all buttons to the valid operation only
    public void SetButtons()
    {
        CandidateManager.Operation operation = CandidateManager.Instance.GetLastOperation();
        // Wheel creation available only after clicking the chance button
        _WheelButton.GetComponentInChildren<Button>().interactable = (operation == CandidateManager.Operation.Chance) ? true : false;
        _InverseFitnessButton.GetComponentInChildren<Button>().interactable = (operation == CandidateManager.Operation.SetAllRank) ? true : false;
    }

    // Lock unnecessary buttons coresponding to the puzzleType
    public void LockButtons(int puzzleType)
    {
        // Tournament-based
        if (puzzleType == 0)
        {
            _GroupButtonLock.SetActive(false);
            _ChanceButtonLock.SetActive(true);
            _RankButtonLock.SetActive(true);
            _WheelButtonLock.SetActive(true);
            _InverseFitnessButtonLock.SetActive(true);
        }
        // Roulette wheel
        else if (puzzleType == 1)
        {
            _GroupButtonLock.SetActive(true);
            _ChanceButtonLock.SetActive(false);
            _RankButtonLock.SetActive(true);
            _WheelButtonLock.SetActive(false);
            _InverseFitnessButtonLock.SetActive(true);
        }
        // Rank-based
        else if (puzzleType == 2)
        {
            _GroupButtonLock.SetActive(true);
            _ChanceButtonLock.SetActive(false);
            _RankButtonLock.SetActive(false);
            _WheelButtonLock.SetActive(false);
            _InverseFitnessButtonLock.SetActive(false);
        }
        // Solving puzzle
        else
        {
            _GroupButtonLock.SetActive(false);
            _ChanceButtonLock.SetActive(false);
            _RankButtonLock.SetActive(false);
            _WheelButtonLock.SetActive(false);
            _InverseFitnessButtonLock.SetActive(false);
        }
    }
}
