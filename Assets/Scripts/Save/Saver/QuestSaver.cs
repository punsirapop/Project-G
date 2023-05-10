using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestSaver
{
    // Main
    public int CurrentQuestIndex;
    public bool IsDayPassed;
    public QuestSO.Status[] QuestStatus;

    // Side DB
    public int NextSideQuestID;
    public int DayLeftBeforeNewQuest;
    public SideQuestSaver[] SideQuestSavers;
}

[Serializable]
public class SideQuestSaver
{
    public QuestSO.Status QuestStatus;
    public int ID;
    public MechSaver WantedMech;
    public string Name;
    public string BriefDesc;
    public string FullDesc;
    public int DueDate;
    public int MinReward;
    public int MaxReward;
}
