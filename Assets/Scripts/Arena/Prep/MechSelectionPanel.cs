using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static FitnessMenu;

public class MechSelectionPanel : SelectionPanel
{
    [SerializeField] TMP_Dropdown _Pref;

    FarmSO _MyFarm => PlayerManager.FarmDatabase[_CurrentPanel];

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < PlayerManager.FarmDatabase.Length; i++)
        {
            _Buttons[i].interactable = PlayerManager.FarmDatabase[i].LockStatus == LockableStatus.Unlock;
        }
        OpenPanel(0);
    }

    private Dictionary<MechChromo, List<float>> GetFitnessDict(List<MechChromo> m)
    {
        List<Tuple<Properties, int>> fv = new List<Tuple<Properties, int>>();
        var dict = new Dictionary<MechChromo, List<float>>();

        for (int i = 0; i < 4; i++) fv.Add(Tuple.Create(Properties.Com, i));

        foreach (MechChromo c in m)
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
            Dictionary<MechChromo, List<float>> fvDict = GetFitnessDict(_MyFarm.MechChromos);
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
            _ContentStorage.BroadcastMessage("AdjustingTeam", SendMessageOptions.DontRequireReceiver);

        }
    }

    public void AutoSelect()
    {
        List<MechChromo> list = PlayerManager.FarmDatabase.SelectMany(x => x.MechChromos).ToList();
        Dictionary<MechChromo, List<float>> fvDict = GetFitnessDict(list);
        fvDict = fvDict.OrderByDescending(x => x.Value[0]).ToDictionary(x => x.Key, x => x.Value);
        int tmp = AllySelectionManager.Instance.CurrentSelection;
        for (int i = 0; i < 3; i++)
        {
            AllySelectionManager.Instance.CurrentSelection = i;
            AllySelectionManager.Instance.SelectingMech(fvDict.Skip(i).First().Key);
        }
        AllySelectionManager.Instance.CurrentSelection = tmp;
        // _ContentStorage.BroadcastMessage("AdjustingTeam", SendMessageOptions.DontRequireReceiver);
    }
}
