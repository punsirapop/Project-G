using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/JigsawPiece")]
public class JigsawPieceSO : LockableObject
{
    [Header("Jigsaw Information")]
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private JigsawLevel _Level;
    public JigsawLevel Level => _Level;
    [SerializeField] private PuzzleType _HowToObtain;
    public PuzzleType HowToObtain => _HowToObtain;
    [SerializeField] private DialogueSO _TestingDialogue;   // Dialogue to use in case if it's PuzzleType.Dialogue for _HowToObtain
    public int SuccessCount { get; private set; }           // The number of success testing through _HowToObtain
    public int FailCount { get; private set; }              // The number of fail testing through _HowToObtain

    public override string GetRequirementPrefix()
    {
        return "Obtain";
    }

    public override string GetLockableObjectName()
    {
        return Name + " (" + _Level.ToString() + ")";
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
        SuccessCount = 0;
        FailCount = 0;
    }
    
    public void GoToObtain()
    {
        SceneMng.SetAndChangePuzzleScene(this);
    }

    // Add SuccessCount/FailCount and return amount of count added
    // return positive number means gaining, negative number means losing/fail to gain
    public int[] AddProgressCount(bool isSuccess, int amount)
    {
        int[] amountAndMoney = new int[] { 0, 0 };
        if ((LockStatus == LockableStatus.Lock) ||
            (amount <= 0))
        {
            return amountAndMoney;
        }
        // Count progress
        if (isSuccess)
        {
            SuccessCount += amount;
            amountAndMoney[0] = amount;
            bool isTransactionSuccess = PlayerManager.GainMoneyIfValid(_CalculateWorth(amount));
            amountAndMoney[1] = isTransactionSuccess ? _CalculateWorth(amount) : 0;
            base.ForceUnlock();
        }
        else
        {
            FailCount += amount;
            amountAndMoney[0] = -amount;
        }
        return amountAndMoney;
    }

    private int _CalculateWorth(int pieceAmount)
    {
        int multiplier = 0;
        if (_Level == JigsawLevel.Copper) multiplier = 10;
        else if (_Level == JigsawLevel.Silver) multiplier = 20;
        else if (_Level == JigsawLevel.Gold) multiplier = 30;
        return multiplier * pieceAmount;
    }
}

public enum JigsawLevel
{
    None,       // Place holder used as null for the Level
    Copper,     // Reflect learning level of Understand
    Silver,     // Reflect learning level of Demonstration
    Gold        // Reflect learning level of Solving
}