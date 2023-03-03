using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * Control Chromosome list tab
 */
public class ChromoMenu : MonoBehaviour
{
    // Prefab for button
    [SerializeField] GameObject preset;
    [SerializeField] ChromoDetail detailOverlay;
    // Place to store generated buttons
    [SerializeField] RectTransform parent;
    [SerializeField] BreedMenu breedMenu;

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
    void OnValueChange()
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
        // ------- get fitness -------
        Dictionary<dynamic, float> fv = breedMenu.GetFitnessDict();

        foreach (var item in FarmManager.Instance.FarmsData[PlayerManager.CurrentFarm].MechChromos)
        {
            GameObject me = Instantiate(preset, parent);
            me.GetComponent<Button>().onClick.AddListener(() => FarmManager.Instance.OpenPanel(3));
            me.GetComponent<Button>().onClick.AddListener(() => ChromoDetail.Instance.SetDisplay(item));
            me.GetComponentInChildren<TextMeshProUGUI>().text = "ID: " + item.ID;
        }
    }
}
