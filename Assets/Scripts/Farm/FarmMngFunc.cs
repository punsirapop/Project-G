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
    public void AddChromo(FarmSO f, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            MechChromo c = new MechChromo(null);
            f.AddChromo(c);
            // AddMech(c);
            OnEditChromo?.Invoke();
        }
    }

    // Delete a chromosome from the current space
    public void DelChromo(FarmSO f, MechChromo c)
    {
        f.DelChromo(c);
        // DelMech(c);
        OnEditChromo?.Invoke();
    }
}
