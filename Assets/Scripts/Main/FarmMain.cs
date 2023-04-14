using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmMain : FarmMngFunc
{ 
    // Sprite of this factory in main game page
    [SerializeField] private Sprite _MainLocked;
    private Sprite _MainNormal;
    private Sprite _MainBroken;
    private Sprite _Locker;

    // Actual UI element object
    [SerializeField] private GameObject _MainBackground;
    [SerializeField] private GameObject _MainLightBoard;

    // Factory information, this should be managed by PlayerManager and/or FactorySO later
    private int _FarmIndex;

    void Start()
    {
        _MainBackground.GetComponent<Image>().sprite = _MainNormal;
        _MainBackground.GetComponent<Button>().onClick.AddListener(() => EnterFarm());
        // _MainLightBoard.GetComponent<Button>().onClick.AddListener(() => EnterKnapsackPuzzle());
    }

    private void Update()
    {
        // for debug purposes
        int lights = 0;
        foreach (var item in _MainLightBoard.GetComponentsInChildren<Image>())
        {
            item.color = (lights < PlayerManager.FarmDatabase[_FarmIndex].Condition) ? Color.green : Color.red;
            lights++;
        }
    }

    public void SetFarm(int newFarmIndex, FarmSO newFarmSO)
    {
        _FarmIndex = newFarmIndex;
        _MainNormal = newFarmSO.MainNormal;
        _MainBroken = newFarmSO.MainBroken;
        _Locker = newFarmSO.Locker;

        /*
        int lights = 0;
        foreach (var item in _MainLightBoard.GetComponentsInChildren<Image>())
        {
            item.color = (lights < newFarmSO.Condition) ? Color.green : Color.red;
            lights++;
        }
        */
    }

    private void EnterFarm()
    {
        PlayerManager.CurrentFarmIndex = _FarmIndex;
        this.GetComponent<SceneMng>().ChangeScene("Farm");
    }

    private void EnterKnapsackPuzzle()
    {
        this.GetComponent<SceneMng>().ChangeScene("KnapsackPuzzle");
    }

    // ------- DEBUG -------
    public void AddChromo()
    {
        AddChromo(PlayerManager.FarmDatabase[_FarmIndex]);
    }

    public void FixFarm()
    {
        PlayerManager.FarmDatabase[_FarmIndex].Fixed();
    }
}