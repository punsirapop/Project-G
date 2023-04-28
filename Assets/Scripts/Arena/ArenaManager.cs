using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public enum Phase
    {
        SelectTeam,
        SelectEnemy,
        Battle,
        End
    }

    public static event Action<Phase> PhaseChange;

    public static Phase CurrentPhase;

    private void Start()
    {
        ChangePhase(Phase.SelectTeam);
    }

    private void ChangePhase(Phase p)
    {
        CurrentPhase = p;
        PhaseChange?.Invoke(CurrentPhase);
    }
}
