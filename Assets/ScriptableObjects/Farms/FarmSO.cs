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

    //  ------- Constants -------
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private int _BreedCostPerUnit;
    public int BreedCostPerUnit => _BreedCostPerUnit;
    [SerializeField] private float _DiscountRatePerGen;
    public float DiscountRatePerGen => _DiscountRatePerGen;
    [SerializeField] private float _BrokeChance;
    public float BrokeChance => _BrokeChance;

    // Fixing puzzle
    [SerializeField] private JigsawPieceGroupSO[] _ObtainableJisawGroups;
    public JigsawPieceGroupSO[] ObtainableJisawGroups => _ObtainableJisawGroups;

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

    // ------- Variables -------
    // Information
    [SerializeField] private int _Generation;
    public int Generation => _Generation;
    [SerializeField] private Status _Status;
    public Status Status => _Status;
    [SerializeField] private int _Condition;
    public int Condition => _Condition;

    // Prev Farm Display
    BreedMenu.BreedPref _BreedPref;
    public BreedMenu.BreedPref BreedPref => _BreedPref;
    List<Tuple<Properties, int>> _FitnessPref;
    public List<Tuple<Properties, int>> FitnessPref => _FitnessPref;

    // Breeding Request
    BreedMenu.BreedInfo? _BreedInfo;
    public BreedMenu.BreedInfo? BreedInfo => _BreedInfo;
    private float _BreedGauge;
    public float BreedGauge => _BreedGauge;
    private float _GaugePerDay;
    public float GaugePerDay => _GaugePerDay;
    private int _BreedGen;
    public int BreedGen => _BreedGen;


    // items
    [SerializeField] private List<MechChromo> _MechChromos;
    public List<MechChromo> MechChromos => _MechChromos;
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
        _BreedGauge = 0;
        _GaugePerDay = 100;
        _BreedGen = 0;
        _BreedPref = new BreedMenu.BreedPref();
        _FitnessPref = new List<Tuple<Properties, int>>();
        _MechChromos = new List<MechChromo>();
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
        _BreedInfo = f.BreedInfo?.Copy();
        _BreedGauge = f.BreedGauge;
        _GaugePerDay = f.GaugePerDay;
        _BreedGen = f.BreedGen;
        /*
        _BG = f.BG;
        _MainNormal = f.MainNormal;
        _MainBroken = f.MainBroken;
        _Locker = f.Locker;
        */
        _MechChromos = f.MechChromos.ToList();
    }

    // Add new random chromosome to the current space
    public void AddChromo(MechChromo c)
    {
        _MechChromos.Add(c);
    }

    // Delete a chromosome from the current space
    public void DelChromo(MechChromo c)
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

        _BreedInfo = b.Copy();
    }

    public void FillBreedGuage()
    {
        _BreedGauge += GaugePerDay * Condition / 4;
        // Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));

        while (BreedGauge >= 100 && BreedInfo?.MyFarm.BreedPref.BreedGen > 0)
        {
            BreedInfo?.BreedMe();
            // Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));
            _BreedGen++;
            _Generation++;
            _BreedGauge -= 100;
        }
        // Debug.Log(string.Join("-", PlayerManager.FarmDatabase.Select(x => x.MechChromos.Count)));

        if (UnityEngine.Random.Range(0f, 1f) < _BrokeChance)
        {
            BreakingBad();
        }
        if (BreedGen >= BreedInfo?.MyFarm.BreedPref.BreedGen)
        {
            SetBreedRequest(new BreedMenu.BreedInfo());
            _BreedGen = 0;
            _BreedGauge = 0;
            SetStatus(Status.IDLE);
        }
    }

    public void BreakingBad()
    {
        // if (UnityEngine.Random.Range(0, 5) < GenBeforeBreak && Condition > 0)
        if (Condition > 0)
        {
            _Condition--;
        }
        // if (Condition == 0 && Status != Status.BROKEN) SetStatus(Status.BROKEN);
    }

    public void Fixed()
    {
        if (_Condition < 4) _Condition++;
        // SetStatus(BreedInfo.Equals(default(BreedMenu.BreedInfo)) ? Status.IDLE : Status.BREEDING);
    }

    public FarmSaver Save()
    {
        FarmSaver f = new FarmSaver();

        f.LockStatus = LockStatus;
        f.Generation = Generation;
        f.Status = Status;
        f.Condition = Condition;
        f.BreedPref = BreedPref.Copy();
        f.FitnessPrefProperties = FitnessPref.Select(x => x.Item1).ToArray();
        f.FitnessPrefInts = FitnessPref.Select(x => x.Item2).ToArray();
        f.BreedPrefsProperties = (BreedInfo != null) ? BreedInfo?.CurrentPref?.Select(x => x.Item1).ToArray() : null;
        f.BreedPrefsInts = (BreedInfo != null) ? BreedInfo?.CurrentPref?.Select(x => x.Item2).ToArray() : null;
        f.BreedGauge = BreedGauge;
        f.GaugePerDay = GaugePerDay;
        f.BreedGen = BreedGen;
        f.MechSavers = MechChromos.Select(x => x.Save()).ToArray();

        return f;
    }

    public void Load(FarmSaver f)
    {
        _LockStatus = f.LockStatus;
        _Generation = f.Generation;
        _Status = f.Status;
        _Condition = f.Condition;
        _BreedPref = f.BreedPref.Copy();
        _FitnessPref = new List<Tuple<Properties, int>>();
        for (int i = 0; i < f.FitnessPrefProperties.Length; i++)
        {
            _FitnessPref.Add(Tuple.Create(f.FitnessPrefProperties[i], f.FitnessPrefInts[i]));
        }
        List<Tuple<Properties, int>> _BreedPrefTuples = new List<Tuple<Properties, int>>();
        for (int i = 0; i < f.BreedPrefsProperties.Length; i++)
        {
            _BreedPrefTuples.Add(Tuple.Create(f.BreedPrefsProperties[i], f.BreedPrefsInts[i]));
        }
        _BreedInfo = new BreedMenu.BreedInfo(this, _BreedPrefTuples);
        _BreedGauge = f.BreedGauge;
        _GaugePerDay = f.GaugePerDay;
        _BreedGen = f.BreedGen;
        _MechChromos = new List<MechChromo>();
        foreach (var item in f.MechSavers)
        {
            _MechChromos.Add(new MechChromo(item));
        }

        f.MechSavers = MechChromos.Select(x => x.Save()).ToArray();
    }
}

public enum Status
{
    IDLE,
    BREEDING,
    // BROKEN
}