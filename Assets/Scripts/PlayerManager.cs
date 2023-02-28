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
    private static int _CurrentFactory;
    public static int CurrentFactory => _CurrentFactory;
    private void Awake()
    {
        // if(Instance == null) Instance = this;
    }

    public void SetCurrentFactory(int index)
    {
        _CurrentFactory = index;
    }
    public void SetCurrentFarm(int index)
    {
        _CurrentFarm = index;
    }
}
