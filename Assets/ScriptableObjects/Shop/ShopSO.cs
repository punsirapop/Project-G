using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Shop")]
public class ShopSO : ScriptableObject
{
    [SerializeField] int _RestockPeriod;

    public MechPresetSO[] ShopItems;
    public bool[] InStock;
    public int DayLeftBeforeRestock;

    private void OnEnable()
    {
        SaveManager.OnReset += Restock;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= Restock;
    }

    public void Restock()
    {
        Debug.Log("Restocking...");
        for (int i = 0; i < 3; i++)
        {
            ShopItems[i].SetRandom(PlayerManager.MechCap);
        }
        InStock = new bool[] {true, true, true};
        DayLeftBeforeRestock = _RestockPeriod;
    }

    public void CheckRestockTime(TimeManager.Date dateBeforeSkip, int skipAmount)
    {
        for (int i = 1; i <= skipAmount; i++)
        {
            DayLeftBeforeRestock--;
            // Generate new quest
            if (DayLeftBeforeRestock <= 0)
            {
                Restock();
            }
        }
    }

    public bool Purchase(int index)
    {
        if (PlayerManager.SpendMoneyIfEnought(500))
        {
            InStock[index] = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
