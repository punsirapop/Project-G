using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChromoMenu : MonoBehaviour
{
    [SerializeField] GameObject preset;
    [SerializeField] Transform parent;


    private void Awake()
    {
        PlayerManager.OnAddChromosome += OnValueChange;
        PlayerManager.OnRemoveChromosome += OnValueChange;
    }

    private void OnDestroy()
    {
        PlayerManager.OnAddChromosome -= OnValueChange;
        PlayerManager.OnRemoveChromosome -= OnValueChange;
    }

    private void OnEnable()
    {
        OnValueChange(null);
    }

    void OnValueChange(ChromosomeSC c)
    {
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in PlayerManager.Chromosomes[PlayerManager.CurrentFarm])
        {
            GameObject me = Instantiate(preset, parent);
            me.GetComponent<Button>().onClick.AddListener(() => FarmManager.Instance.OpenPanel(3));
            me.GetComponent<Button>().onClick.AddListener(() => ChromoDetail.Instance.SetDisplay(item));
            me.GetComponentInChildren<TextMeshProUGUI>().text = "ID: " + item.ID;
        }
    }
}
