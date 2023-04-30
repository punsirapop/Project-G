using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KnapsackPuzzleManager : MonoBehaviour
{
    public static KnapsackPuzzleManager Instance;

    // Data of all factory
    public FactorySO[] FactoriesData => PlayerManager.FactoryDatabase;
    [SerializeField] private GameObject _OverlayPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (PlayerManager.FixingFacility)
        {
            SetPuzzle(PlayerManager.IsSolveFixFactory, PlayerManager.FacilityToFixIndex);
        }
        else
        {
            SetPuzzle(PlayerManager.PuzzleToGenerate);
        }
    }

    // Set Pheonotype and Genotype managers to suit the type of puzzle
    public void SetPuzzle(bool isSolve, int factoryIndex)
    {
        PhenotypeManager.Instance.ResetObject();
        GenotypeManager.Instance.ResetObject();
        // Get chromosome from factory to generate puzzle
        int[][] bitstring;
        if (PlayerManager.FixingFacility)
        {
            bitstring = FactoriesData[PlayerManager.FacilityToFixIndex].GetBestBitstring();
        }
        else
        {
           bitstring = FactoriesData[factoryIndex].GetRandomValidBitstring();
        }
        //string logText = "Generate puzzle using bitstring: ";
        //foreach (int bit in bitstring[0])
        //{
        //    logText += bit.ToString();
        //}
        //Debug.Log(logText);
        // Encoding puzzle, set Phenotype, player interact with Genotype
        if (FactoriesData[factoryIndex].GetIsEncode())
        {
            PhenotypeManager.Instance.InstantiateKnapsacks(factoryIndex);
            PhenotypeManager.Instance.InstantiateItems(factoryIndex, bitstring);
            PhenotypeManager.Instance.SetPresetChangable(false);
            PhenotypeManager.Instance.SetResettable(false);
            // Solve
            if (isSolve)
            {
                GenotypeManager.Instance.CreateMapMask(FactoriesData[factoryIndex].Items.Length, 0.7f);
                GenotypeManager.Instance.InstantiateBitBlocks();
                GenotypeManager.Instance.SetPresetChangable(true);
            }
            // Demonstrate
            else
            {
                GenotypeManager.Instance.CreateMapMask(FactoriesData[factoryIndex].Items.Length, 1f);
                GenotypeManager.Instance.InstantiateBitBlocks(factoryIndex);
                GenotypeManager.Instance.SetPresetChangable(false);
            }
            GenotypeManager.Instance.SetResettable(true);
        }
        // Decoding puzzle, set Genotype, player interact with Phenotype
        else
        {
            GenotypeManager.Instance.CreateMapMask(FactoriesData[factoryIndex].Items.Length, 1f);
            GenotypeManager.Instance.InstantiateBitBlocks(factoryIndex, bitstring);
            GenotypeManager.Instance.SetPresetChangable(false);
            GenotypeManager.Instance.SetResettable(false);
            // Solve
            if (isSolve)
            {
                PhenotypeManager.Instance.InstantiateKnapsacks();
                PhenotypeManager.Instance.InstantiateItems();
                PhenotypeManager.Instance.SetPresetChangable(true);
            }
            // Demonstrate
            else
            {
                PhenotypeManager.Instance.InstantiateKnapsacks(factoryIndex);
                PhenotypeManager.Instance.InstantiateItems(factoryIndex);
                PhenotypeManager.Instance.SetPresetChangable(false);
            }
            PhenotypeManager.Instance.SetResettable(true);
        }
        PhenotypeManager.Instance.EnableSettingOnBitblock();
    }

    // Overloading method for set the puzzle used for testing propose, using with OnClick() of button on editor
    // puzzleType:
    //      0 = demonstrate decoding
    //      1 = demonstrate encoding
    //      2 = solve decoding
    //      3 = solve encoding
    //public void SetPuzzle(int puzzleType=0)
    //{
    //    switch(puzzleType)
    //    {
    //        case 0:
    //            SetPuzzle(false, false);
    //            break;
    //        case 1:
    //            SetPuzzle(false, true);
    //            break;
    //        case 2:
    //            SetPuzzle(true, false);
    //            break;
    //        case 3:
    //            SetPuzzle(true, true);
    //            break;
    //    }
    //}

    // Overload method for set the puzzle using PuzzleType
    public void SetPuzzle(PuzzleType puzzleType)
    {
        if (puzzleType == PuzzleType.KnapsackStandardDemon)
        {
            SetPuzzle(false, 0);
        }
        else if (puzzleType == PuzzleType.KnapsackStandardSolve)
        {
            SetPuzzle(true, 0);
        }
        else if (puzzleType == PuzzleType.KnapsackMultiDimenDemon)
        {
            SetPuzzle(false, 1);
        }
        else if (puzzleType == PuzzleType.KnapsackMultiDimenSolve)
        {
            SetPuzzle(true, 1);
        }
        else if (puzzleType == PuzzleType.KnapsackMultipleDemon)
        {
            SetPuzzle(false, 2);
        }
        else if (puzzleType == PuzzleType.KnapsackMultipleSolve)
        {
            SetPuzzle(true, 2);
        }
    }

    // Calculate factory index to get bitstring from
    private int _CalculateFactoryIndex(PuzzleType puzzleType)
    {
        if ((puzzleType == PuzzleType.KnapsackStandardDemon) ||
            puzzleType == PuzzleType.KnapsackStandardSolve)
        {
            return 0;   // First factory
        }
        else if ((puzzleType == PuzzleType.KnapsackMultiDimenDemon) ||
            puzzleType == PuzzleType.KnapsackMultiDimenSolve)
        {
            return 1;   // Second factory
        }
        else if ((puzzleType == PuzzleType.KnapsackMultipleDemon) ||
            puzzleType == PuzzleType.KnapsackMultipleSolve)
        {
            return 2;   // Third factory
        }
        else
        {
            // If there is nothing wrong, it shouldn't go into this else line...
            return 3;
        }
    }

    public void SubmitAnswer()
    {
        string genoAnswer = GenotypeManager.Instance.ToString();
        string phenoAnswer = PhenotypeManager.Instance.ToString();
        bool isCorrect = (genoAnswer == phenoAnswer);
        string feedbackText = "";
        // Result conclusion
        int[] amountAndMoney = PlayerManager.CountJigsawPieceProgress(isCorrect);
        foreach (string jigsawFeedback in PlayerManager.GenerateJigsawFeedback(amountAndMoney))
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
}
