using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AllySelectionManager : MonoBehaviour
{
    public static AllySelectionManager Instance;
    public static event Action OnTeamChange;

    public int CurrentSelection;

    [SerializeField] GameObject _SelectionPanel;
    [SerializeField] AMechButtonDisplay[] _Buttons;
    public AMechButtonDisplay[] Buttons => _Buttons;
    [SerializeField] ArenaMechDisplay[] _MechDisplays;
    [SerializeField] Transform[] _Holders;

    public MechChromo[] AllyMech => _Buttons.Select(x => x.MyMechSO).ToArray();
    public WeaponChromosome[] AllyWeapon => _Buttons.Select(x => x.MyWeaponSO).ToArray();

    private void Start()
    {
        if (Instance == null) Instance = this;

        CurrentSelection = -1;
    }

    public void OpenSelector(int index)
    {
        CurrentSelection = index;
        _SelectionPanel.SetActive(true);
        foreach (var item in _Buttons)
        {
            item.SetButton(true);
        }
        _Buttons[index].SetButton(false);
        _Holders[0].BroadcastMessage("AdjustingTeam",SendMessageOptions.DontRequireReceiver);
        _Holders[1].BroadcastMessage("AdjustingTeam",SendMessageOptions.DontRequireReceiver);
    }

    public void SelectingMech(MechChromo m)
    {
        if (!AllyMech.Contains(m) || m == AllyMech[CurrentSelection])
        {
            _Buttons[CurrentSelection].SetChromo(m);
            _MechDisplays[CurrentSelection].SetChromo(m);
            // AdjustTeamList(m);
            _Holders[0].BroadcastMessage("AdjustingTeam",SendMessageOptions.DontRequireReceiver);
        }
    }

    public void SelectingWeapon(WeaponChromosome w)
    {
        if (!AllyWeapon.Contains(w) || w == AllyWeapon[CurrentSelection])
        {
            _Buttons[CurrentSelection].SetWeapon(w);
            _MechDisplays[CurrentSelection].SetWeapon(w);
            // AdjustTeamList(m);
            _Holders[1].BroadcastMessage("AdjustingTeam", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void CloseSelector()
    {
        _SelectionPanel.SetActive(false);
        foreach (var item in _Buttons)
        {
            item.SetButton(true);
        }
    }

    public void ClearSelection()
    {
        foreach (var item in _Buttons)
        {
            item.SetChromo(item.MyMechSO);
            item.SetWeapon(item.MyWeaponSO);
        }
        foreach (var item in _MechDisplays)
        {
            item.SetChromo(item.MySO);
        }
    }

    public void BackToSelection()
    {
        foreach (var item in _MechDisplays)
        {
            item.gameObject.SetActive(true);
        }
        if (BattleManager.WinningStatus == 1) ClearSelection();
    }

    /*
    public void AdjustTeamList(MechChromoSO c)
    {
        if (AllyTeam.Contains(c)) _AllyTeam.Remove(c);
        else _AllyTeam.Add(c);
    }
    */
}
