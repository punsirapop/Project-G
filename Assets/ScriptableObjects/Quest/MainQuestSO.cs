using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/QuestMain")]
public class MainQuestSO : QuestSO
{
    // Complete condition
    [SerializeField] protected LockableObject _RequireObject;
    public LockableObject RequireObject => _RequireObject;
    // Reward
    [SerializeField] protected int _RewardMoney;
    public int RewardMoney => _RewardMoney;
    [SerializeField] protected MechPresetSO[] _RewardMechs;
    public MechPresetSO[] RewardMechs => _RewardMechs;


    public override void ValdiateStatus()
    {
        //base.ValdiateStatus();
        if ((_QuestStatus != Status.InProgress) &&
            (_QuestStatus != Status.Completable))
        {
            return;
        }
        // If time's out, it's expired
        if (IsTimeOut())
        {
            _QuestStatus = Status.Expired;
            return;
        }

        if (_RequireObject.LockStatus == LockableStatus.Unlock)
        {
            Debug.Log(Name + " is now completable");
            _QuestStatus = Status.Completable;
        }
        else
        {
            _QuestStatus = Status.InProgress;
        }
    }

    // Gain reward and complete the quest
    public override void CompleteQuest()
    {
        // Gain money reward
        bool isTransactionSuccess = PlayerManager.GainMoneyIfValid(RewardMoney);
        if (!isTransactionSuccess)
        {
            return;
        }
        // Gain mech reward, send all mech into habitat
        foreach (var mech in _RewardMechs)
        {
            MechChromo m = new MechChromo(null).SetChromosomeFromPreset(mech);
            PlayerManager.FarmDatabase[0].AddChromo(m);
        }
        // Make database wait for day before give new quest
        MainQuestDatabaseSO.WaitForDay();
        base.CompleteQuest();
    }
}
