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
    // Place to store generated buttons
    [SerializeField] RectTransform parent;


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
     * 
     * Input
     *      Actually doesn't need one but the event requires one
     *      So actually just a place holder, don't have to care
     */
    void OnValueChange()
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in FarmManager.Instance.FarmsData[PlayerManager.CurrentFarm].MechChromos)
        {
            GameObject me = Instantiate(preset, parent);
            me.GetComponent<Button>().onClick.AddListener(() => FarmManager.Instance.OpenPanel(3));
            me.GetComponent<Button>().onClick.AddListener(() => ChromoDetail.Instance.SetDisplay(item));
            me.GetComponentInChildren<TextMeshProUGUI>().text = "ID: " + item.ID;
        }
    }
}
