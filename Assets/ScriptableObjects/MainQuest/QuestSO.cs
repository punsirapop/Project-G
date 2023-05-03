using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Quest")]
public class QuestSO : ScriptableObject
{
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private string _BriefDescription;
    public string BriefDescription => _BriefDescription;
    [SerializeField] [TextArea(1, 5)] private string _Description;
    public string Description => _Description;
    [SerializeField] private Status _QuestStatus;
    public Status QuestStatus => _QuestStatus;
    [SerializeField] private LockableObject _RequireObject;
    [SerializeField] private TimeManager.Date _DueDate;
    [SerializeField] private int _RewardMoney;
    public int RewardMoney => _RewardMoney;
    [SerializeField] private MechChromoSO[] _RewardMechs;
    public MechChromoSO[] RewardMechs => _RewardMechs;

    public enum Status
    {
        Unacquired,     // Unacquired quest
        InProgress,     // Quest in player's hand that complete condition is not satisfied
        Completable,    // Quest in player's hand that complete condition is satisfied
        Expired,        // Quest in player's hand that is't completed before the due date
        Completed       // Quest that used to be in player's hand and already be completed
    }
}
