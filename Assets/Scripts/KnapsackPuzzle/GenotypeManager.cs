using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GenotypeManager : MonoBehaviour
{
    public static GenotypeManager Instance;

    // Reference to the holder UI and prefab
    [SerializeField] private Transform _BitHolder;
    [SerializeField] private GameObject _BitBlockPrefab;
    [SerializeField] private GameObject _GrayOutBackground;
    [SerializeField] private Button[] _PresetButtons;
    // Reference to actual BitBlock
    private BitBlock[] _BitBlocks;
    // Necessary variables for generating the puzzle
    private FactorySO[] _FactoriesData;
    private int _BitblockPresetIndex;
    private bool _Resettable;
    private int[] _MapMask = new int[10];

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _FactoriesData = KnapsackPuzzleManager.Instance.FactoriesData;
        _BitblockPresetIndex = 0;
        _Resettable = true;
        _GrayOutBackground.SetActive(false);
        ResetObject();
    }

    #region Set and resetting values
    // Set objects resettable indicate wheter the objects can be resetted when click CLEAR button on puzzle
    public void SetResettable(bool resettable)
    {
        _Resettable = resettable;
        _GrayOutBackground.SetActive(!resettable);
    }

    // Reset object used when click CLEAR button on puzzle
    public void ResetObject()
    {
        if (_Resettable)
        {
            InstantiateBitBlocks(_BitblockPresetIndex);
            PhenotypeManager.Instance.EnableSettingOnBitblock();
        }
    }

    // Set enable/disable preset changing by enable/disable the buttons
    public void SetPresetChangable(bool changable)
    {
        foreach (Button button in _PresetButtons)
        {
            button.interactable = changable;
        }
    }

    // Change the preset of item and knapsack used for creating Bitblock
    public void ChangePreset(int amount)
    {
        _BitblockPresetIndex += amount;
        if (_BitblockPresetIndex > _FactoriesData.Length - 1)
        {
            _BitblockPresetIndex = 0;
        }
        else if (_BitblockPresetIndex < 0)
        {
            _BitblockPresetIndex = _FactoriesData.Length - 1;
        }
        ResetObject();
    }

    // Set item name on all enabled BitBlock
    public void SetItemOnEnabledBits(string itemName)
    {
        foreach (BitBlock bitBlock in _BitBlocks)
        {
            bitBlock.SetItemIfEnabled(itemName);
        }
    }

    // Set knapsack name on all enabled BitBlock
    public void SetKnapsackOnEnabledBits(string knapsackName)
    {
        foreach (BitBlock bitBlock in _BitBlocks)
        {
            bitBlock.SetKnapsackIfEnabled(knapsackName);
        }
    }
    #endregion

    // Create mask using for auto-generating mapping (item-knapsack) on the bitblock
    public void CreateMapMask(int mapLenght, float mapRatio)
    {
        _MapMask = new int[mapLenght];
        for (int i = 0; i < mapLenght; i++)
        {
            if (Random.Range(0f, 1f) <= mapRatio)
            {
                _MapMask[i] = 1;
            }
        }
    }

    // Instantiate BitBlock according to the preset and given bitstring (if any)
    public void InstantiateBitBlocks(int presetIndex=-1, int[][] bitstring=null)
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _BitHolder)
        {
            Destroy(child.gameObject);
        }
        // Selecting item and knapsack preset, random preset if generated with default parameter (presetIndex=-1)
        _BitblockPresetIndex = (presetIndex != -1) ? presetIndex : Random.Range(0, _FactoriesData.Length);
        KnapsackSO[] knapsackPreset = _FactoriesData[_BitblockPresetIndex].Knapsacks;
        ItemSO[] itemPreset = _FactoriesData[_BitblockPresetIndex].Items;
        bool hasBitstring = (bitstring != null);
        // Create Bitblock from the preset
        for (int kIndex = 0; kIndex < knapsackPreset.Length; kIndex++)
        {
            for (int iIndex = 0; iIndex < itemPreset.Length; iIndex++)
            {
                // Create actual Bitblock object in the game
                GameObject newBitBlock = Instantiate(_BitBlockPrefab);
                newBitBlock.transform.SetParent(_BitHolder);
                // Set mapping for the bit where map mask is 1
                if (_MapMask[iIndex] == 1)
                {
                    newBitBlock.GetComponent<BitBlock>().SetKnapsack(knapsackPreset[kIndex].Name);
                    newBitBlock.GetComponent<BitBlock>().SetItem(itemPreset[iIndex].Name);
                }
                // Set the bit of Bitblock in case if there is bitstring
                newBitBlock.GetComponent<BitBlock>().SetInteractable(!hasBitstring);
                if (hasBitstring)
                {
                    newBitBlock.GetComponent<BitBlock>().SetBit(bitstring[kIndex][iIndex]);
                }
            }
        }
        // Keep reference to the BitBlock
        _BitBlocks = GetComponentsInChildren<BitBlock>();
    }

    // Function to transform the current bitstring and the mapping into string for the sake of answer checking
    public new string ToString()
    {
        // Convert all Bitblock to string
        string answer = "";
        _BitBlocks = GetComponentsInChildren<BitBlock>();
        foreach (BitBlock block in _BitBlocks)
        {
            answer += block.ItemName + "/" + block.KnapsackName + "/" + block.BitValue.ToString() + "_";
        }
        // Sort the string
        answer = answer.Trim('_');
        List<string> answerList = new List<string> { };
        answerList.AddRange(answer.Split("_"));
        answerList.Sort();
        // Concat it back to string
        string sortedAnswer = "";
        foreach (string a in answerList)
        {
            sortedAnswer += a + "_";
        }
        sortedAnswer = sortedAnswer.Trim('_');
        return sortedAnswer;
    }
}