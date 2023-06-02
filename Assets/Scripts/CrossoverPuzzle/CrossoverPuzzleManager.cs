using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrossoverPuzzleManager : MonoBehaviour
{
    public static CrossoverPuzzleManager Instance;
    private int _PuzzleType;
    [SerializeField] private GameObject _OverlayPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Set Parent and Children Manager to suit the type of puzzle
    // puzzleType: 0 = demonstrate single point, 1 = demonstrate two point, 2 = solve single point, 3 = solve two point
    public void SetPuzzle(int puzzleType)
    {
        _PuzzleType = puzzleType;
        ParentManager.Instance.InstaniateChromosomeRodToggles(puzzleType);
        ParentManager.Instance.UnselectAllToggles();
        ChildrenManager.Instance.SetPuzzle(_PuzzleType >= 2);
    }

    // Check the result of solving puzzle
    public void SubmitAnswer()
    {
        bool isCorrect = false;
        // If it's the solving puzzle, check the chromosome value
        if (_PuzzleType >= 2)
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
                    isCorrect = false;
                }
            }
            // If it's single point crossover, the color change through chromosome should be 1
            // If it's two point crossover, the color change through chromosome should be 2
            int wantedChangeCount = (_PuzzleType == 0) ? 1 : 2;
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
                isCorrect = false;
            }
        }
        string feedbackText = isCorrect ? "Correct" : "Wrong";
        GameObject overlay = Instantiate(_OverlayPrefab, this.transform);
        overlay.GetComponent<PuzzleFeedbackOverlay>().SetFeedBack(isCorrect, feedbackText);
    }
}
