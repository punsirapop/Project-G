using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [SerializeField] ShopSO _Shop;
    [SerializeField] ShopItemDisplay[] _ShopDisplays;
    [SerializeField] TextMeshProUGUI _RemainingDays, _Money;
    [SerializeField] int _MechPricePerUnit;
    public int MechPricePerUnit => _MechPricePerUnit;


    private void Start()
    {
        if (Instance == null) Instance = this;

        Rendering();
    }

    private void Rendering()
    {
        for (int i = 0; i < 3; i++)
        {
            _ShopDisplays[i].UpdateChromo(_Shop.ShopItems[i], _Shop.InStock[i]);
        }
        _RemainingDays.text = $"Restock in {_Shop.DayLeftBeforeRestock} {(_Shop.DayLeftBeforeRestock > 1 ? "days" : "day")}";
        _Money.text = PlayerManager.Money.ToString();
    }

    public void Purchase(int index)
    {
        if (_Shop.Purchase(index, _MechPricePerUnit))
        {
            MechChromo m = new MechChromo(null);
            m.SetChromosomeFromPreset(_Shop.ShopItems[index]);
            PlayerManager.FarmDatabase[0].AddChromo(m);
            Rendering();
        }
    }

    public void ForceRendering()
    {
        _Shop.Restock();
    }
}
