using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class for lock the button to new system using the main quest
public class LockableByQuest : MonoBehaviour
{
    [SerializeField] private MainQuestSO _RequiredMainQuest;
    [SerializeField] private QuestSO.Status _RequiredStatus;

    private void Awake()
    {
        // Unlock the button only required quest reach at least certain status
        if (_RequiredMainQuest.QuestStatus >= _RequiredStatus)
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
