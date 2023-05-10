using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AllySelectionManager : MonoBehaviour
{
    public static AllySelectionManager Instance;

    // Current farm & mech index
    public int CurrentSelection;

    [SerializeField] GameObject _SelectionPanel;

    // Team Detail setup displays
    [SerializeField] AMechButtonDisplay[] _Buttons;
    public AMechButtonDisplay[] Buttons => _Buttons;
    // Mech line-up
    [SerializeField] ArenaMechDisplay[] _MechDisplays;

    // Mech & weapon selection holders
    [SerializeField] Transform[] _Holders;

    public MechChromo[] AllyMech => _Buttons.Select(x => x.MyMechSO).ToArray();
    public WeaponChromosome[] AllyWeapon => _Buttons.Select(x => x.MyWeaponSO).ToArray();

    private void Start()
    {
        if (Instance == null) Instance = this;

        CurrentSelection = -1;
    }

    // Select which position to edit
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

    // Message Sent from mech selection panel
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

    // Message Sent from weapon selection panel
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

    // Close selector
    public void CloseSelector()
    {
        _SelectionPanel.SetActive(false);
        foreach (var item in _Buttons)
        {
            item.SetButton(true);
        }
    }

    // Remove all selected mech and weapon
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

    // Return from battle - turn on mechs
    public void BackToSelection()
    {
        foreach (var item in _MechDisplays)
        {
            item.gameObject.SetActive(true);
        }
    }

    /*
    public void AdjustTeamList(MechChromoSO c)
    {
        if (AllyTeam.Contains(c)) _AllyTeam.Remove(c);
        else _AllyTeam.Add(c);
    }
    */
}
