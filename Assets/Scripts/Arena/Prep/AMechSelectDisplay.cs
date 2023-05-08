using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AMechSelectDisplay : MechCanvasDisplay, IPointerClickHandler
{
    [SerializeField] Image _BgColor;
    [SerializeField] GameObject[] _Indicators;
    [SerializeField] TextMeshProUGUI[] _Stats;

    public void OnPointerClick(PointerEventData eventData)
    {
        AllySelectionManager.Instance.SelectingMech(MyMechSO);
    }

    public override void SetChromo(MechChromoSO c)
    {
        base.SetChromo(c);

        _Stats[0].text = c.Atk.Sum().ToString();
        _Stats[1].text = c.Def.Sum().ToString();
        _Stats[2].text = c.Hp.Sum().ToString();
        _Stats[3].text = c.Spd.Sum().ToString();

        Color bg = Color.white;
        foreach (var item in _Stats)
        {
            item.color = Color.black;
        }
        switch (c.Element)
        {
            case MechChromoSO.Elements.Fire:
                bg = Color.red;
                break;
            case MechChromoSO.Elements.Plant:
                bg = Color.green;
                break;
            case MechChromoSO.Elements.Water:
                bg = Color.blue;
                break;
            case MechChromoSO.Elements.Light:
                bg = Color.white;
                break;
            case MechChromoSO.Elements.Dark:
                bg = Color.black;
                foreach (var item in _Stats)
                {
                    item.color = Color.white;
                }
                break;
            case MechChromoSO.Elements.None:
                bg = Color.gray;
                break;
        }
        bg.a = .5f;
        _BgColor.color = bg;
    }

    public void AdjustingTeam()
    {
        if (AllySelectionManager.Instance.Buttons[AllySelectionManager.Instance.CurrentSelection]
            .MyMechSO == MyMechSO)
            AdjustIndicators(2);
        else if (AllySelectionManager.Instance.AllyMech.Contains(MyMechSO))
            AdjustIndicators(1);
        else AdjustIndicators(0);
    }

    /*
     * 0 - none
     * 1 - select
     * 2 - alr in team
     */
    public void AdjustIndicators(int mode)
    {
        foreach (var item in _Indicators)
        {
            item.SetActive(false);
        }
        if (mode > 0) _Indicators[mode - 1].SetActive(true);
    }
}
