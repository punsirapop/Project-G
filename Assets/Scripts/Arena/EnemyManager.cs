using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FitnessMenu;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] MechCanvasDisplay[] _MechIcons;
    [SerializeField] AMechDisplay[] _MechBars;
    [SerializeField] ArenaMechDisplay[] _MechLineUp;

    List<MechChromoSO> _EnemyPool;
    List<List<MechChromoSO>> _EnemyParties;

    private void Start()
    {
        _EnemyPool = new List<MechChromoSO>();
        _EnemyParties = new List<List<MechChromoSO>>();

        for (int i = 0; i < 10; i++)
        {
            _EnemyPool.Add(ScriptableObject.CreateInstance(typeof(MechChromoSO)) as MechChromoSO);
            _EnemyPool.Last().SetRandomStat(MechChromoSO.Cap + 2);
            MechChromoSO.IDCounter--;
        }

        List<MechChromoSO> list = new List<MechChromoSO>();
        list.Add(GetFitnessDict(_EnemyPool, 1).Where(x => !list.Contains(x.Key)).
            OrderByDescending(x => x.Value[0]).ThenByDescending(x => x.Value[1]).First().Key);
        list.Insert(0, GetFitnessDict(_EnemyPool, 2).Where(x => !list.Contains(x.Key)).
            OrderByDescending(x => x.Value[0]).ThenByDescending(x => x.Value[1]).First().Key);
        list.Insert(1, GetFitnessDict(_EnemyPool, 0).Where(x => !list.Contains(x.Key)).
            OrderByDescending(x => x.Value[0]).First().Key);

        _EnemyParties.Add(list);

        _EnemyPool = new List<MechChromoSO>();

        for (int i = 0; i < 10; i++)
        {
            _EnemyPool.Add(ScriptableObject.CreateInstance(typeof(MechChromoSO)) as MechChromoSO);
            _EnemyPool.Last().SetRandomStat(MechChromoSO.Cap);
            MechChromoSO.IDCounter--;
        }

        list = new List<MechChromoSO>();
        list.Add(GetFitnessDict(_EnemyPool, 1).Where(x => !list.Contains(x.Key)).
            OrderByDescending(x => x.Value[0]).ThenByDescending(x => x.Value[1]).First().Key);
        list.Insert(0, GetFitnessDict(_EnemyPool, 2).Where(x => !list.Contains(x.Key)).
            OrderByDescending(x => x.Value[0]).ThenByDescending(x => x.Value[1]).First().Key);
        list.Insert(1, GetFitnessDict(_EnemyPool, 0).Where(x => !list.Contains(x.Key)).
            OrderByDescending(x => x.Value[0]).First().Key);

        _EnemyParties.Add(list);

        foreach (var item in _MechIcons.Zip(_EnemyParties.SelectMany(x => x), (a, b) => Tuple.Create(a, b)))
        {
            item.Item1.SetChromo(item.Item2);
        }
    }

    /*
     * 0 - hard
     * 1 - easy
     */
    public void SetLineUp(int m)
    {
        for (int i = 0; i < 3; i++)
        {
            _MechLineUp[i].SetChromo(_EnemyParties[m][i]);
            _MechBars[i].SetChromo(_EnemyParties[m][i]);
        }
    }

    public void CloseLineUp()
    {
        for (int i = 0; i < 3; i++)
        {
            _MechLineUp[i].SetChromo(_MechLineUp[i].MySO);
            _MechBars[i].SetChromo(_MechBars[i].MySO);
        }
    }

    /*
     * 0 - default
     * 1 - offensive
     * 2 - defensive
     */
    private Dictionary<dynamic, List<float>> GetFitnessDict(List<MechChromoSO> m, int mode)
    {
        List<Tuple<Properties, int>> fv = new List<Tuple<Properties, int>>();
        var dict = new Dictionary<dynamic, List<float>>();

        for (int i = 0; i < 4; i++) fv.Add(Tuple.Create(Properties.Com, i));

        foreach (MechChromoSO c in m)
        {
            List<float> list = new List<float>();
            list.Add(c.GetFitness(fv));
            switch (mode)
            {
                case 1:
                    list.Add(c.GetFitness(new List<Tuple<Properties, int>>()
                        { Tuple.Create(Properties.Com, 0) }) +
                        c.GetFitness(new List<Tuple<Properties, int>>()
                        { Tuple.Create(Properties.Com, 3) }));
                    break;
                case 2:
                    list.Add(c.GetFitness(new List<Tuple<Properties, int>>()
                        { Tuple.Create(Properties.Com, 1) }) +
                        c.GetFitness(new List<Tuple<Properties, int>>()
                        { Tuple.Create(Properties.Com, 2) }));
                    break;
                default:
                    break;
            }

            dict.Add(c, list);
        }

        return dict;
    }
}
