using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhenotypeManager : MonoBehaviour
{
    public static PhenotypeManager Instance;

    // Reference to the holder UI and prefab
    [SerializeField] private Transform _Mask;
    [SerializeField] private Transform _ItemHolder;
    [SerializeField] private Transform _KnapsackHolder;
    [SerializeField] private GameObject _ItemPrefab;
    [SerializeField] private GameObject _KnapsackPrefab;
    [SerializeField] private GameObject _GrayOutBackground;
    [SerializeField] private Button[] _PresetButtons;
    // Information about knapsack and item
    private FactorySO[] _FactoriesData;
    private GameObject[] _Knapsacks;
    private int _KnapsackPresetIndex;
    private int _ItemPresetIndex;
    // Variable indicates the objects can be resetted
    private bool _Resettable;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _FactoriesData = KnapsackPuzzleManager.Instance.FactoriesData;
        _KnapsackPresetIndex = 0;
        _ItemPresetIndex = 0;
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

    // Reset object when click CLEAR button on puzzle
    public void ResetObject()
    {
        if (_Resettable)
        {
            InstantiateKnapsacks(_KnapsackPresetIndex);
            InstantiateItems(_ItemPresetIndex);
            EnableSettingOnBitblock();
        }
    }

    // Set enable/disable preset changing by enable/disable _PresetButtons
    public void SetPresetChangable(bool changable)
    {
        foreach (Button button in _PresetButtons)
        {
            button.interactable = changable;
        }
    }

    // Change the Knapsack preset
    public void ChangeKnapsackPreset(int amount)
    {
        _KnapsackPresetIndex += amount;
        if (_KnapsackPresetIndex > _FactoriesData.Length - 1)
        {
            _KnapsackPresetIndex = 0;
        }
        else if (_KnapsackPresetIndex < 0)
        {
            _KnapsackPresetIndex = _FactoriesData.Length - 1;
        }
        ResetObject();
    }

    // Change the Item preset
    public void ChangeItemsPreset(int amount)
    {
        _ItemPresetIndex += amount;
        if (_ItemPresetIndex > _FactoriesData.Length - 1)
        {
            _ItemPresetIndex = 0;
        }
        else if (_ItemPresetIndex < 0)
        {
            _ItemPresetIndex = _FactoriesData.Length - 1;
        }
        ResetObject();
    }
    #endregion

    // Make the objects can be assigned to the BitBlock when it's clicked by adding listener
    public void EnableSettingOnBitblock()
    {
        // Get reference to all available item in the panel
        Item[] items = this.GetComponentsInChildren<Item>();
        foreach (Item item in items)
        {
            // Add this item to enabled BitBlock(s) when click on this item
            Button button = item.GetComponent<Button>();
            button.onClick.AddListener(() => GenotypeManager.Instance.SetItemOnEnabledBits(item.Name));
        }
        // Get reference to all available knapsack in the panel
        Knapsack[] knapsacks = this.GetComponentsInChildren<Knapsack>();
        foreach (Knapsack knapsack in knapsacks)
        {
            // Add this knapsack to enabled BitBlock(s) when click on this knapsack
            Button button = knapsack.GetComponent<Button>();
            button.onClick.AddListener(() => GenotypeManager.Instance.SetKnapsackOnEnabledBits(knapsack.Name));
        }

    }

    #region Knapsacks and Items Instantiation
    // Instantiate Knapsack according to the preset index, random preset by default
    public void InstantiateKnapsacks(int presetIndex = -1)
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _KnapsackHolder)
        {
            Destroy(child.gameObject);
        }
        // Set the preset as same as the configuration in the factory
        _KnapsackPresetIndex = (presetIndex != -1) ? presetIndex : Random.Range(0, _FactoriesData.Length);
        KnapsackSO[] knapsackPreset = _FactoriesData[_KnapsackPresetIndex].Knapsacks;
        // Create actual Knapsack object in the game
        _Knapsacks = new GameObject[knapsackPreset.Length];
        for (int i = 0; i < knapsackPreset.Length; i++)
        {
            GameObject newKnapsack = Instantiate(_KnapsackPrefab);
            newKnapsack.transform.SetParent(_KnapsackHolder);
            newKnapsack.GetComponent<Knapsack>().SetKnapsack(knapsackPreset[i]);
            // Keep the reference to gameObject of Knapsack for instanitiate the Item on it later
            _Knapsacks[i] = newKnapsack;
        }
    }

    // Instantiate Item according to the preset, random preset by default
    public void InstantiateItems(int presetIndex = -1, int[][] bitstring = null)
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _ItemHolder)
        {
            Destroy(child.gameObject);
        }
        // Set the preset as same as the configuration in the factory
        _ItemPresetIndex = (presetIndex != -1) ? presetIndex : Random.Range(0, _FactoriesData.Length);
        ItemSO[] itemPreset = _FactoriesData[_ItemPresetIndex].Items;
        // Calculate place to put each item in
        // itemPlace indicate where to place item in, 0 = outside knapsack, 1 = first knapsack, 2 = second knapsack ...
        // note that the <new int[]> give all the value inside as 0 by default
        int[] itemPlace = new int[itemPreset.Length];
        bool hasBitstring = (bitstring != null);
        if (hasBitstring)
        {
            for (int kIndex = 0; kIndex < bitstring.Length; kIndex++)
            {
                int[] section = bitstring[kIndex];
                if (section == null)
                {
                    continue;
                }
                // Mark itemPlace using the kIndex (knapsack index) if the bit inside bitstring section is 1
                for (int iIndex = 0; iIndex < section.Length; iIndex++)
                {
                    itemPlace[iIndex] += section[iIndex] * (kIndex + 1);
                }
            }
        }
        // Create actual Item object in the game
        for (int i = 0; i < itemPlace.Length; i++)
        {
            GameObject newItem = Instantiate(_ItemPrefab);
            int place = itemPlace[i];
            if (place == 0)
            {
                newItem.transform.SetParent(_ItemHolder);
            }
            else
            {
                Debug.Log("Place = " + place.ToString());
                newItem.transform.SetParent(_Knapsacks[place - 1].GetComponent<Knapsack>().GetItemHolder());
            }
            newItem.GetComponent<Item>().SetMask(_Mask);
            newItem.GetComponent<Item>().SetItem(itemPreset[i]);
            newItem.GetComponent<Item>().SetDraggable(!hasBitstring);
        }
    }
    #endregion

    // Function to transform the current phenotype into string for the sake of answer checking
    public new string ToString()
    {
        // Check if dimension of knapsack not match the items
        int knapsackDimension = GetComponentInChildren<Knapsack>().GetDimension();
        int itemDimension = GetComponentInChildren<Item>().GetDimension();
        if (knapsackDimension != itemDimension)
        {
            return "dim";
        }
        // Create string from the phenotype
        Knapsack[] knapsacks = GetComponentsInChildren<Knapsack>();
        string answer = "";
        foreach (Knapsack k in knapsacks)
        {
            if (k == null)
            {
                break;
            }
            Item[] itemsInKnapsack = k.GetComponentsInChildren<Item>();
            Item[] items = this.GetComponentsInChildren<Item>();
            foreach (Item i in items)
            {
                if (i == null)
                {
                    break;
                }
                // Check if the items is in the knapsack
                bool isInKnapsack = false;
                foreach (Item ik in itemsInKnapsack)
                {
                    if (i.Name == ik.Name)
                    {
                        isInKnapsack = true;
                        break;
                    }
                }
                if (isInKnapsack)
                {
                    answer += i.Name + "/" + k.Name + "/" + "1_";
                }
                else
                {
                    answer += i.Name + "/" + k.Name + "/" + "0_";
                }
            }
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