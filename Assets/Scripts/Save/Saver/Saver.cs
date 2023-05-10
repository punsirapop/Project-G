using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Saver
{
    public int MechIDCounter;
    public int MechCap;
    public int Days;
    public int Money;

    // ContentChapSO
    public LockableStatus[] CCLockStatus;

    // JigsawSO
    public LockableStatus[] JigsawLockStatus;
    public int[] SuccessCounts;
    public int[] FailCounts;

    // InfoSO
    public bool[] IsNeverShow;

    // ShopSO
    public MechSaver[] ShopItems;
    public bool[] InStock;
    public int DayLeftBeforeRestock;

    // CappySO
    public float CumulativeSpawnChance;
    public bool IsFirstSpawn;
    public LockableStatus[] CappyLockStatus;
    public int[] FoundCount;
}
