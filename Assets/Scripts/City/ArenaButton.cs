using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArenaButton : MonoBehaviour
{
    Button _MyButton;
    
    void Start()
    {
        _MyButton = GetComponent<Button>();
        _MyButton.interactable = PlayerManager.FarmDatabase.Any(x => x.MechChromos.Count > 0) &&
            PlayerManager.FactoryDatabase.Any(x => x.LockStatus == LockableStatus.Unlock);
    }
}
