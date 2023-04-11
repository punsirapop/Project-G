using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Store functions relevant to farm management
 */
public class FarmMngFunc : MonoBehaviour
{
    public static event Action OnEditChromo;

    // Add new random chromosome to the current space
    public void AddChromo(FarmSO f)
    {
        for (int i = 0; i < 10; i++)
        {
            MechChromoSO c = (MechChromoSO)ScriptableObject.CreateInstance("MechChromoSO");
            f.AddChromo(c);
            // AddMech(c);
            OnEditChromo?.Invoke();

        }
    }

    // Delete a chromosome from the current space
    public void DelChromo(FarmSO f, MechChromoSO c)
    {
        f.DelChromo(c);
        // DelMech(c);
        OnEditChromo?.Invoke();
    }
}
