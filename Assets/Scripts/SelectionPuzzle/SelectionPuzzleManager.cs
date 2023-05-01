using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionPuzzleManager : MonoBehaviour
{
    public static SelectionPuzzleManager Instance;
    private PuzzleType _PuzzleType;
    [SerializeField] private TextMeshProUGUI _InstructionText;
    [SerializeField] private GameObject _OverlayPrefab;

    private void Start()
    {
        SetPuzzle(PlayerManager.PuzzleToGenerate);
    }

    // Set puzzle scence corresponding to the puzzleType
    // puzzleType:  0 = demonstrate tournament-based, 1 = demonstrate roulette wheel, 2 = demonstrate rank-based
    //              3 = solve tournament-based, 4 = solve roulette wheel, 5 = solve solve rank-based
    public void SetPuzzle(PuzzleType puzzleType)
    {
        _PuzzleType = puzzleType;
        PopulationManager.Instance.CreatePopulation();
        CandidateManager.Instance.ClearPanel();
        CandidateManager.Instance.ClearLog();
        SelectionButtonManager.Instance.SetButtons();
        SelectionButtonManager.Instance.LockButtons(puzzleType);
        SelectedParentManager.Instance.ClearPanel();
        if (puzzleType == PuzzleType.SelectionTournamentDemon)
        {
            _InstructionText.text = "Please demonstrate the Tournament-based Selection.";
        }
        else if (puzzleType == PuzzleType.SelectionRouletteDemon)
        {
            _InstructionText.text = "Please demonstrate the Roulette Wheel Selection.";
        }
        else if (puzzleType == PuzzleType.SelectionRankDemon)
        {
            _InstructionText.text = "Please demonstrate the Rank-based Selection.";
        }
        else if (puzzleType == PuzzleType.SelectionTournamentSolve)
        {
            _InstructionText.text = "Please select the parent using the method that repeatedly divides the population into small groups.";
        }
        else if (puzzleType == PuzzleType.SelectionRouletteSolve)
        {
            _InstructionText.text = "Please select the parent using the method that the chance to be chosen directly depends on the current fitness value.";
        }
        else if (puzzleType == PuzzleType.SelectionRankSolve)
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
        if ((_PuzzleType == PuzzleType.SelectionTournamentDemon) ||
            (_PuzzleType == PuzzleType.SelectionTournamentSolve))
        {
            correctOperations = CandidateManager.Instance.TournamentOperations;
            if (playerOperations.Contains(CandidateManager.Operation.PickNotBestInGroup))
            {
                isCorrect = false;
                feedbackText += "\n- You pick a chromosome with not the best fitness value in a group.";
            }
            if (_IsPickTwiceInGroup(playerOperations))
            {
                isCorrect = false;
                feedbackText += "\n- You pick the chromosome from the same group twice.";
            }
        }
        // Roulette wheel demon and solve
        else if ((_PuzzleType == PuzzleType.SelectionRouletteDemon) ||
            (_PuzzleType == PuzzleType.SelectionRouletteSolve))
        {
            correctOperations = CandidateManager.Instance.RouletteWheelOperations;
            if (playerOperations.Contains(CandidateManager.Operation.PickInChance))
            {
                isCorrect = false;
                feedbackText += "\n- You should create a wheel from chance, not directly pick it.";
            }
        }
        // Rank demon and solve
        else
        {
            correctOperations = CandidateManager.Instance.RankOperations;
            int[] properRank = PopulationManager.Instance.GetProperRank();
            int[] assignedRank = CandidateManager.Instance.AssignedRank;
            bool isRankCorrect = _IsArrayEqual(properRank, assignedRank);
            if (!isRankCorrect)
            {
                isCorrect = false;
                feedbackText += "\n- Your chromosome ranking is wrong.";
            }
            if (playerOperations.Contains(CandidateManager.Operation.PickInChance))
            {
                isCorrect = false;
                feedbackText += "\n- You should create a wheel from chance, not directly pick it.";
            }
        }
        // Checking wheter the player play as preferred operations
        if (playerOperations.Count != correctOperations.Count)
        {
            isCorrect = false;
            feedbackText += "\n- You may select more or fewer chromosomes than the current population.";
        }
        else if (isCorrect)
        {
            for (int i = 0; i < playerOperations.Count; i++)
            {
                if (playerOperations[i] != correctOperations[i])
                {
                    isCorrect = false;
                    feedbackText += "\n- You perform some steps wrongly.";
                    break;
                }
            }
        }
        // Show the name of selection type if they wrong in solving puzzle
        if (!isCorrect)
        {
            if (_PuzzleType == PuzzleType.SelectionTournamentSolve)
            {
                feedbackText += "\n- You perform Tournament-based Selection wrongly.";
            }
            else if (_PuzzleType == PuzzleType.SelectionRouletteSolve)
            {
                feedbackText += "\n- You perform Roulette Wheel Selection wrongly.";
            }
            else if (_PuzzleType == PuzzleType.SelectionRankSolve)
            {
                feedbackText += "\n- You perform Rank-based Selection wrongly.";
            }
        }
        // Result conclusion
        foreach (string jigsawFeedback in PlayerManager.RecordPuzzleResult(isCorrect))
        {
            feedbackText += "\n" + jigsawFeedback;
        }
        GameObject overlay = Instantiate(_OverlayPrefab, this.transform);
        overlay.GetComponent<PuzzleFeedbackOverlay>().SetFeedBack(
            isPass: isCorrect,
            headerText: isCorrect ? "Success" : "Fail",
            feedbackText: feedbackText
            );
    }

    private bool _IsArrayEqual(int[] array1, int[] array2)
    {
        if ((array1 == null) || (array2 == null))
        {
            return false;
        }
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

    // Check whether there is 2 consequetive <pick in group operation> in the given list
    private bool _IsPickTwiceInGroup(List<CandidateManager.Operation> playerOperations)
    {
        for (int i = 0; i < playerOperations.Count - 1; i++)
        {
            bool bestBest = (
                (playerOperations[i] == CandidateManager.Operation.PickBestInGroup) &&
                (playerOperations[i + 1] == CandidateManager.Operation.PickBestInGroup)
                );
           bool bestNot = (
                (playerOperations[i] == CandidateManager.Operation.PickBestInGroup) &&
                (playerOperations[i + 1] == CandidateManager.Operation.PickNotBestInGroup)
                );
            bool NotBest = (
                (playerOperations[i] == CandidateManager.Operation.PickNotBestInGroup) &&
                (playerOperations[i + 1] == CandidateManager.Operation.PickBestInGroup)
                );
            bool NotNot = (
                (playerOperations[i] == CandidateManager.Operation.PickNotBestInGroup) &&
                (playerOperations[i + 1] == CandidateManager.Operation.PickNotBestInGroup)
                );
            if (bestBest || bestNot || NotBest || NotNot)
            {
                return true;
            }
        }
        return false;
    }
}
