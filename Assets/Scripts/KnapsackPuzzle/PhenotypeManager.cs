using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhenotypeManager : MonoBehaviour
{
    private Item[] _Items;
    private Knapsack[] _Knapsacks;

    void Start()
    {
        // Get reference to all available item in the panel
        _Items = GetComponentsInChildren<Item>();
        foreach (Item item in _Items)
        {
            // Add this item to enabled BitBlock(s) when click on this item
            Button button = item.GetComponent<Button>();
            button.onClick.AddListener(() => GenotypeManager.Instance.SetItemOnBits(item.Name));
        }

        // Get reference to all available knapsack in the panel
        _Knapsacks = GetComponentsInChildren<Knapsack>();
        foreach (Knapsack knapsack in _Knapsacks)
        {
            // Add this knapsack to enabled BitBlock(s) when click on this knapsack
            Button button = knapsack.GetComponent<Button>();
            button.onClick.AddListener(() => GenotypeManager.Instance.SetKnapsackOnBits(knapsack.Name));
        }
    }
}
