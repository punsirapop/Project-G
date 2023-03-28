using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Farm")]
public class FarmSO : ScriptableObject
{
    // Invoke when changing state
    public static event Action<FarmSO, Status> OnFarmChangeStatus;

    // Information
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private int _Generation;
    public int Generation => _Generation;
    [SerializeField] private Status _Status;
    public Status Status => _Status;
    private int _Condition;
    public int Condition => _Condition;

    // Breeding Request
    BreedMenu.BreedInfo _BreedInfo;
    public BreedMenu.BreedInfo BreedInfo => _BreedInfo;
    private float _BreedGuage;
    public float BreedGuage => _BreedGuage;
    private float _GuagePerDay;
    public float GuagePerDay => _GuagePerDay;
    private int _BreedGen;
    public int BreedGen => _BreedGen;
    private int _DaysBeforeBreak;
    public int DaysBeforeBreak => _DaysBeforeBreak;

    // Interior Sprites
    [SerializeField] private Sprite _BG;
    public Sprite BG => _BG;

    // Exterior Sprites
    [SerializeField] private Sprite _MainNormal;
    public Sprite MainNormal => _MainNormal;
    [SerializeField] private Sprite _MainBroken;
    public Sprite MainBroken => _MainBroken;
    [SerializeField] private Sprite _Locker;
    public Sprite Locker => _Locker;

    // items
    [SerializeField] private List<MechChromoSO> _MechChromos;
    public List<MechChromoSO> MechChromos => _MechChromos;

    private void OnEnable()
    {
        SaveManager.OnReset += ResetMe;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= ResetMe;
    }

    // Reset this farm
    private void ResetMe()
    {
        Debug.Log("RESET FROM FARM");
        _Generation = 0;
        _Status = 0;
        _Condition = 4;
        _BreedInfo = new BreedMenu.BreedInfo();
        _BreedGuage = 0;
        _GuagePerDay = 100;
        _BreedGen = 0;
        _DaysBeforeBreak = 0;
        _MechChromos = new List<MechChromoSO>();
    }

    /*
    // Set farm from value
    public void SetMe(FarmSO f)
    {
        _Name = f.Name;
        _Generation = f.Generation;
        _Status = f.Status;
        _Condition = f.Condition;
        _BreedInfo = f.BreedInfo;
        _BreedGuage = f.BreedGuage;
        _GuagePerDay = f.GuagePerDay;
        _BreedGen = f.BreedGen;
        _GenBeforeBreak = f.GenBeforeBreak;
        _BG = f.BG;
        _MainNormal = f.MainNormal;
        _MainBroken = f.MainBroken;
        _Locker = f.Locker;
        _MechChromos = new List<MechChromoSO>(f.MechChromos);
        _BreedInfo = f._BreedInfo;
    }
    */

    // Add new random chromosome to the current space
    public void AddChromo(MechChromoSO c)
    {
        _MechChromos.Add(c);
    }

    // Delete a chromosome from the current space
    public void DelChromo(MechChromoSO c)
    {
        _MechChromos.Remove(c);
    }

    public void SetStatus(Status s)
    {
        _Status = s;
        OnFarmChangeStatus?.Invoke(this, Status);
    }

    public void SetBreedRequest(BreedMenu.BreedInfo b)
    {
        TimeManager.Date targetDate = new TimeManager.Date();
        targetDate.AddDay(PlayerManager.CurrentDate.ToDay() + b.BreedGen);
        _BreedInfo = b;
    }

    public void FillBreedGuage()
    {
        _BreedGuage += GuagePerDay * Condition / 4;
        Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));

        while (BreedGuage >= 100 && BreedInfo.BreedGen > 0)
        {
            BreedInfo.BreedMe();
            Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));
            _BreedGen++;
            _Generation++;
            _BreedGuage -= 100;
        }
        Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));

        _DaysBeforeBreak++;
        BreakingBad();
        if (BreedGen >= BreedInfo.BreedGen)
        {
            SetBreedRequest(new BreedMenu.BreedInfo());
            _BreedGuage = 0;
            SetStatus(Status.IDLE);
        }
    }

    public void BreakingBad()
    {
        // if (UnityEngine.Random.Range(0, 5) < GenBeforeBreak && Condition > 0)
        if (Condition > 0)
        {
            _DaysBeforeBreak = 0;
            _Condition--;
        }
        // if (Condition == 0 && Status != Status.BROKEN) SetStatus(Status.BROKEN);
    }

    public void Fixed()
    {
        _Condition++;
        // SetStatus(BreedInfo.Equals(default(BreedMenu.BreedInfo)) ? Status.IDLE : Status.BREEDING);
    }
}

public enum Status
{
    IDLE,
    BREEDING,
    // BROKEN
}