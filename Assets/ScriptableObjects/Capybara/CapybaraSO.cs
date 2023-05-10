using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Capybara")]
public class CapybaraSO : LockableObject
{
    [Header("Capybara")]
    // Basic information
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private CapybaraRank _Rank;
    public CapybaraRank Rank => _Rank;
    [SerializeField] private Color _HeartColor;
    public Color HeartColor => _HeartColor;
    [SerializeField] private int _RequiredTapCount;
    public int RequiredTapCount => _RequiredTapCount;
    [SerializeField] private int _MoneyReward;
    public int MoneyReward => _MoneyReward;
    [SerializeField] private int _FoundCount;
    public int FoundCount => _FoundCount;
    // Movement
    [SerializeField] private Sprite[] _Sprites;
    public Sprite[] Sprites => _Sprites;
    [SerializeField] private float _StepPerSecond;
    public float StepPerSecond => _StepPerSecond;
    [SerializeField] private float _WalkedPixelPerSeconds;
    public float WalkedPixelPerSeconds => _WalkedPixelPerSeconds;
    
    public enum CapybaraRank
    {
        Normal,
        Rare,
        Legandary
    }

    public new void Reset()
    {
        base.Reset();
        _FoundCount = 0;
    }

    public void Found()
    {
        base.Unlock();
        _FoundCount++;
        PlayerManager.GainMoneyIfValid(_MoneyReward);
        PlayerManager.ValidateUnlocking();
    }

    // Save - Load
    public void Load(LockableStatus l, int f)
    {
        _LockStatus = l;
        _FoundCount = f;
    }

}
