using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhenotypeManager : MonoBehaviour
{
    //public static event Action<GameObject> OnClickObject;
    private Item[] _Items;

    void Start()
    {
        // Get reference to all available item in the panel
        _Items = GetComponentsInChildren<Item>();
        foreach (Item item in _Items)
        {
            // Add this item to enabled BitBlock (s) when click on this item
            Button button = item.GetComponent<Button>();
            button.onClick.AddListener(() => GenotypeManager.Instance.SetItemOnBits(item.Name));
        }
    }

    void Update()
    { 
    }
}
