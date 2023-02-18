using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GenotypeManager : MonoBehaviour
{
    public static GenotypeManager Instance;

    [SerializeField] TextMeshProUGUI RowCount;

    // Reference to the holder UI and prefab
    [SerializeField] Transform BitHolder;
    [SerializeField] GameObject BitBlockPrefab;
    // Reference to actual BitBlock
    private BitBlock[] _BitBlocks;
    // Variable indicating the number of row of BitBlock
    private bool _IsTwoRow;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _IsTwoRow = false;
        UpdateRowCount();
        _InstantiateBitBlocks();
    }

    public void ResetObject()
    {
        _InstantiateBitBlocks();
    }

    // Instantiate BitBlock according to given amount of bit
    private void _InstantiateBitBlocks()
    {
        // Destroy all previous object in the holder
        foreach (Transform child in BitHolder)
        {
            Destroy(child.gameObject);
        }
        int amount = _IsTwoRow ? 20 : 10;
        // Instantiate BitBlock in the holder
        for (int i = 0; i < amount; i++)
        {
            GameObject newBitBlock = Instantiate(BitBlockPrefab);
            newBitBlock.transform.SetParent(BitHolder);
        }
        // Keep reference to the BitBlock
        _BitBlocks = GetComponentsInChildren<BitBlock>();
        // Enable setting item/knapsack on BitBlock when click object on phenotype panel
        PhenotypeManager.Instance.EnableObjectSetting();
    }

    // Update the number of row on the UI
    public void UpdateRowCount()
    {
        RowCount.text = _IsTwoRow ? "2" : "1";
    }

    // Toggle between 1 and 2 row of BitBlock
    public void ToggleRow()
    {
        _IsTwoRow = !_IsTwoRow;
        UpdateRowCount();
        ResetObject();
    }

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
