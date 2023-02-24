using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhenotypeManager : MonoBehaviour
{
    public static PhenotypeManager Instance;

    // Reference to the holder UI and prefab
    [SerializeField] private Transform _ItemHolder;
    [SerializeField] private Transform _KnapsackHolder;
    [SerializeField] private GameObject _ItemPrefab;
    [SerializeField] private GameObject _KnapsackPrefab;
    [SerializeField] private FactoryConfig _FactoryConf;
    // Reference to actual Item and Knapsack Configuration
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
        ResetObject();
    }

    public void ResetObject()
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
        ResetObject();
    }

    // Change the Knapsack preset to the previous
    public void PreviousKnapsackPreset()
    {
        _KnapsackPreset--;
        if (_KnapsackPreset < 1)
        {
            _KnapsackPreset = 4;
        }
        ResetObject();
    }

    // Instantiate Knapsack according to the preset
    private void _InstantiateKnapsacks()
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _KnapsackHolder)
        {
            Destroy(child.gameObject);
        }
        // Set the preset as same as the configuration in the factory
        KnapsackSO[] knapsackPreset;
        switch(_KnapsackPreset)
        {
            default:
                knapsackPreset = _FactoryConf.Factory1Knapsacks;
                break;
            case 1:
                knapsackPreset = _FactoryConf.Factory1Knapsacks;
                break;
            case 2:
                knapsackPreset = _FactoryConf.Factory2Knapsacks;
                break;
            case 3:
                knapsackPreset = _FactoryConf.Factory3Knapsacks;
                break;
            case 4:
                knapsackPreset = _FactoryConf.Factory4Knapsacks;
                break;
        }
        // Create actual Knapsack object in the game
        foreach(KnapsackSO knapsack in knapsackPreset)
            {
            GameObject newKnapsack = Instantiate(_KnapsackPrefab);
            newKnapsack.transform.SetParent(_KnapsackHolder);
            newKnapsack.GetComponent<Knapsack>().SetKnapsack(knapsack);
        }
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
        ResetObject();
    }

    // Change the Item preset to the previous
    public void PreviousItemPreset()
    {
        _ItemPreset--;
        if (_ItemPreset < 1)
        {
            _ItemPreset = 2;
        }
        ResetObject();
    }

    // Instantiate Item according to the preset
    private void _InstantiateItems()
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _ItemHolder)
        {
            Destroy(child.gameObject);
        }
        // Set the preset as same as the configuration in the factory
        ItemSO[] itemPreset;
        switch (_ItemPreset)
        {
            default:
                itemPreset = _FactoryConf.Set1Items;
                break;
            case 1:
                itemPreset = _FactoryConf.Set1Items;
                break;
            case 2:
                itemPreset = _FactoryConf.Set2Items;
                break;
        }
        // Create actual Item object in the game
        foreach (ItemSO item in itemPreset)
        {
            GameObject newItem = Instantiate(_ItemPrefab);
            newItem.transform.SetParent(_ItemHolder);
            newItem.GetComponent<Item>().SetItem(item);
        }
        // Keep reference to the Item
        _Items = GetComponentsInChildren<Item>();
    }
    #endregion
}
