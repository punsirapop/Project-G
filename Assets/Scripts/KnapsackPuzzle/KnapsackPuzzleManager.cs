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

    private bool _IsEncode;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // Check for special case of fixing factory 4, which is the only puzzle scene that involve more than one JigsawPieceSO
        if (PlayerManager.IsFixingLastFactory)
        {
            bool isSolve = (
                (PlayerManager.PuzzleToGenerate == PuzzleType.KnapsackMultiDimenSolve) ||
                (PlayerManager.PuzzleToGenerate == PuzzleType.KnapsackMultipleSolve)
                );
            Debug.Log("Fixing for last factory");
            SetPuzzle(isSolve, 3);  // change this later
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
        _IsEncode = FactoriesData[factoryIndex].GetIsEncode();
        if (_IsEncode)
        {
            PhenotypeManager.Instance.InstantiateKnapsacks(factoryIndex);
            PhenotypeManager.Instance.InstantiateItems(factoryIndex, bitstring);
            PhenotypeManager.Instance.SetPresetChangable(false);
            PhenotypeManager.Instance.SetResettable(false);
            // Solve
            if (isSolve)
            {
                GenotypeManager.Instance.CreateNotMapMask(factoryIndex, 3);
                GenotypeManager.Instance.InstantiateBitBlocks();
                GenotypeManager.Instance.SetPresetChangable(true);
            }
            // Demonstrate
            else
            {
                GenotypeManager.Instance.CreateNotMapMask(factoryIndex, 0);
                GenotypeManager.Instance.InstantiateBitBlocks(factoryIndex);
                GenotypeManager.Instance.SetPresetChangable(false);
            }
            GenotypeManager.Instance.SetResettable(true);
        }
        // Decoding puzzle, set Genotype, player interact with Phenotype
        else
        {
            GenotypeManager.Instance.CreateNotMapMask(factoryIndex, 0);
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

    public void SubmitAnswer()
    {
        string genoAnswer = GenotypeManager.Instance.ToString();
        string phenoAnswer = PhenotypeManager.Instance.ToString();
        bool isCorrect = (genoAnswer == phenoAnswer);
        string feedbackText = "";
        if (phenoAnswer == "dim")
        {
            feedbackText += "\n- The dimension (number of weights) of the knapsack and item is not matched.";
        }
        else
        {
            if (genoAnswer.Length != phenoAnswer.Length)
            {
                feedbackText += "\n- The number of knapsacks does not match the length of the chromosome.";
            }
            else
            {
                string[] splitedGeno = genoAnswer.Split("_");
                string[] splitedPheno = phenoAnswer.Split("_");
                for (int i = 0; i < splitedGeno.Length; i++)
                {
                    string[] ItemKnapBitGeno = splitedGeno[i].Split("/");
                    string[] ItemKnapBitPheno = splitedPheno[i].Split("/");
                    if ((ItemKnapBitGeno[0] != ItemKnapBitPheno[0]) ||
                    (ItemKnapBitGeno[1] != ItemKnapBitPheno[1]))
                    {
                        feedbackText += _IsEncode ? 
                            "\n- The chromosome mapping is wrong." :
                            "\n- You select the wrong preset.";
                        break;
                    }
                    else if (ItemKnapBitGeno[2] != ItemKnapBitPheno[2])
                    {
                        feedbackText += _IsEncode ?
                            "\n- Some bit value is wrong." :
                            "\n- Some item placement is wrong.";
                        break;
                    }
                }
            }
        }
        // Result conclusion
        List<string> jigsawFeedbacks = PlayerManager.IsFixingLastFactory ? PlayerManager.RecordPuzzleResultForLastFactory(isCorrect) : PlayerManager.RecordPuzzleResult(isCorrect);
        foreach (string jigsawFeedback in jigsawFeedbacks)
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
