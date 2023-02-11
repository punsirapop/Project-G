using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhenotypeManager : MonoBehaviour
{
    public static PhenotypeManager Instance;

    // Reference to the holder UI and prefab
    [SerializeField] Transform ItemHolder;
    [SerializeField] Transform KnapsackHolder;
    [SerializeField] GameObject ItemPrefab;
    [SerializeField] GameObject KnapsackPrefab;
    // Reference to actual Item and Knapsack
    private int _ItemPreset;
    private int _KnapsackPreset;
    private Item[] _Items;
    private Knapsack[] _Knapsacks;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _ItemPreset = 1;
        _KnapsackPreset = 1;
        _ResetObject();
    }

    private void _ResetObject()
    {
        _InstantiateKnapsacks();
        _InstantiateItems();
        EnableObjectSetting();
    }

    // Make the objects can be assigned to the BitBlock when it's clicked by adding listener
    public void EnableObjectSetting()
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

    #region Knapsack Instantiation ################################################################
    // Change the Knapsack preset to the nex
    public void NextKnapsackPreset()
    {
        _KnapsackPreset++;
        if(_KnapsackPreset > 4)
        {
            _KnapsackPreset = 1;
        }
        _ResetObject();
    }

    // Change the Knapsack preset to the previous
    public void PreviousKnapsackPreset()
    {
        _KnapsackPreset--;
        if (_KnapsackPreset < 1)
        {
            _KnapsackPreset = 4;
        }
        _ResetObject();
    }

    // Instantiate Knapsack according to the preset
    private void _InstantiateKnapsacks()
    {
        // Destroy all previous object in the holder
        foreach (Transform child in KnapsackHolder)
        {
            Destroy(child.gameObject);
        }
        // Temp instantiate prodedure /////////////////////////////////////////////////////////////
        int amount = _KnapsackPreset < 3 ? 1 : 2;
        // Instantiate Knapsack in the holder
        for (int i = 0; i < amount; i++)
        {
            GameObject newKnapsack = Instantiate(KnapsackPrefab);
            newKnapsack.transform.SetParent(KnapsackHolder);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Keep reference to the Knapsack
        _Knapsacks = GetComponentsInChildren<Knapsack>();
    }
    #endregion

    #region Item Instantiation ####################################################################
    // Change the Item preset to the nex
    public void NextItemPreset()
    {
        _ItemPreset++;
        if (_ItemPreset > 2)
        {
            _ItemPreset = 1;
        }
        _ResetObject();
    }

    // Change the Item preset to the previous
    public void PreviousItemPreset()
    {
        _ItemPreset--;
        if (_ItemPreset < 1)
        {
            _ItemPreset = 2;
        }
        _ResetObject();
    }

    // Instantiate Item according to the preset
    private void _InstantiateItems()
    {
        // Destroy all previous object in the holder
        foreach (Transform child in ItemHolder)
        {
            Destroy(child.gameObject);
        }
        // Temp instantiate prodedure /////////////////////////////////////////////////////////////
        int amount = _ItemPreset;
        // Instantiate Item in the holder
        for (int i = 0; i < amount*5; i++)
        {
            GameObject newItem = Instantiate(ItemPrefab);
            newItem.transform.SetParent(ItemHolder);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Keep reference to the Item
        _Items = GetComponentsInChildren<Item>();
    }
    #endregion
}
