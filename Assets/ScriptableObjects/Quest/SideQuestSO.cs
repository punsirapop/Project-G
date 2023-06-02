using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/QuestSide")]
public class SideQuestSO : QuestSO
{
    // ID for saving asset purpose
    public int ID { get; private set; }
    // Complete condition
    public MechChromo WantedMech { get; private set; }
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
        WantedMech = new MechChromo(null);
        PlayerManager.MechIDCounter--;
        MinRewardMoney = minRewardMoney;
        MaxRewardMoney = maxRewardMoney;
    }


    // Calculate expected reward from mech similarity
    public int CalculateExpectedRewardMoney()
    {
        // If there is no selected mech, expect no reward
        if (SideQuestSubmissionManager.Instance.SelectedMech == null)
        {
            return 0;
        }
        // If there is some selected mech, calculate reward
        // float similarityRate = SideQuestSubmissionManager.Instance.SimilarityRate;
        float similarityRate = WantedMech.CompareMechQuest(SideQuestSubmissionManager.Instance.SelectedMech.Item1);
        // Hard-code the bound here...
        float lowerBound = 50f;    // Rate at least for starting getting bonus money
        float upperBound = 90f;    // Rate at most for getting max money
        int rewardMoney;
        if (similarityRate < lowerBound)
        {
            rewardMoney = 0;
        }
        else if (similarityRate >= upperBound)
        {
            rewardMoney = MaxRewardMoney;
        }
        else
        {
            float bonusRate = (similarityRate - lowerBound) / (upperBound - lowerBound);
            int bonusMoney = Mathf.RoundToInt(((float)(MaxRewardMoney - MinRewardMoney)) * bonusRate);
            rewardMoney = MinRewardMoney + bonusMoney;
        }
        return rewardMoney;
    }

    // Gain reward and complete the quest 
    public override void CompleteQuest()
    {
        // If it's expired quest, just abandon it and mark as completed
        if (_QuestStatus == Status.Expired)
        {
            base.CompleteQuest();
            // May give some penalty like losing money around here...
            PlayerManager.ForceSpendMoney(MinRewardMoney / 2);
            return;
        }

        // Gain money reward (move compare mech into SideQuesSubmissionManager or so)
        int rewardMoney = CalculateExpectedRewardMoney();
        bool isTransactionSuccess = PlayerManager.GainMoneyIfValid(rewardMoney);
        if (!isTransactionSuccess)
        {
            return;
        }

        // Remove submitted mech from the habitat (this should also be moved into SideQuesSubmissionManager or so I guess)
        // WIP

        base.CompleteQuest();
    }

    // Save - Load
    public SideQuestSaver Save()
    {
        SideQuestSaver s = new SideQuestSaver();

        s.QuestStatus = QuestStatus;
        s.ID = ID;
        s.WantedMech = WantedMech.Save();
        s.Name = Name;
        s.BriefDesc = BriefDescription;
        s.FullDesc = FullDescription;
        s.DueDate = DueDate.ToDay();
        s.MinReward = MinRewardMoney;
        s.MaxReward = MaxRewardMoney;

        return s;
    }

    public void Load(SideQuestSaver s)
    {
        _QuestStatus = s.QuestStatus;
        ID = s.ID;
        WantedMech = new MechChromo(s.WantedMech);
        PlayerManager.MechIDCounter--;
        _Name = s.Name;
        _BriefDescription = s.BriefDesc;
        _FullDescription = s.FullDesc;
        _DueDate = new TimeManager.Date().AddDay(s.DueDate);
        MinRewardMoney = s.MinReward;
        MaxRewardMoney = s.MaxReward;
    }
}