using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableObject : ScriptableObject
{
    // Locking information
    [Header("Locking information")]
    [SerializeField] protected LockableStatus _LockStatus;
    public LockableStatus LockStatus => _LockStatus;
    [SerializeField] protected int _RequiredMoney;
    [SerializeField] protected LockableObject[] _RequiredObjects;

    public virtual string GetRequirementPrefix()
    {
        return "Unlock LockableObject";
    }

    public virtual string GetLockableObjectName()
    {
        return "Object Name";
    }

    // Change locking status from lock to unlockable when condition satisfy
    public void ValidateUnlockRequirement()
    {
        // If it's already Unlock, do nothing
        if (_LockStatus == LockableStatus.Unlock)
        {
            return;
        }
        bool isRequirementSatisfy = true;
        // Validate required facilities
        foreach (LockableObject requiredObject in _RequiredObjects)
        {
            if (requiredObject == null)
            {
                break;
            }
            if (requiredObject.LockStatus != LockableStatus.Unlock)
            {
                isRequirementSatisfy = false;
            }
        }
        // Validate money
        if (_RequiredMoney > PlayerManager.Money)
        {
            isRequirementSatisfy = false;
        }

        // If the requirements satisfy, set it as Unlockable
        if (isRequirementSatisfy)
        {
            _LockStatus = LockableStatus.Unlockable;
        }
        // Else, Lock the SO
        else
        {
            _LockStatus = LockableStatus.Lock;
        }
    }

    // Return all unlock requirements of this Factory in a form of array
    public List<UnlockRequirementData> GetUnlockRequirements()
    {
        List<UnlockRequirementData> unlockRequirements = new List<UnlockRequirementData>();
        // ObjectRequirement
        foreach (LockableObject requireObject in _RequiredObjects)
        {
            if (requireObject == null)
            {
                break;
            }
            unlockRequirements.Add(
                new UnlockRequirementData(
                    requireObject.LockStatus == LockableStatus.Unlock,
                    requireObject.GetRequirementPrefix(),
                    requireObject.GetLockableObjectName()
                    )
                );
        }
        // Money requirement
        unlockRequirements.Add(
            new UnlockRequirementData(
                _RequiredMoney <= PlayerManager.Money,
                "Money",
                PlayerManager.Money.ToString() + "/" + _RequiredMoney.ToString()
                )
            );
        return unlockRequirements;
    }

    // Consume resource and unlock Factory
    public void Unlock()
    {
        bool isTransactionSuccess = PlayerManager.SpendMoneyIfEnought(_RequiredMoney);
        if (isTransactionSuccess)
        {
            _LockStatus = LockableStatus.Unlock;
        }
    }

    // TEMP function for force unlocking
    public void ForceUnlock()
    {
        _LockStatus = LockableStatus.Unlock;
    }
}

// Use for lock FactorySO, FarmSO, and ContentChapterSO
public enum LockableStatus
{
    Lock,
    Unlockable,
    Unlock
}

// Use for displaying requirement for unlock things with LockableStatus
public struct UnlockRequirementData
{
    public bool IsSatisfy { get; private set; }
    public string Header { get; private set; }              // Data like: Money, Factory, Research Chapter
    public string Description { get; private set; }         // Data like: 999, Factory 1, Single-point Crossover

    public UnlockRequirementData(bool isSatisfy, string header, string description)
    {
        IsSatisfy = isSatisfy;
        Header = header;
        Description = description;
    }
}