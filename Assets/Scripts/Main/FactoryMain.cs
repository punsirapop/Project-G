using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryMain : MonoBehaviour
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
    private int _FactoryIndex;

    void Start()
    {
        _MainBackground.GetComponent<Image>().sprite = _MainNormal;
        _MainBackground.GetComponent<Button>().onClick.AddListener(() => EnterFactory());
        _MainLightBoard.GetComponent<Button>().onClick.AddListener(() => EnterKnapsackPuzzle());
    }

    void Update()
    {
        int lights = 0;
        foreach (var item in _MainLightBoard.GetComponentsInChildren<Image>())
        {
            item.color = (lights < PlayerManager.FactoryDatabase[_FactoryIndex].Condition) ? Color.green : Color.red;
            lights++;
        }
    }

    public void SetFactory(int newFactoryIndex, FactorySO newFactorySO)
    {
        _FactoryIndex = newFactoryIndex;
        _MainNormal = newFactorySO.MainNormal;
        _MainBroken = newFactorySO.MainBroken;
        _Locker = newFactorySO.Locker;
    }

    private void EnterFactory()
    {
        PlayerManager.CurrentFactoryIndex = _FactoryIndex;
        this.GetComponent<SceneMng>().ChangeScene("Factory");
    }

    private void EnterKnapsackPuzzle()
    {
        this.GetComponent<SceneMng>().ChangeScene("KnapsackPuzzle");
    }

    public void Fix()
    {
        PlayerManager.FactoryDatabase[_FactoryIndex].Fixed();
    }
}
