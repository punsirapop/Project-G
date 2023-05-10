using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour
{
    [SerializeField] MechCanvasDisplay _MyMech;
    [SerializeField] TextMeshProUGUI[] _Stats;
    [SerializeField] GameObject _OutOfStock;
    [SerializeField] Button _Purchase;

    public void UpdateChromo(MechPresetSO m, bool b)
    {
        if (b)
        {
            _MyMech.SetChromoFromPreset(m);
            _Stats[0].text = $"Head: {m.Head}";
            _Stats[1].text = $"Body: {string.Join("/",m.Body)}";
            _Stats[2].text = $"Acc: {m.Acc}";
            _Stats[3].text = $" {m.Atk.Sum()}";
            _Stats[4].text = $" {m.Def.Sum()}";
            _Stats[5].text = $" {m.Hp.Sum()}";
            _Stats[6].text = $" {m.Spd.Sum()}";
        }
        _OutOfStock.SetActive(!b);
        _Purchase.interactable = b && PlayerManager.Money >= 500;
    }
}
