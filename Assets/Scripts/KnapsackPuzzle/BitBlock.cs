 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BitBlock : MonoBehaviour
{
    // Variable for keep track of the statuses
    private int _BitValue;
    private string _ItemName;
    private string _KnapsackName;

    // Reference to UI components corresponding to the statuses
    [SerializeField] Toggle BitToggle;
    [SerializeField] Toggle ItemToggle;
    [SerializeField] Toggle KnapsackToggle;

    // Set default value of the BitBlock and visually represent it
    void Start()
    {
        _ItemName = "-";
        _KnapsackName = "-";
        UpdateBit();
        UpdateItem();
        UpdateKnapsack();
    }

    // Update value of the bit and display it visually
    public void UpdateBit()
    {
        _BitValue = BitToggle.isOn ? 1 : 0;
        BitToggle.GetComponentInChildren<Text>().text = _BitValue.ToString();
    }

    // Set item of BitBlock to clicked item on Phenotype panel
    public void SetItem(string itemName)
    {
        if (ItemToggle.isOn)
        {
            _ItemName = itemName;
            ItemToggle.isOn = false;
            UpdateItem();
        }
    }

    // Update item name and display it visually
    public void UpdateItem()
    {
        string displayItem = ItemToggle.isOn ? "?" : _ItemName;
        ItemToggle.GetComponentInChildren<Text>().text = displayItem;
    }

    // Set knapsack of BitBlock to clicked knapsack on Phenotype panel
    public void SetKnapsack(string knapsackName)
    {
        if (KnapsackToggle.isOn)
        {
            _KnapsackName = knapsackName;
            KnapsackToggle.isOn = false;
            UpdateKnapsack();
        }
    }

    // Update item name and display it visually
    public void UpdateKnapsack()
    {
        string displayKnapsack = KnapsackToggle.isOn ? "?" : _KnapsackName;
        KnapsackToggle.GetComponentInChildren<Text>().text = displayKnapsack;
    }
}
