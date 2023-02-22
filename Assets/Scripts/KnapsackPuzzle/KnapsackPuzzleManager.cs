using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnapsackPuzzleManager : MonoBehaviour
{
    public static KnapsackPuzzleManager Instance;

    // Data of all factory
    [SerializeField] private FactorySO[] _FactoriesData;
    public FactorySO[] FactoriesData => _FactoriesData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }


    // Set Pheonotype and Genotype managers to suit the type of puzzle
    // isSolve and isEncode might be placed in PlayerManager which works as the inter-scence data holder later
    public void SetPuzzle(bool isSolve, bool isEncode)
    {
        PhenotypeManager.Instance.ResetObject();
        GenotypeManager.Instance.ResetObject();
        // Get random chromosome from factory to generate puzzle
        // Temp forced it to use bitstring from the first factory /////////////////////////////////
        int factoryIndex = 0;
        int[][] bitstring = _FactoriesData[factoryIndex].GetRandomBitstring();
        string logText = "Generate puzzle using bitstring: ";
        foreach (int bit in bitstring[0])
        {
            logText += bit.ToString();
        }
        Debug.Log(logText);
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Encoding puzzle, set Phenotype, player interact with Genotype
        if (isEncode)
        {
            PhenotypeManager.Instance.InstantiateKnapsacks(factoryIndex);
            PhenotypeManager.Instance.InstantiateItems(factoryIndex, bitstring);
            PhenotypeManager.Instance.SetPresetChangable(false);
            PhenotypeManager.Instance.SetResettable(false);
            // Solve
            if (isSolve)
            {
                GenotypeManager.Instance.CreateMapMask(_FactoriesData[factoryIndex].Items.Length, 0.7f);
                GenotypeManager.Instance.InstantiateBitBlocks();
                GenotypeManager.Instance.SetPresetChangable(true);
            }
            // Demonstrate
            else
            {
                GenotypeManager.Instance.CreateMapMask(_FactoriesData[factoryIndex].Items.Length, 1f);
                GenotypeManager.Instance.InstantiateBitBlocks(factoryIndex);
                GenotypeManager.Instance.SetPresetChangable(false);
            }
            GenotypeManager.Instance.SetResettable(true);
        }
        // Decoding puzzle, set Genotype, player interact with Phenotype
        else
        {
            GenotypeManager.Instance.CreateMapMask(_FactoriesData[factoryIndex].Items.Length, 1f);
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
    public void SetPuzzle(int puzzleType=0)
    {
        switch(puzzleType)
        {
            case 0:
                SetPuzzle(false, false);
                break;
            case 1:
                SetPuzzle(false, true);
                break;
            case 2:
                SetPuzzle(true, false);
                break;
            case 3:
                SetPuzzle(true, true);
                break;
        }
    }

}
