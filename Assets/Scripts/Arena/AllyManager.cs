using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    public static AllyManager Instance;
    public static event Action OnTeamChange;

    public int CurrentSelection;

    [SerializeField] GameObject _SelectionPanel;
    [SerializeField] AMechButtonDisplay[] _Buttons;
    public AMechButtonDisplay[] Buttons => _Buttons;
    [SerializeField] ArenaMechDisplay[] _MechDisplays;
    [SerializeField] Transform _MechHolder;

    MechChromoSO[] _AllyTeam => _Buttons.Select(x => x.MySO).ToArray();
    public MechChromoSO[] AllyTeam => _AllyTeam;

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
        _MechHolder.BroadcastMessage("AdjustingTeam",SendMessageOptions.DontRequireReceiver);
    }

    public void Selecting(MechChromoSO m)
    {
        if (!AllyTeam.Contains(m) || m == AllyTeam[CurrentSelection])
        {
            _Buttons[CurrentSelection].SetChromo(m);
            _MechDisplays[CurrentSelection].SetChromo(m);
            // AdjustTeamList(m);
            _MechHolder.BroadcastMessage("AdjustingTeam",SendMessageOptions.DontRequireReceiver);
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
            item.SetChromo(item.MySO);
        }
        foreach (var item in _MechDisplays)
        {
            item.SetChromo(item.MySO);
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
