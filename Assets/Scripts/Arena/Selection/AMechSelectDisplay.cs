using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AMechSelectDisplay : MechCanvasDisplay
{
    [SerializeField] GameObject[] _Indicators;
    [SerializeField] TextMeshProUGUI[] _Stats;

    public override void SetChromo(MechChromoSO c)
    {
        base.SetChromo(c);

        _Stats[0].text = c.Atk.Sum().ToString();
        _Stats[1].text = c.Def.Sum().ToString();
        _Stats[2].text = c.Hp.Sum().ToString();
        _Stats[3].text = c.Spd.Sum().ToString();
    }

    /*
     * 0 - clear
     * 1 - select
     * 2 - select batch
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
