using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenotypeManager : MonoBehaviour
{
    public static GenotypeManager Instance;

    [SerializeField] TextMeshProUGUI BitCountText;
    private int _BitCount;
    private BitBlock[] _BitBlocks;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Get reference to all available BitBlock in the panel
        _BitBlocks = GetComponentsInChildren<BitBlock>();
        _BitCount = 0;
    }

    #region Temp Function to test the button
    private void _UpdateValue()
    {
        BitCountText.text = _BitCount.ToString();
    }

    public void AddBit()
    {
        _BitCount = _BitCount + 1;
        _UpdateValue();
    }

    public void RemoveBit()
    {
        _BitCount = _BitCount - 1;
        _UpdateValue();
    }
    #endregion

    // Set item on all enabled BitBlock
    public void SetItemOnBits(string itemName)
    {
        foreach (BitBlock bitBlock in _BitBlocks)
        {
            bitBlock.SetItem(itemName);
        }
    }

    // Set knapsack on all enabled BitBlock
    public void SetKnapsackOnBits(string knapsackName)
    {
        foreach (BitBlock bitBlock in _BitBlocks)
        {
            bitBlock.SetKnapsack(knapsackName);
        }
    }
}
