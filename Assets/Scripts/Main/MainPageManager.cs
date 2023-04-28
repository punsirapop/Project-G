using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TimeManager;

public class MainPageManager : MonoBehaviour
{
    [SerializeField] private Transform _FactoriesArea;
    [SerializeField] private Transform _FarmsArea;
    [SerializeField] private GameObject _FactoryMainPrefab;
    [SerializeField] private GameObject _FarmsMainPrefab;
    [SerializeField] private TextMeshProUGUI _DateDisplay;

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

        foreach (Transform child in _FarmsArea)
        {
            Destroy(child.gameObject);
        }
        for (int farmIndex = 1; farmIndex < PlayerManager.FarmDatabase.Length; farmIndex++)
        {
            GameObject newFarmMain = Instantiate(_FarmsMainPrefab, _FarmsArea);
            newFarmMain.GetComponent<FarmMain>().SetFarm(farmIndex, PlayerManager.FarmDatabase[farmIndex]);
        }

        if (PlayerManager.CurrentDate.Equals(default(Date))) PlayerManager.CurrentDate.InitDate();
    }

    private void Update()
    {
        _DateDisplay.text = PlayerManager.CurrentDate.ShowDate();
    }

    public void SetCurrentDialogueIndex(int newIndex)
    {
        PlayerManager.Instance.SetCurrentDialogueIndex(newIndex);
    }
}
