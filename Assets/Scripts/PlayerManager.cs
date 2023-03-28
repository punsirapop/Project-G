using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TimeManager;

/*
 * Store various miscellanous functions
 * Still don't know how to categorize them
 * - AddChromo
 * - DelChromo
 */
public class PlayerManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static PlayerManager Instance;
    public static int CurrentFactoryIndex = 0;
    public static int CurrentFarmIndex = 1;

    public static Date CurrentDate;

    public static FactorySO[] FactoryDatabase;
    public static FarmSO[] FarmDatabase;

    public static FactorySO CurrentFactoryDatabase => FactoryDatabase[CurrentFactoryIndex];
    public static FarmSO CurrentFarmDatabase => FarmDatabase[CurrentFarmIndex];

    [SerializeField] private FactorySO[] FactoryDatabaseHelper;
    [SerializeField] private FarmSO[] FarmDatabaseHelper;

    private void Awake()
    {
        TimeManager.OnChangeDate += OnChangeDate;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("FOUND DUPE");
            Destroy(Instance.gameObject);
        }
    }

    private void OnDestroy()
    {
        TimeManager.OnChangeDate -= OnChangeDate;
    }

    public static void OnChangeDate(Date d)
    {
        int day = d.CompareDate(CurrentDate);
        Debug.Log("ASK TO BREED FOR " + day + " FROM PM");
        for (int i = 0; i < day; i++)
        {
            foreach (var item in FarmDatabase)
            {
                if (item.Status == Status.BREEDING) item.FillBreedGuage();
            }
        }

        CurrentDate = d.DupeDate();
    }

    // Assign factories data from serialized field on editor to the static variable
    public void OnAfterDeserialize()
    {
        FactoryDatabase = FactoryDatabaseHelper;
        FarmDatabase = FarmDatabaseHelper;
    }

    // Reflect the value back into editor
    public void OnBeforeSerialize()
    {
        FactoryDatabaseHelper = FactoryDatabase;
        FarmDatabaseHelper = FarmDatabase;
    }

    // Change current factory
    public void SetCurrentFactoryIndex(int newFactoryIndex)
    {
        CurrentFactoryIndex = newFactoryIndex;
    }

    public void SetCurrentFarmIndex(int index)
    {
        CurrentFarmIndex = index;
    }
}
