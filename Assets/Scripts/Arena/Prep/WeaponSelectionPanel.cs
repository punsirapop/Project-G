using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WeaponSelectionPanel : SelectionPanel
{
    // FactorySO _MyFactory => PlayerManager.FactoryDatabase[_CurrentPanel];
    WeaponChromosome[][] _Chromosomes;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < PlayerManager.FactoryDatabase.Length; i++)
        {
            _Buttons[i].interactable = PlayerManager.FactoryDatabase[i].LockStatus == LockableStatus.Unlock;
        }

        CreateWeaponStorage();

        OpenPanel(0);
    }

    public override void UpdateValue()
    {
        base.UpdateValue();

        if (_Chromosomes[_CurrentPanel].Length > 0)
        {
            foreach (var item in _Chromosomes[_CurrentPanel])
            {
                GameObject m = _Pool.Get();
                m.GetComponent<AWeaponSelectDisplay>().SetChromo(item);
            }
            _ContentStorage.BroadcastMessage("AdjustingTeam", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AutoSelect()
    {
        List<WeaponChromosome> c = new List<WeaponChromosome>();
        for (int i = 0; i < 4; i++)
        {
            c.AddRange(_Chromosomes[i].Take(5));
        }
        int tmp = AllySelectionManager.Instance.CurrentSelection;
        List<int> ints = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            AllySelectionManager.Instance.CurrentSelection = i;
            int r;
            do
            {
                r = Random.Range(0, c.Count());
            } while (ints.Contains(r));
            c[r].SetIsMode1Active(Random.Range(0, 2) == 0);
            AllySelectionManager.Instance.SelectingWeapon(c[r]);
            ints.Add(r);
        }
        AllySelectionManager.Instance.CurrentSelection = tmp;
    }

    public void CreateWeaponStorage()
    {
        if (_Chromosomes == null)
        {
            _Chromosomes = new WeaponChromosome[4][];

            for (int i = 0; i < 4; i++)
            {
                _Chromosomes[i] = PlayerManager.FactoryDatabase[i].GetAllWeapon()
                    .OrderByDescending(x => x.Fitness).ToArray();
            }
        }
    }
}
