using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] ShopSO _Shop;
    [SerializeField] ShopItemDisplay[] _ShopDisplays;
    [SerializeField] TextMeshProUGUI _RemainingDays, _Money;

    private void Start()
    {
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
        if (_Shop.Purchase(index))
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
