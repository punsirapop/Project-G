using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/QuestSide")]
public class SideQuestSO : QuestSO
{
    // ID for saving asset purpose
    public int ID { get; private set; }
    // Complete condition
    public MechChromoSO WantedMech { get; private set; }
    // Reward
    public int MinRewardMoney { get; private set; }
    public int MaxRewardMoney { get; private set; }

    public string GetDaysLeftText()
    {
        int dayLeft = _DueDate.CompareDate(PlayerManager.CurrentDate);
        string unitSuffix = (dayLeft > 1) ? " days left" : " day left";
        return dayLeft.ToString() + unitSuffix;
    }

    public void SetSideQuest(int id,
        string name, string briefDescription, string fullDescription, TimeManager.Date dueDate,
        int minRewardMoney, int maxRewardMoney)
    {
        ID = id;
        _Name = name;
        _BriefDescription = briefDescription;
        _FullDescription = fullDescription;
        _QuestStatus = Status.Unacquired;
        _DueDate = dueDate;
        // WantedMech
        MinRewardMoney = minRewardMoney;
        MaxRewardMoney = maxRewardMoney;
    }

    // Gain reward and complete the quest
    public override void CompleteQuest()
    {
        // If it's expired quest, just abandon it and mark as completed
        if (_QuestStatus == Status.Expired)
        {
            base.CompleteQuest();
            return;
        }

        // Compare mech and calculate reward
        // WIP

        // Gain money reward
        bool isTransactionSuccess = PlayerManager.GainMoneyIfValid(MaxRewardMoney);
        if (!isTransactionSuccess)
        {
            return;
        }

        // Remove submitted mech from the habitat
        // WIP

        base.CompleteQuest();
    }
}