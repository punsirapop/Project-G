using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPageManager : MonoBehaviour
{
    [SerializeField] private Transform _FactoriesArea;
    [SerializeField] private GameObject _FactoryMainPrefab;

    void Start()
    {
        foreach (Transform child in _FactoriesArea)
        {
            Destroy(child.gameObject);
        }
        for (int factoryIndex = 0; factoryIndex < PlayerManager.FactoryDatabase.Length; factoryIndex++)
        {
            GameObject newFactoryMain = Instantiate(_FactoryMainPrefab, _FactoriesArea);
            newFactoryMain.GetComponent<FactoryMain>().SetFactory(factoryIndex, PlayerManager.FactoryDatabase[factoryIndex]);
        }
    }
}
