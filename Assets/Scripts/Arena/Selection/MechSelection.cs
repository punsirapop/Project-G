using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using static ChromoMenu;
using static FitnessMenu;

public class MechSelection : SelectionPanel
{
    [SerializeField] TMP_Dropdown _Pref;

    FarmSO _MyFarm => PlayerManager.FarmDatabase[_CurrentPanel];

    protected override void Start()
    {
        base.Start();
        OpenPanel(0);
    }

    private Dictionary<dynamic, List<float>> GetFitnessDict()
    {
        List<Tuple<Properties, int>> fv = new List<Tuple<Properties, int>>();
        var dict = new Dictionary<dynamic, List<float>>();

        for (int i = 0; i < 4; i++) fv.Add(Tuple.Create(Properties.Com, i));

        foreach (MechChromoSO c in _MyFarm.MechChromos)
        {
            List<float> list = new List<float>();
            list.Add(c.GetFitness(fv));
            if(_Pref.value > 0)
            {
                list.Add(c.GetFitness(new List<Tuple<Properties, int>>()
                { Tuple.Create(Properties.Com, _Pref.value - 1) }));
            }
            dict.Add(c, list);
        }

        return dict;
    }

    public override void UpdateValue()
    {
        base.UpdateValue();

        if (_MyFarm.MechChromos.Count > 0)
        {
            Dictionary<dynamic, List<float>> fvDict = GetFitnessDict();
            /*
            List<OrderFormat> fv = new List<OrderFormat>();
            foreach (var item in fvDict)
            {
                MechChromoSO c = item.Key;
                OrderFormat of = new OrderFormat();
                of.name = c.ID;
                of.chromo = c;
                of.fitness = item.Value;
                fv.Add(of);
            }
            */
            // ------- sort -------
            fvDict = fvDict.OrderByDescending(x => x.Value[0]).
                ToDictionary(x => x.Key, x => x.Value);
            if(_Pref.value > 0)
            fvDict = fvDict.OrderByDescending(x => x.Value[1]).
                ToDictionary(x => x.Key, x => x.Value);
            // fv = fv.OrderByDescending(x => x.fitness).ThenBy(x => x.name).ToList();
            // ------- display -------
            foreach (var item in fvDict)
            {
                GameObject m = _Pool.Get();
                m.GetComponent<MechCanvasDisplay>().SetChromo(item.Key);
            }
        }
    }
}
