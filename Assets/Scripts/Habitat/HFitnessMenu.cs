using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFitnessMenu : FitnessMenu
{
    [SerializeField] int _MyIndex;

    protected override FarmSO myFarm => PlayerManager.FarmDatabase[_MyIndex];
}
