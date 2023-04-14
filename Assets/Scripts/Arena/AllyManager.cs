using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    public static event Action OnTeamChange;

    public int Selecting;

    [SerializeField] GameObject _SelectionPanel;
    [SerializeField] AMechAllyDisplay[] _Buttons;

    List<MechChromoSO> _AllyTeam;
    public List<MechChromoSO> AllyTeam => _AllyTeam;

    private void Start()
    {
        Selecting = -1;
        _AllyTeam = new List<MechChromoSO>();
    }

    public void OnSelecting(int index)
    {
        Selecting = index;
        _SelectionPanel.SetActive(true);
        foreach (var item in _Buttons)
        {
            item.SetButton(true);
        }
        _Buttons[index].SetButton(false);
    }

    public void CloseSelection()
    {
        _SelectionPanel.SetActive(false);
        foreach (var item in _Buttons)
        {
            item.SetButton(true);
        }
    }
}
