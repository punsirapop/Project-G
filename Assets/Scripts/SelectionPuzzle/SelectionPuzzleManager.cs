using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionPuzzleManager : MonoBehaviour
{
    public static SelectionPuzzleManager Instance;
    private int _PuzzleType;
    [SerializeField] private TextMeshProUGUI _InstructionText;
    [SerializeField] private GameObject _OverlayPrefab;

    // Set puzzle scence corresponding to the puzzleType
    // puzzleType:  0 = demonstrate tournament-based, 1 = demonstrate roulette wheel, 2 = demonstrate rank-based
    //              3 = solve tournament-based, 4 = solve roulette wheel, 5 = solve solve rank-based
    public void SetPuzzle(int puzzleType)
    {
        _PuzzleType = puzzleType;
        PopulationManager.Instance.CreatePopulation();
        CandidateManager.Instance.ClearPanel();
        CandidateManager.Instance.ClearLog();
        SelectionButtonManager.Instance.SetButtons();
        SelectionButtonManager.Instance.LockButtons(puzzleType);
        SelectedParentManager.Instance.ClearPanel();
        if (puzzleType == 0)
        {
            _InstructionText.text = "Please demonstrate the Tournament-based Selection.";
        }
        else if (puzzleType == 1)
        {
            _InstructionText.text = "Please demonstrate the Roulette Wheel Selection.";
        }
        else if (puzzleType == 2)
        {
            _InstructionText.text = "Please demonstrate the Rank-based Selection.";
        }
        else if (puzzleType == 3)
        {
            _InstructionText.text = "Please select the parent using the method that repeatedly divides the population into small groups.";
        }
        else if (puzzleType == 4)
        {
            _InstructionText.text = "Please select the parent using the method that the chance to be chosen directly depends on the current fitness value.";
        }
        else if (puzzleType == 5)
        {
            _InstructionText.text = "Please select the parent using the method that the chance to be chosen depends on the ranking of the chromosome's fitness value.";
        }
    }

    public void SubmitAnswer()
    {
        List<CandidateManager.Operation> playerOperations = CandidateManager.Instance.OperationLog;
        List<CandidateManager.Operation> correctOperations;
        bool isCorrect = true;
        string feedbackText = "";
        // Tournament demon and solve
        if (_PuzzleType % 3 == 0)
        {
            //feedbackText += "Check for tournament";
            correctOperations = CandidateManager.Instance.TournamentOperations;
        }
        // Roulette wheel demon and solve
        else if (_PuzzleType % 3 == 1)
        {
            //feedbackText += "Check for roulette";
            correctOperations = CandidateManager.Instance.RouletteWheelOperations;
        }
        // Rank demon and solve
        else
        {
            //feedbackText += "Check for rank";
            correctOperations = CandidateManager.Instance.RankOperations;
            int[] properRank = PopulationManager.Instance.GetProperRank();
            int[] assignedRank = CandidateManager.Instance.AssignedRank;
            isCorrect = _IsArrayEqual(properRank, assignedRank);
            if (!isCorrect)
            {
                Debug.Log("Wrong ranking");
            }
        }
        // Checking wheter the player play as preferred operations
        if (playerOperations.Count != correctOperations.Count)
        {
            //feedbackText += "\nLenght not match";
            isCorrect = false;
        }
        else
        {
            for (int i = 0; i < playerOperations.Count; i++)
            {
                if (playerOperations[i] != correctOperations[i])
                {
                    //feedbackText += "\nOperation " + i.ToString() + " not match";
                    isCorrect = false;
                }
            }
        }
        // Show feedback
        feedbackText += isCorrect ? "Correct" : "Wrong";
        GameObject overlay = Instantiate(_OverlayPrefab, this.transform);
        overlay.GetComponent<PuzzleFeedbackOverlay>().SetFeedBack(isCorrect, feedbackText);
    }

    private bool _IsArrayEqual(int[] array1, int[] array2)
    {
        if (array1.Length != array2.Length)
        {
            return false;
        }
        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
            {
                return false;
            }
        }
        return true;
    }
}
