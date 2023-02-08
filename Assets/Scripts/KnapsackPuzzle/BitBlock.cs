 
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

    // Variable for enable object assigning to the BitBlock
    private bool _EnableSetItem;
    private bool _EnableSetKnapsack;

    // Reference to UI components corresponding to the statuses
    [SerializeField] Toggle BitToggle;
    [SerializeField] Button Item;
    [SerializeField] Button Knapsack;

    // Start is called before the first frame update
    void Start()
    {
        _EnableSetItem = false;
        _EnableSetKnapsack = false;
        UpdateBit();
    }

    // Update value of the bit and display it visually
    public void UpdateBit()
    {
        _BitValue = BitToggle.isOn ? 1 : 0;
        BitToggle.GetComponentInChildren<Text>().text = _BitValue.ToString();
        Debug.Log("_BitValue = " + _BitValue.ToString());
    }

    // Toggle on to enable item setting
    public void ToggleSetItem()
    {
        _EnableSetItem = !_EnableSetItem;
    }

    // Set item of BitBlock to clicked item on Phenotype panel
    public void SetItem(string itemName)
    {
        if(_EnableSetItem)
        {
            Item.GetComponentInChildren<TextMeshProUGUI>().text = itemName;
            _EnableSetItem = false;
        }
    }
}
