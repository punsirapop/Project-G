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
public class PlayerManager : MonoBehaviour, ISerializationCallbackReceiver
{
    // public static PlayerManager Instance;
    public static int CurrentFactoryIndex = 0;

    public static FactorySO[] FactoryDatabase;
    public static FactorySO CurrentFactoryDatabase => FactoryDatabase[CurrentFactoryIndex];
    [SerializeField] private FactorySO[] FactoryDatabaseHelper;

    private static int _CurrentFarm;
    public static int CurrentFarm => _CurrentFarm;
    private static int _CurrentFactory;
    public static int CurrentFactory => _CurrentFactory;
    private void Awake()
    {
        // if(Instance == null) Instance = this;
    }

    // Assign factories data from serialized field on editor to the static variable
    public void OnAfterDeserialize()
    {
        FactoryDatabase = FactoryDatabaseHelper;
    }

    // Reflect the value back into editor
    public void OnBeforeSerialize()
    {
        FactoryDatabaseHelper = FactoryDatabase;
    }

    // Change current factory
    public void SetCurrentFactoryIndex(int newFactoryIndex)
    {
        CurrentFactoryIndex = newFactoryIndex;
    }

    public void SetCurrentFarm(int index)
    {
        _CurrentFarm = index;
    }
}
