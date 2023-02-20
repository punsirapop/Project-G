using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChromoMenu : MonoBehaviour
{
    // Prefab for button
    [SerializeField] private GameObject _WeaponButtonPrefab;
    // Place to store generated buttons
    [SerializeField] private RectTransform _SpawnButtonArea;

    public void Start()
    {
        ResetPanel();
    }

    // Re-generate all buttons in panels
    public void ResetPanel()
    {
        foreach (Transform item in _SpawnButtonArea)
        {
            Destroy(item.gameObject);
        }
        WeaponChromosome[] chromosomes = FactoryManager.Instance.GetAllWeapon();
        foreach (WeaponChromosome chromosome in chromosomes)
        {
            GameObject me = Instantiate(_WeaponButtonPrefab, _SpawnButtonArea);
            me.GetComponent<WeaponButton>().SetChromosome(chromosome);
        }
    }
}
