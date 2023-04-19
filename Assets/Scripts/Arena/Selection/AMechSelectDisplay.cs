using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class AMechSelectDisplay : MechCanvasDisplay, IPointerClickHandler
{
    [SerializeField] GameObject[] _Indicators;
    [SerializeField] TextMeshProUGUI[] _Stats;

    public void OnPointerClick(PointerEventData eventData)
    {
        AllyManager.Instance.Selecting(MySO);
    }

    public override void SetChromo(MechChromoSO c)
    {
        base.SetChromo(c);

        _Stats[0].text = c.Atk.Sum().ToString();
        _Stats[1].text = c.Def.Sum().ToString();
        _Stats[2].text = c.Hp.Sum().ToString();
        _Stats[3].text = c.Spd.Sum().ToString();
    }

    public void AdjustingTeam()
    {
        if (AllyManager.Instance.Buttons[AllyManager.Instance.CurrentSelection].MySO == MySO)
            AdjustIndicators(2);
        else if (AllyManager.Instance.AllyTeam.Contains(MySO))
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
