using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Quest")]
public class QuestSO : ScriptableObject
{
    // Quest information
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private string _BriefDescription;
    public string BriefDescription => _BriefDescription;
    [SerializeField] [TextArea(5, 10)] private string _FullDescription;
    public string FullDescription => _FullDescription;
    [SerializeField] private Status _QuestStatus;
    public Status QuestStatus => _QuestStatus;
    [SerializeField] private LockableObject _RequireObject;
    public LockableObject RequireObject => _RequireObject;
    [SerializeField] private TimeManager.Date _DueDate;
    public TimeManager.Date DueDate => _DueDate;
    // Reward
    [SerializeField] private int _RewardMoney;
    public int RewardMoney => _RewardMoney;
    [SerializeField] private MechChromoSO[] _RewardMechs;
    public MechChromoSO[] RewardMechs => _RewardMechs;
    // Dialogue
    [SerializeField] private DialogueSO _IntroDialogue;
    public DialogueSO IntroDialogue => _IntroDialogue;
    [SerializeField] private DialogueSO _OutroDialogue;
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

    public void ValdiateStatus()
    {
        Debug.Log("Validating quest");
        if ((_QuestStatus != Status.InProgress) &&
            (_QuestStatus != Status.Completable))
        {
            Debug.Log("Immediate return");
            return;
        }
        // If required object is unlocked in time, it's completable
        // If time's out, it's expired
        if (IsTimeOut())
        {
            _QuestStatus = Status.Expired;
        }
        else
        {
            if (_RequireObject == null)
            {
                _QuestStatus = Status.Completable;
                return;
            }
            if (_RequireObject.LockStatus == LockableStatus.Unlock)
            {
                Debug.Log("Quest is Completable");
                _QuestStatus = Status.Completable;
            }
            else
            {
                Debug.Log("Quest is InProgress");
                _QuestStatus = Status.InProgress;
            }
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
}
