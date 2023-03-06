using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

/*
 * Control Chromosome list tab
 */
public class ChromoMenu : MonoBehaviour
{
    public struct OrderFormat
    {
        public string name;
        public MechChromoSO chromo;
        public float fitness;
    }
    // Prefab for button
    [SerializeField] GameObject preset;
    [SerializeField] ChromoDetail detailOverlay;
    // Place to store generated buttons
    [SerializeField] RectTransform parent;
    [SerializeField] BreedMenu breedMenu;
    [SerializeField] Toggle _ToggleSortByFitness;
    [SerializeField] Toggle _ToggleSortDescending;

    private void Awake()
    {
        FarmManager.OnEditChromo += OnValueChange;
    }

    private void OnDestroy()
    {
        FarmManager.OnEditChromo -= OnValueChange;
    }

    private void OnEnable()
    {
        OnValueChange();
    }

    /*
     * Update buttons when the chromosome list changes
     */
    public void OnValueChange()
    {
        // ------- misc -------
        _ToggleSortByFitness.GetComponentInChildren<TextMeshProUGUI>().text = _ToggleSortByFitness.isOn ? "Fitness" : "Name";
        _ToggleSortDescending.GetComponentInChildren<TextMeshProUGUI>().text = _ToggleSortDescending.isOn ? "Des" : "Asc";
        // ------- clear -------
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
        // ------- get fitness -------
        Dictionary<dynamic, float> fvDict = breedMenu.GetFitnessDict();
        List<OrderFormat> fv = new List<OrderFormat>();
        foreach (var item in fvDict)
        {
            MechChromoSO c = item.Key;
            OrderFormat of = new OrderFormat();
            of.name = c.ID.ToString();
            of.chromo = c;
            of.fitness = item.Value;
            fv.Add(of);
        }
        // ------- sort -------
        switch (_ToggleSortByFitness.isOn, _ToggleSortDescending.isOn)
        {
            case (true, true):
                fv = fv.OrderByDescending(x => x.fitness).ThenByDescending(x => x.name).ToList();
                break;
            case (true, false):
                fv = fv.OrderBy(x => x.fitness).ThenBy(x => x.name).ToList();
                break;
            case (false, true):
                fv = fv.OrderByDescending(x => x.name).ThenByDescending(x => x.fitness).ToList();
                break;
            case (false, false):
                fv = fv.OrderBy(x => x.name).ThenBy(x => x.fitness).ToList();
                break;
        }
        fv.Sort();
        // ------- display -------
        foreach (var item in fvDict)
        {
            GameObject me = Instantiate(preset, parent);
            
            me.GetComponent<Button>().onClick.AddListener(() => detailOverlay.SetDisplay(item.Key));
            me.GetComponentInChildren<TextMeshProUGUI>().text = "ID: " + item.Key.ID;
        }
    }
}
