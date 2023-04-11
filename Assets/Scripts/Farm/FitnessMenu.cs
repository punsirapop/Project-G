using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FitnessMenu : MonoBehaviour
{
    public enum Properties { Head, Body, Acc, Com }

    [SerializeField] Button AddButton;
    [SerializeField] FitnessSelector[] Selectors;
    // head, body-line, body-color, acc
    // [SerializeField] public Image[] myRenderer;

    protected virtual FarmSO myFarm => PlayerManager.CurrentFarmDatabase;
    // Color[] bodyColor = {Color.red, Color.green, Color.blue, Color.white, Color.black};
    // Color inactive = new Color(.5f, .5f, .5f, .5f);

    private void Awake()
    {
        // FarmSO.OnFarmChangeStatus += OnChangeStatus;
        // OnChangeStatus(myFarm, myFarm.Status);
        SetFitnessAdjustor();
    }

    private void OnEnable()
    {
        SetFitnessAdjustor();
    }

    private void OnDisable()
    {
        myFarm.SetFitnessPref(GetFitnessPref());
    }

    private void OnDestroy()
    {
        // FarmSO.OnFarmChangeStatus -= OnChangeStatus;
    }

    private void Update()
    {
        AddButton.interactable = (myFarm.Status != Status.BREEDING) ?
            Selectors.Where(x => x.gameObject.activeSelf).Count() < 5 : false;
    }

    /*
    private void OnChangeStatus(FarmSO f, Status s)
    {
        if (f == myFarm)
        {
            // Change behavior depending on status
            switch (s)
            {
                case Status.IDLE:
                    // Activate interactables

                    break;
                case Status.BREEDING:
                    // Deactivate interactables
                    SetFitnessAdjustor();
                    break;
                default:
                    break;
            }
        }
    }
    */

    private void SetFitnessAdjustor()
    {
        foreach (var item in Selectors)
        {
            item.Deactivate();
        }
        var Temp = (myFarm.Status == Status.BREEDING) ? myFarm.BreedInfo.CurrentPref : myFarm.FitnessPref;
        foreach (var item in Temp.Zip(Selectors, (a, b) => Tuple.Create(a, b)))
        {
            OpenSelector();
            item.Item2.SetValue(item.Item1);
        }
    }

    /*
     * Get current fitness preferences
     * 
     * Output
     *      list of preferred values (default = -1)
     */
    public List<Tuple<Properties, int>> GetFitnessPref()
    {
        List<Tuple<Properties, int>> a = new List<Tuple<Properties, int>>();
        foreach (var item in Selectors)
        {
            if(item.gameObject.activeSelf)
                a.Add(Tuple.Create(item.Type, item.Value));
        }
        return a;
    }
    
    /*
     * Output
     *      Dictionary of MechChromo and their respective fitness value
     */
    public Dictionary<dynamic, float> GetFitnessDict()
    {
        Dictionary<dynamic, float> dict = new Dictionary<dynamic, float>();
        foreach (MechChromoSO c in myFarm.MechChromos)
        {
            dict.Add(c, c.GetFitness(GetFitnessPref()));
        }
        return dict;
    }

    // Open top-most fitness adjustor if possible
    public void OpenSelector()
    {
        foreach (var item in Selectors)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                break;
            }
        }
    }
}
