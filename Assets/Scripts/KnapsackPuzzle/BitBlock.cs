 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BitBlock : MonoBehaviour
{
    // Variable for keep track of the statuses
    private int _BitValue;
    public int BitValue => _BitValue;
    private string _ItemName = "-";
    public string ItemName => _ItemName;
    private string _KnapsackName = "-";
    public string KnapsackName => _KnapsackName;

    // Reference to UI components corresponding to the statuses
    [SerializeField] Toggle BitToggle;
    [SerializeField] Toggle ItemToggle;
    [SerializeField] Toggle KnapsackToggle;

    // Set default value of the BitBlock and visually represent it
    void Start()
    {
        //_ItemName = "-";
        //_KnapsackName = "-";
        UpdateBit();
        UpdateItem();
        UpdateKnapsack();
    }

    public void SetInteractable(bool interactable)
    {
        BitToggle.interactable = interactable;
        ItemToggle.interactable = interactable;
        KnapsackToggle.interactable = interactable;
    }

    public void SetBit(int newBit)
    {
        if (newBit == 0)
        {
            BitToggle.isOn = false;
        }
        else
        {
            BitToggle.isOn = true;
        }
        UpdateBit();
    }

    // Update value of the bit and display it visually
    public void UpdateBit()
    {
        _BitValue = BitToggle.isOn ? 1 : 0;
        BitToggle.GetComponentInChildren<Text>().text = _BitValue.ToString();
    }

    public void SetItem(string itemName)
    {
        _ItemName = itemName;
        ItemToggle.isOn = false;
        UpdateItem();
    }

    // Set item of BitBlock to clicked item on Phenotype panel
    public void SetItemIfEnabled(string itemName)
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

    public void SetKnapsack(string knapsackName)
    {
        _KnapsackName = knapsackName;
        KnapsackToggle.isOn = false;
        UpdateKnapsack();
    }

    // Set knapsack of BitBlock to clicked knapsack on Phenotype panel
    public void SetKnapsackIfEnabled(string knapsackName)
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
