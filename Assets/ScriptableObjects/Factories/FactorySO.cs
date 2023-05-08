using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Factory")]
public class FactorySO : LockableObject
{
    [Header("Factory information")]
    // Factory informations
    [SerializeField] private int _FactoryIndex;
    public int FactoryIndex => _FactoryIndex;
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] [TextArea(3, 10)] private string _Description;
    public string Description => _Description;
    [SerializeField] private int _Generation;
    public int Generation => _Generation;
    [SerializeField] private Status _Status;
    public Status Status => _Status;
    [SerializeField] private int _BreedCostPerUnit;
    public int BreedCostPerUnit => _BreedCostPerUnit;
    [SerializeField] private float _DiscountRatePerGen;
    public float DiscountRatePerGen => _DiscountRatePerGen;
    private int _Condition; // If Condition remain 0, the facility completely broken
    public int Condition => _Condition;
    [SerializeField] private float _BrokeChance;
    private bool _isEncode = false;  // Variable to help switching generate puzzle between decode and encode

    // Fixing puzzle
    [SerializeField] private JigsawPieceGroupSO[] _ObtainableJisawGroups;
    public JigsawPieceGroupSO[] ObtainableJisawGroups => _ObtainableJisawGroups;

    // Breeding Request
    FactoryProduction.BreedPref _BreedPref;
    public FactoryProduction.BreedPref BreedPref => _BreedPref;
    FactoryProduction.BreedInfo _BreedInfo;
    public FactoryProduction.BreedInfo BreedInfo => _BreedInfo;
    private float _BreedGuage;
    public float BreedGuage => _BreedGuage;
    private float _GuagePerDay;
    public float GuagePerDay => _GuagePerDay;
    private int _BreedGen;
    public int BreedGen => _BreedGen;

    // Interior Sprites
    [Header("Sprites")]
    [SerializeField] private Sprite _Floor;
    public Sprite Floor => _Floor;
    [SerializeField] private Sprite _Conveyor;
    public Sprite Conveyor => _Conveyor;
    [SerializeField] private Sprite _Border;
    public Sprite Border => _Border;

    // Exterior Sprites
    [SerializeField] private Sprite _MainNormal;
    public Sprite MainNormal => _MainNormal;
    [SerializeField] private Sprite _MainBroken;
    public Sprite MainBroken => _MainBroken;
    [SerializeField] private Sprite _Locker;
    public Sprite Locker => _Locker;

    [Space(10)]
    [Header("Knapsack problem information")]
    // Knapsack and items preset
    [SerializeField] private KnapsackSO[] _Knapsacks;
    public KnapsackSO[] Knapsacks => _Knapsacks;
    [SerializeField] private ItemSO[] _Items;
    public ItemSO[] Items => _Items;
    [SerializeField] private int _MaxFitness;
    public int MaxFitness => _MaxFitness;

    // Chromosome population database
    [SerializeField] private BitChromoDatabase _ChromoDatabase;
    [SerializeField] private int _PopulationCount;
    public int PopulationCount => _PopulationCount;

    [Space(10)]
    [Header("Weapon information")]
    // Weapon information
    [SerializeField] private string _WeaponPrefix;
    [SerializeField] private string _WeaponIdFormat;
    [SerializeField] private WeaponMode _ProducedWeaponMode1;
    [SerializeField] private WeaponMode _ProducedWeaponMode2;
    [SerializeField] private float _MinCooldownSeconds;
    [SerializeField] private float _MaxCooldownSeconds;
    [SerializeField] private WeaponRankInfo[] _WeaponRankConfigDesc;

    [System.Serializable]
    public struct WeaponRankInfo
    {
        public WeaponRank Rank;
        public int MinFitness;
        public Sprite Image;
        public Sprite BigImage;
        public int BonusAtk;
        public int BonusDef;
        public int BonusHp;
        public int BonusSpd;
    }

    #region Getter, Setter, Reset
    public override string GetRequirementPrefix()
    {
        return "Build";
    }
    public override string GetLockableObjectName()
    {
        return _Name;
    }

    // Return current value and flip it's value afterward
    public bool GetIsEncode()
    {
        _isEncode = !_isEncode;
        return !_isEncode;
    }

    public void SetStatus(Status newStatus)
    {
        _Status = newStatus;
    }

    public void SetBreedPref(FactoryProduction.BreedPref newBreedPref)
    {
        _BreedPref = newBreedPref.Copy();
    }

    public void SetBreedRequest(FactoryProduction.BreedInfo newBreedInfo)
    {
        if (!newBreedInfo.Equals(default(FactoryProduction.BreedInfo)))
        {
            TimeManager.Date targetDate = new TimeManager.Date();
            targetDate.AddDay(PlayerManager.CurrentDate.ToDay() + newBreedInfo.MyFactory.BreedPref.BreedGen);
        }

        _BreedInfo = newBreedInfo;
    }

    public void SetBitChromoDatabase(int[][][] newBitstringArray)
    {
        _ChromoDatabase.SetDatabase(newBitstringArray);
    }

    private void OnEnable()
    {
        SaveManager.OnReset += Reset;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= Reset;
    }

    public new void Reset()
    {
        base.Reset();
        _Generation = 0;
        _Status = Status.IDLE;
        _Condition = 4;
        _isEncode = false;
        _BreedGuage = 0;
        _GuagePerDay = 100;
        _BreedGen = 0;
        _PopulateDatabaseIfNot(forcePopulate: true);
    }
    #endregion

    #region Weapon Chromosome
    // Populate database in case if it's not populated yet
    private void _PopulateDatabaseIfNot(bool forcePopulate=false)
    {
        if (!_ChromoDatabase.IsValid(_Knapsacks.Length, _Items.Length, _PopulationCount) ||
            forcePopulate)
        {
            _ChromoDatabase.SetChromoDimension(_Knapsacks.Length);
            _ChromoDatabase.SetChromoLength(_Items.Length);
            _ChromoDatabase.Populate(_PopulationCount);
        }
    }

    // Generate random valid bitstring
    public int[][] GetRandomValidBitstring()
    {
        int[][] randomBitstring = new int[_Knapsacks.Length][];
        // Repeatly generate random bitstring until it's valid
        int startBit1Count = (_Knapsacks.Length > 1) ? 4 : 6;
        for (int bit1Count = startBit1Count; bit1Count > 0; bit1Count--)
        {
            randomBitstring = _ChromoDatabase.GenerateRandomBitstring(bit1Count);
            if (EvaluateChromosome(randomBitstring)[0] > 0)
            {
                break;
            }
        }
        return randomBitstring;
    }

    // Return one of the best bitstring
    public int[][] GetBestBitstring()
    {
        WeaponChromosome[] allWeapon = GetAllWeapon();
        List<WeaponChromosome> allWeaponList = new List<WeaponChromosome>(allWeapon);
        allWeaponList.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));
        int index = Random.Range(0, 10);    // Random one of the best top-10 index
        return allWeaponList[index].Bitstring;
    }

    // Return all weapon in database and its evaluated values
    public WeaponChromosome[] GetAllWeapon()
    {
        // If the factory isn't unlocked yet, return empty array
        if (_LockStatus != LockableStatus.Unlock)
        {
            return new WeaponChromosome[0];
        }
        _PopulateDatabaseIfNot();
        // Create empty array of type WeaponChromosome
        WeaponChromosome[] allWeapon = new WeaponChromosome[_PopulationCount];
        for (int i = 0; i < _PopulationCount; i++)
        {
            // Get and evaluate the bitstring
            int[][] bitstringAtIndex = _ChromoDatabase.GetBitstringAtIndex(i);
            int[] values = EvaluateChromosome(bitstringAtIndex);
            // Calculate the rank of weapon
            WeaponRankInfo rankInfo = _WeaponRankConfigDesc[_WeaponRankConfigDesc.Length - 1];
            foreach (WeaponRankInfo info in _WeaponRankConfigDesc)
            {
                if (values[0] >= info.MinFitness)
                {
                    rankInfo = info;
                    break;
                }
            }
            // Calculate the cooldown proportionate to the fitness
            float eff = (float)values[0] / (float)_MaxFitness;
            float maxCooldownDelta = _MaxCooldownSeconds - _MinCooldownSeconds;
            float cooldownSeconds = _MaxCooldownSeconds - (eff * maxCooldownDelta);
            // Create actual instance of WeaponChromosome
            allWeapon[i] = new WeaponChromosome(
                fromFactory: _FactoryIndex,
                mode1: _ProducedWeaponMode1,
                mode2: _ProducedWeaponMode2,
                isMode1Active: true,    // Set to mode 1 as default
                rank: rankInfo.Rank,
                image: rankInfo.Image,
                bigImage: rankInfo.BigImage,
                name: _WeaponPrefix + (i + 1).ToString(_WeaponIdFormat),
                bitstring: bitstringAtIndex,
                fitness: values[0],
                weight1: values[1],
                weight2: values[2],
                efficiency: eff,
                cooldown: cooldownSeconds,
                atk: rankInfo.BonusAtk,
                def: rankInfo.BonusDef,
                hp: rankInfo.BonusHp,
                spd: rankInfo.BonusSpd
                ) ;
        }
        return allWeapon;
    }

    // Since FactorySO hold both Knapsacks and Items, it should perform the evaluation task that use these information
    private int[] EvaluateChromosome(int[][] bitString)
    {
        int fitness = 0;
        int weight1 = 0;
        int weight2 = -1;
        // If the item preset have 2 weight, init weight2
        if (_Items[0].Weight2 != 0)
        {
            weight2 = 0;
        }
        // For each knapsack
        for (int kIndex = 0; kIndex < _Knapsacks.Length; kIndex++)
        {
            int[] thisKnapsackBitstring = bitString[kIndex];
            int thisKnapsackFitness = 0;
            int thisKnapsackWeight1 = 0;
            int thisKnapsackWeight2 = 0;
            // For eack item, sum all value of it
            for (int iIndex = 0; iIndex < _Items.Length; iIndex++)
            {
                // Skip to next item if it's not picked
                if (thisKnapsackBitstring[iIndex] == 0)
                {
                    continue;
                }
                thisKnapsackFitness += _Items[iIndex].Value;
                thisKnapsackWeight1 += _Items[iIndex].Weight1;
                thisKnapsackWeight2 += _Items[iIndex].Weight2;
            }
            // Set the fitness to 0 if the weight exceed the limit
            if (thisKnapsackWeight1 > _Knapsacks[kIndex].Weight1Limit)
            {
                thisKnapsackFitness = 0;
            }
            else if (thisKnapsackWeight2 > _Knapsacks[kIndex].Weight2Limit)
            {
                thisKnapsackFitness = 0;
            }
            // Sum up overall value
            fitness += thisKnapsackFitness;
            weight1 += thisKnapsackWeight1;
            if (weight2 != -1)
            {
                weight2 += thisKnapsackWeight2;
            }
        }
        int[] returnValue = { fitness, weight1, weight2 };
        return returnValue;
    }
    #endregion

    #region Breeding
    public void FillBreedGuage()
    {
        _BreedGuage += _GuagePerDay * _Condition / 4;

        while (_BreedGuage >= 100 && _BreedInfo.MyFactory.BreedPref.BreedGen > 0)
        {
            BreedInfo.Produce();
            _BreedGen++;
            _Generation++;
            _BreedGuage -= 100;
        }
        if (Random.Range(0f, 1f) < _BrokeChance)
        {
            BreakingBad();
        }
        if (BreedGen >= BreedInfo.MyFactory.BreedPref.BreedGen)
        {
            SetBreedRequest(new FactoryProduction.BreedInfo());
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
            _Condition--;
        }
        // if (Condition == 0 && Status != Status.BROKEN) SetStatus(Status.BROKEN);
    }

    public void Fixed()
    {
        if (_Condition < 4) _Condition++;
        // SetStatus(BreedInfo.Equals(default(BreedMenu.BreedInfo)) ? Status.IDLE : Status.BREEDING);
    }
    #endregion
}
