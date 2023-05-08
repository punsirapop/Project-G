using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FitnessMenu;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Farm")]
public class FarmSO : LockableObject
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
    [SerializeField] private int _BreedCostPerUnit;
    public int BreedCostPerUnit => _BreedCostPerUnit;
    [SerializeField] private float _DiscountRatePerGen;
    public float DiscountRatePerGen => _DiscountRatePerGen;
    private int _Condition;
    public int Condition => _Condition;
    [SerializeField] private float _BrokeChance;
    public float BrokeChance => _BrokeChance;

    // Fixing puzzle
    [SerializeField] private JigsawPieceGroupSO[] _ObtainableJisawGroups;
    public JigsawPieceGroupSO[] ObtainableJisawGroups => _ObtainableJisawGroups;

    // Prev Farm Display
    BreedMenu.BreedPref _BreedPref;
    public BreedMenu.BreedPref BreedPref => _BreedPref;
    List<Tuple<Properties, int>> _FitnessPref;
    public List<Tuple<Properties, int>> FitnessPref => _FitnessPref;

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
    public int PopulationCount => _MechChromos.Count;

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
        base.Reset();
        if (Name == "Habitat") _LockStatus = LockableStatus.Unlock;
        _Generation = 0;
        _Status = 0;
        _Condition = 4;
        _BreedInfo = new BreedMenu.BreedInfo();
        _BreedGuage = 0;
        _GuagePerDay = 100;
        _BreedGen = 0;
        _DaysBeforeBreak = 0;
        _BreedPref = new BreedMenu.BreedPref();
        _FitnessPref = new List<Tuple<Properties, int>>();
        _MechChromos = new List<MechChromoSO>();
    }

    public override string GetRequirementPrefix()
    {
        return "Build";
    }
    public override string GetLockableObjectName()
    {
        return _Name;
    }

    // Set farm from value
    public void SetMe(FarmSO f)
    {
        Debug.Log("FARM SET");
        /*
        _LockStatus = f._LockStatus;
        _RequiredMoney = f.RequiredMoney;
        _RequiredObjects = f.RequiredObjects.ToArray();
        */
        _Name = f.Name;
        _Generation = f.Generation;
        _Status = f.Status;
        _BreedCostPerUnit = f.BreedCostPerUnit;
        _DiscountRatePerGen = f.DiscountRatePerGen;
        _Condition = f.Condition;
        _BrokeChance = f.BrokeChance;
        _ObtainableJisawGroups = f.ObtainableJisawGroups.ToArray();
        _BreedPref = f.BreedPref.Copy();
        _FitnessPref = f.FitnessPref?.ToList();
        _BreedInfo = f.BreedInfo.Copy();
        _BreedGuage = f.BreedGuage;
        _GuagePerDay = f.GuagePerDay;
        _BreedGen = f.BreedGen;
        _DaysBeforeBreak = f.DaysBeforeBreak;
        /*
        _BG = f.BG;
        _MainNormal = f.MainNormal;
        _MainBroken = f.MainBroken;
        _Locker = f.Locker;
        */
        _MechChromos = f.MechChromos.ToList();
    }

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
        Debug.Log("INVOKERR");
        OnFarmChangeStatus?.Invoke(this, Status);
    }

    public void SetFitnessPref(List<Tuple<Properties, int>> p)
    {
        _FitnessPref = p;
    }

    public void SetBreedPref(BreedMenu.BreedPref b)
    {
        _BreedPref = b.Copy();
    }

    public void SetBreedRequest(BreedMenu.BreedInfo b)
    {
        if (!b.Equals(default(BreedMenu.BreedInfo)))
        {
            TimeManager.Date targetDate = new TimeManager.Date();
            targetDate.AddDay(PlayerManager.CurrentDate.ToDay() + b.MyFarm.BreedPref.BreedGen);
        }

        _BreedInfo = b;
    }

    public void FillBreedGuage()
    {
        _BreedGuage += GuagePerDay * Condition / 4;
        // Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));

        while (BreedGuage >= 100 && BreedInfo.MyFarm.BreedPref.BreedGen > 0)
        {
            BreedInfo.BreedMe();
            // Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));
            _BreedGen++;
            _Generation++;
            _BreedGuage -= 100;
        }
        // Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));

        _DaysBeforeBreak++;
        if (UnityEngine.Random.Range(0f, 1f) < _BrokeChance)
        {
            BreakingBad();
        }
        if (BreedGen >= BreedInfo.MyFarm.BreedPref.BreedGen)
        {
            SetBreedRequest(new BreedMenu.BreedInfo());
            _BreedGen = 0;
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
        if (_Condition < 4) _Condition++;
        // SetStatus(BreedInfo.Equals(default(BreedMenu.BreedInfo)) ? Status.IDLE : Status.BREEDING);
    }
}

public enum Status
{
    IDLE,
    BREEDING,
    // BROKEN
}