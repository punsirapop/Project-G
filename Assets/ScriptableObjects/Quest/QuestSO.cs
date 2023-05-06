using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSO : ScriptableObject
{
    // Quest information
    [SerializeField] protected string _Name;
    public string Name => _Name;
    [SerializeField] protected string _BriefDescription;
    public string BriefDescription => _BriefDescription;
    [SerializeField] [TextArea(5, 10)] protected string _FullDescription;
    public string FullDescription => _FullDescription;
    [SerializeField] protected Status _QuestStatus;
    public Status QuestStatus => _QuestStatus;
    [SerializeField] protected TimeManager.Date _DueDate;
    public TimeManager.Date DueDate => _DueDate;
    // Dialogue
    [SerializeField] protected DialogueSO _IntroDialogue;
    public DialogueSO IntroDialogue => _IntroDialogue;
    [SerializeField] protected DialogueSO _OutroDialogue;
    public DialogueSO OutroDialogue => _OutroDialogue;

    public enum Status
    {
        Unacquired,     // Unacquired quest
        InProgress,     // Quest in player's hand that complete condition is not satisfied
        Completable,    // Quest in player's hand that complete condition is satisfied
        Expired,        // Quest in player's hand that is't completed before the due date
        Completed       // Quest that used to be in player's hand and already be completed
    }

    public void SetStatus(Status newStatus)
    {
        _QuestStatus = newStatus;
    }

    public virtual void ValdiateStatus()
    {
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
    }

    // Return true if current game date pass the due date of the quest
    public bool IsTimeOut()
    {
        // If there is no due date, it's never time out
        if (_DueDate.ToDay() == 0)
        {
            return false;
        }
        else if (_DueDate.CompareDate(PlayerManager.CurrentDate) < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Receive the quest
    public void ReceiveQuest()
    {
        // Change status and play dialogue
        _QuestStatus = Status.InProgress;
        PlayerManager.ValidateUnlocking();
        if (IntroDialogue != null)
        {
            PlayerManager.SetCurrentDialogue(IntroDialogue);
            SceneMng.StaticChangeScene("Cutscene");
        }
    }

    // Complete the quest
    public virtual void CompleteQuest()
    {
        // Change status and play dialogue
        _QuestStatus = Status.Completed;
        PlayerManager.ValidateUnlocking();
        if (OutroDialogue != null)
        {
            PlayerManager.SetCurrentDialogue(OutroDialogue);
            SceneMng.StaticChangeScene("Cutscene");
        }
    }

    // tmp method for entering with GM start
    public void ForceCompleteQuest()
    {
        _QuestStatus = Status.Completed;
    }
}
