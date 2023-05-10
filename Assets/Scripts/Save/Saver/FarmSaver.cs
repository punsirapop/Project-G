using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FarmXaver
{
    public List<FarmSaver> FarmSavers;

    public FarmXaver(List<FarmSaver> farmSavers)
    {
        FarmSavers = farmSavers;
    }
}

[Serializable]
public class FarmSaver
{
    public LockableStatus LockStatus;
    public int Generation;
    public Status Status;
    public int Condition;

    public BreedMenu.BreedPref BreedPref;
    public FitnessMenu.Properties[] FitnessPrefProperties;
    public int[] FitnessPrefInts;
    public FitnessMenu.Properties[] BreedPrefsProperties;
    public int[] BreedPrefsInts;

    public float BreedGauge;
    public float GaugePerDay;
    public int BreedGen;

    public MechSaver[] MechSavers;
}

[Serializable]
public class MechSaver
{
    public int ID;
    public int Head;
    public int[] Body;
    public int Acc;
    public int[] Atk;
    public int[] Def;
    public int[] Hp;
    public int[] Spd;
}
