using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TimeManager;

public class MainPageManager : MonoBehaviour
{
    public static MainPageManager Instance;

    // Facilities
    [SerializeField] private Transform _FactoriesArea;
    [SerializeField] private Transform _FarmsArea;
    [SerializeField] private GameObject _FactoryMainPrefab;
    [SerializeField] private GameObject _FarmsMainPrefab;

    // Overlay
    private FactoryMain _CurrentFactoryMain;
    [SerializeField] private GameObject _UnlockOverlay;
    [SerializeField] private GameObject _FixChoicesOverlay;

    // Date
    [SerializeField] private TextMeshProUGUI _DateDisplay;

    // Money
    [SerializeField] private TextMeshProUGUI _MoneyDisplay;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        RenderFacilities();

        if (PlayerManager.CurrentDate.Equals(default(Date))) PlayerManager.CurrentDate.InitDate();
        _UnlockOverlay.SetActive(false);
        _FixChoicesOverlay.SetActive(false);
    }

    private void Update()
    {
        _DateDisplay.text = PlayerManager.CurrentDate.ShowDate();
        _MoneyDisplay.text = PlayerManager.Money.ToString();
    }

    // Render facilities
    public void RenderFacilities()
    {
        foreach (Transform child in _FactoriesArea)
        {
            Destroy(child.gameObject);
        }
        foreach (FactorySO factorySO in PlayerManager.FactoryDatabase)
        {
            GameObject newFactoryMain = Instantiate(_FactoryMainPrefab, _FactoriesArea);
            newFactoryMain.GetComponent<FactoryMain>().SetFactory(factorySO);
        }
        //for (int factoryIndex = 0; factoryIndex < PlayerManager.FactoryDatabase.Length; factoryIndex++)
        //{
        //    GameObject newFactoryMain = Instantiate(_FactoryMainPrefab, _FactoriesArea);
        //    newFactoryMain.GetComponent<FactoryMain>().SetFactory(factoryIndex, PlayerManager.FactoryDatabase[factoryIndex]);
        //}

        foreach (Transform child in _FarmsArea)
        {
            Destroy(child.gameObject);
        }
        for (int farmIndex = 1; farmIndex < PlayerManager.FarmDatabase.Length; farmIndex++)
        {
            GameObject newFarmMain = Instantiate(_FarmsMainPrefab, _FarmsArea);
            newFarmMain.GetComponent<FarmMain>().SetFarm(farmIndex, PlayerManager.FarmDatabase[farmIndex]);
        }
    }

    // Display a facility unlock overlay when the facility isn't unlocked
    public void DisplayUnlockOverlay(FactoryMain factoryMain)
    {
        _CurrentFactoryMain = factoryMain;
        _UnlockOverlay.GetComponent<MainUnlockOverlay>().SetOverlay(factoryMain.FactoryDatabase);
        _UnlockOverlay.SetActive(true);
    }

    // Wrap function for unlocking factory, use for Unlock overlay
    public void UnlockFactory()
    {
        _CurrentFactoryMain.UnlockFactory();
    }

    // Display a facility fix choices overlay, transition to the puzzles
    public void DisplayFixChoice()
    {
        // Random the proper puzzle type
        // WIP

        // Display the fix choice overlay
        _FixChoicesOverlay.SetActive(true);
    }

    // tmp function to test facility fixing
    public void FixFacility()
    {
        PlayerManager.Instance.FixFacility();
    }

    // tmp function to add money via main game page
    public void GainHugeMoney()
    {
        PlayerManager.GainMoneyIfValid(1000);
        PlayerManager.ValidateUnlocking();
        RenderFacilities();
    }
}
