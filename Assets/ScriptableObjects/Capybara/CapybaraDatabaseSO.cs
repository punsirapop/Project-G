using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/CapybaraDatabase")]
public class CapybaraDatabaseSO : ScriptableObject
{
    [SerializeField] private float _SpawnChancePerDay;
    [SerializeField] private CapybaraSO[] _Capybaras;
    public CapybaraSO[] Capybaras => _Capybaras;
    [SerializeField] private int _ChanceNormal;
    [SerializeField] private int _ChanceRare;
    [SerializeField] private int _ChanceLegend;
    private int _AllChance => _ChanceNormal + _ChanceRare + _ChanceLegend;
    public float CumulativeSpawnChance { get; private set; }
    public bool IsFirstSpawn { get; private set; }

    private void OnEnable()
    {
        SaveManager.OnReset += Reset;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= Reset;
    }

    public void Reset()
    {
        IsFirstSpawn = true;
        CumulativeSpawnChance = 0;
        foreach (CapybaraSO bara in _Capybaras)
        {
            bara.Reset();
        }
    }

    public void SetIsFirstSpawn(bool isFirst)
    {
        IsFirstSpawn = isFirst;
    }

    // Increase the cumulative chance when day passed
    public void AddChanceByDays(int dayPassed)
    {
        CumulativeSpawnChance += _SpawnChancePerDay * dayPassed;
        Debug.Log("Capybara spawn chance = " + CumulativeSpawnChance.ToString());
    }

    // Reduce the cumulative chance when spawn
    public void OnSpawnCapybara()
    {
        CumulativeSpawnChance -= 1;
        if (CumulativeSpawnChance < 0)
        {
            CumulativeSpawnChance = 0;
        }
    }

    // Return random capybara with the given rank
    private CapybaraSO GetRandomCapybaraByRank(CapybaraSO.CapybaraRank rank)
    {
        List<CapybaraSO> capybaraByRank = new List<CapybaraSO>();
        foreach (CapybaraSO bara in _Capybaras)
        {
            if (bara.Rank == rank)
            {
                capybaraByRank.Add(bara);
            }
        }
        int index = Random.Range(0, capybaraByRank.Count);
        return capybaraByRank[index];
    }

    public CapybaraSO GetRandomCapybara()
    {
        // If request for the first time, return the typical capybara
        if (IsFirstSpawn)
        {
            return _Capybaras[0];
        }
        // Proportionate random for each rank
        int randomValue = Random.Range(0, _AllChance);
        CapybaraSO randomCapybara;
        if (randomValue < _ChanceLegend)
        {
            randomCapybara = GetRandomCapybaraByRank(CapybaraSO.CapybaraRank.Legandary);
        }
        else if (randomValue - _ChanceLegend < _ChanceRare)
        {
            randomCapybara = GetRandomCapybaraByRank(CapybaraSO.CapybaraRank.Rare);
        }
        else
        {
            randomCapybara = GetRandomCapybaraByRank(CapybaraSO.CapybaraRank.Normal);
        }
        return randomCapybara;
    }

    public List<CapybaraSO> GetAllCapybara(bool sortByRank)
    {
        List<CapybaraSO> capybaraList = new List<CapybaraSO>(_Capybaras);
        if (sortByRank)
        {
            capybaraList.Sort((a, b) => b.Rank.CompareTo(a.Rank));
        }
        return capybaraList;
    }

    // Save - Load
    public void Load(float c, bool b, LockableStatus[] l, int[] f)
    {
        CumulativeSpawnChance = c;
        IsFirstSpawn = b;
        for (int i = 0; i < Capybaras.Length; i++)
        {
            Capybaras[i].Load(l[i], f[i]);
        }
    }
}
