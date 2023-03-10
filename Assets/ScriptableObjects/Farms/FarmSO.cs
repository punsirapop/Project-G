using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Farm")]
public class FarmSO : ScriptableObject
{
    // Informations
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private int _Generation;
    public int Generation => _Generation;
    [SerializeField] private Status _Status;
    public Status Status => _Status;

    // Sprites
    [SerializeField] private Sprite _BG;
    public Sprite BG => _BG;

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
        _MechChromos = new List<MechChromoSO>();
    }

    // Set farm from value
    public void SetMe(FarmSO f)
    {
        _Name = f.Name;
        _Generation = f.Generation;
        _Status = f.Status;
        _BG = f.BG;
        _MechChromos = new List<MechChromoSO>(f.MechChromos);
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

    public void AddGen(int g)
    {
        _Generation += g;
    }
}

public enum Status
{
    IDLE,
    BREEDING,
    BROKEN
}