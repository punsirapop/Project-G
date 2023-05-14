using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FactoryXaver
{
    public List<FactorySaver> FacSavers;
    public FactoryXaver(List<FactorySaver> facSavers)
    {
        FacSavers = facSavers;
    }
}

[Serializable]
public class FactorySaver
{
    public LockableStatus LockStatus;
    public int Generation;
    public Status Status;
    public int Condition;
    public bool IsEncode;

    public FactoryProduction.BreedPref BreedPref;

    public float BreedGauge;
    public float GaugePerDay;
    public int BreedGen;
    public int PopulationCount;

    public BitChromoDatabase.BitChromosome[] Population;
    public int[] BitChromoInfo;
}
