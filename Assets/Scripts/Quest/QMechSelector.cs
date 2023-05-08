using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QMechSelector : HMechDisplay
{
    public int FromFarm;

    protected override void Start()
    {
        _Manager = transform.parent.parent.parent.parent;
        _IsTriggered = false;
        _IsHeld = -1;
        _HoldRange = new int[2];
    }
}
