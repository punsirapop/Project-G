using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrossoverPuzzleManager : MonoBehaviour
{
    public static CrossoverPuzzleManager Instance;
    private PuzzleType _PuzzleType;
    [SerializeField] private GameObject _OverlayPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        SetPuzzle(PlayerManager.PuzzleToGenerate);
    }

    // Set Parent and Children Manager to suit the type of puzzle
    public void SetPuzzle(PuzzleType puzzleType)
    {
        _PuzzleType = puzzleType;
        ParentManager.Instance.InstaniateChromosomeRodToggles(puzzleType);
        ParentManager.Instance.UnselectAllToggles();
        bool isSolve = ((puzzleType == PuzzleType.CrossoverOnePointSolve) || (puzzleType == PuzzleType.CrossoverTwoPointsSolve));
        ChildrenManager.Instance.SetPuzzle(isSolve);
    }

    // Wrapper-method for debugging in editor
    // puzzleType: 0 = demonstrate single point, 1 = demonstrate two point, 2 = solve single point, 3 = solve two point
    public void SetPuzzle(int puzzleType)
    {
        if (puzzleType == 0) SetPuzzle(PuzzleType.CrossoverOnePointDemon);
        else if (puzzleType == 1) SetPuzzle(PuzzleType.CrossoverTwoPointsDemon);
        else if (puzzleType == 2) SetPuzzle(PuzzleType.CrossoverOnePointSolve);
        else if (puzzleType == 3) SetPuzzle(PuzzleType.CrossoverTwoPointsSolve);
    }

    // Check the result of solving puzzle
    public void SubmitAnswer()
    {
        bool isCorrect = false;
        string feedbackText = "";
        // If it's the solving puzzle, check the chromosome value
        if (_PuzzleType == PuzzleType.CrossoverOnePointSolve ||
            _PuzzleType == PuzzleType.CrossoverTwoPointsSolve)
        {
            int[] wantedChild = ParentManager.Instance.GetWantedChild();
            int[][] bredChildren = ChildrenManager.Instance.GetBredChildren();
            foreach (int[] bredChild in bredChildren)
            {
                isCorrect = true;
                for (int i = 0; i < wantedChild.Length; i++)
                {
                    if (wantedChild[i] != bredChild[i])
                    {
                        isCorrect = false;
                    }
                }
                if (isCorrect)
                {
                    break;
                }
            }
            if (!isCorrect)
            {
                feedbackText += "\n- No wanted child is produced.";
            }
        }
        // If it's the demonstration puzzle
        // 1. Check that the selected chromosome have the same type
        // 2. Check the change in color through single chromosome, this work only on chromosome that is instantiated with single color
        else
        {
            // Check the type of selected chromosomes
            int[] selectedTypes = ParentManager.Instance.GetSelectedRodsType();
            isCorrect = true;
            for (int i = 0; i < selectedTypes.Length - 1; i++)
            {
                if (selectedTypes[i] != selectedTypes[i + 1])
                {
                    feedbackText += "\n- You can't crossover a pair with different types (integer and binary).";
                    isCorrect = false;
                }
            }
            // If it's single point crossover, the color change through chromosome should be 1
            // If it's two point crossover, the color change through chromosome should be 2
            int wantedChangeCount = (_PuzzleType == PuzzleType.CrossoverOnePointDemon) ? 1 : 2;
            int bredChangeCount = 0;
            Color32[] bredColor = ChildrenManager.Instance.GetBredChildColor();
            for (int i = 0; i < bredColor.Length - 1; i++)
            {
                bool isRedChange = (bredColor[i].r != bredColor[i + 1].r);
                bool isGreenChange = (bredColor[i].g != bredColor[i + 1].g);
                bool isBlueChange = (bredColor[i].b != bredColor[i + 1].b);
                bool isAlphaChange = (bredColor[i].a != bredColor[i + 1].a);
                bredChangeCount += (isRedChange || isGreenChange || isBlueChange || isAlphaChange) ? 1 : 0;
            }
            if (wantedChangeCount != bredChangeCount)
            {
                feedbackText += "\n- The number of crossover point(s)/section(s) is wrong.";
                isCorrect = false;
            }
        }
        // Result conclusion
        foreach(string jigsawFeedback in PlayerManager.RecordPuzzleResult(isCorrect))
        {
            feedbackText += "\n" + jigsawFeedback;
        }
        GameObject overlay = Instantiate(_OverlayPrefab, this.transform);
        overlay.GetComponent<PuzzleFeedbackOverlay>().SetFeedBack(
            isPass: isCorrect,
            headerText: isCorrect ? "Success" : "Fail" , 
            feedbackText: feedbackText
            );
    }
}