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
        _MyButton.interactable = PlayerManager.FarmDatabase.Select(x => x.MechChromos).Count() > 3 &&
            PlayerManager.FactoryDatabase.Any(x => x.LockStatus == LockableStatus.Unlock);
    }
}
