using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/ContentChapter")]
public class ContentChapterSO : ScriptableObject
{
    // Header and contents
    [SerializeField] private string _Header;
    public string Header => _Header;
    [SerializeField] private ContentPageSO[] _Contents;
    public ContentPageSO[] Contents => _Contents;

    // Locking information
    [SerializeField] private LockableStatus _LockStatus;
    public LockableStatus LockStatus => _LockStatus;
    [SerializeField] private int _RequiredMoney;
    [SerializeField] private ContentChapterSO[] _RequiredChapters;
    //[SerializeField] private Quest _RequiredQuest;

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
        // TEMP Unlock first chapter
        if (_Header == "On the Origin of Species")
        {
            _LockStatus = LockableStatus.Unlockable;
        }
        else
        {
            _LockStatus = LockableStatus.Lock;
        }
    }

    // Return array of unlock requirements
    public UnlockRequirementData[] GetUnlockRequirements()
    {
        UnlockRequirementData[] unlockRequirements = new UnlockRequirementData[_RequiredChapters.Length + 1];
        int i;
        // Chapter requirement
        for (i = 0; i < _RequiredChapters.Length; i++)
        {
            unlockRequirements[i] = new UnlockRequirementData(
                _RequiredChapters[i].LockStatus == LockableStatus.Unlock,
                "Unlock research",
                _RequiredChapters[i].Header
                );
        }
        // Money requirement
        unlockRequirements[i] = new UnlockRequirementData(
            _RequiredMoney <= PlayerManager.Money,
            "Money",
            PlayerManager.Money.ToString() + "/" + _RequiredMoney.ToString()
            );
        return unlockRequirements;
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
        // Validate for Money
        if (_RequiredMoney > PlayerManager.Money)
        {
            isRequirementSatisfy = false;
        }
        // Validate for Chapter
        foreach (ContentChapterSO chapter in _RequiredChapters)
        {
            if (chapter.LockStatus != LockableStatus.Unlock)
            {
                isRequirementSatisfy = false;
            }
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

    // Consume resource and unlock Chapter
    public void UnlockChapter()
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
