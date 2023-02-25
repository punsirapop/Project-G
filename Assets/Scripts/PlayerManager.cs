using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Store various miscellanous functions
 * Still don't know how to categorize them
 * - AddChromo
 * - DelChromo
 */
public class PlayerManager : MonoBehaviour
{
    // public static PlayerManager Instance;

    private static int _CurrentFarm;
    public static int CurrentFarm => _CurrentFarm;

    private void Awake()
    {
        // if(Instance == null) Instance = this;
    }

    public void ChangePlace(int i)
    {
        _CurrentFarm = i;
    }
}
