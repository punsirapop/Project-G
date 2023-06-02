using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChromoMenu : MonoBehaviour
{
    // Prefab for button
    [SerializeField] private GameObject _WeaponButtonPrefab;
    // Place to store generated buttons
    [SerializeField] private RectTransform _SpawnButtonArea;
    [SerializeField] private Toggle _ToggleSortByFitness;
    [SerializeField] private Toggle _ToggleSortDescending;
    [SerializeField] private GameObject _BigWeaponOverlay;
    [SerializeField] private TextMeshProUGUI _OverlayName;
    [SerializeField] private Image _OverlayImage;
    [SerializeField] private TextMeshProUGUI _OverlayBitstring;
    [SerializeField] private TextMeshProUGUI _OverlayFitness;
    [SerializeField] private TextMeshProUGUI _OverlayWeight1;
    [SerializeField] private GameObject _OverlayWeight2;
    private WeaponChromosome _OverlayChromosome;


    public void Start()
    {
        _BigWeaponOverlay.SetActive(false);
        ResetPanel();
    }

    // Re-generate all buttons in panels
    // sortBy:  0 = by name ascending
    //          1 = by name descending
    //          2 = by fitness descending
    //          3 = by fitness ascending
    public void ResetPanel()
    {
        foreach (Transform item in _SpawnButtonArea)
        {
            Destroy(item.gameObject);
        }
        WeaponChromosome[] chromosomes = FactoryManager.Instance.GetAllWeapon();
        List<WeaponChromosome> chromosomesList = new();
        chromosomesList.AddRange(chromosomes);
        _ToggleSortByFitness.GetComponentInChildren<TextMeshProUGUI>().text = _ToggleSortByFitness.isOn ? "Fitness" : "Name";
        _ToggleSortDescending.GetComponentInChildren<TextMeshProUGUI>().text = _ToggleSortDescending.isOn ? "Des" : "Asc";
        if (_ToggleSortByFitness.isOn)
        {
            if (_ToggleSortDescending.isOn)
            {
                chromosomesList.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));
            }
            else
            {
                chromosomesList.Sort((a, b) => a.Fitness.CompareTo(b.Fitness));
            }
        }
        else
        {
            if (_ToggleSortDescending.isOn)
            {
                chromosomesList.Sort((a, b) => b.Name.CompareTo(a.Name));
            }
            else
            {
                chromosomesList.Sort((a, b) => a.Name.CompareTo(b.Name));
            }
        }
        foreach (WeaponChromosome chromosome in chromosomesList)
        {
            GameObject me = Instantiate(_WeaponButtonPrefab, _SpawnButtonArea);
            me.GetComponent<WeaponButton>().SetChromosome(chromosome);
            me.GetComponent<Button>().onClick.AddListener(() => _ToggleDetail(chromosome));
        }
    }

    private void _ToggleDetail(WeaponChromosome chromosome)
    {
        // Close overlay only when click on the same button for second time while the overlay is on
        // Checking null in case if clicking for the first time
        if (_OverlayChromosome == null)
        {
            _OverlayChromosome = chromosome;
        }
        else if ((_OverlayChromosome == chromosome) && _BigWeaponOverlay.activeSelf)
        {
            _BigWeaponOverlay.SetActive(false);
            return;
        }
        _OverlayChromosome = chromosome;
        _BigWeaponOverlay.SetActive(true);
        // Formatting bitstring
        string bitstring = "";
        foreach (int[] section in chromosome.Bitstring)
        {
            if (section == null)
            {
                break;
            }
            foreach (int bit in section)
            {
                bitstring += bit.ToString();
            }
            bitstring += "\n";
        }
        // Assign this chromosome properties on UI
        _OverlayName.text = chromosome.Name;
        _OverlayImage.sprite = chromosome.BigImage;
        _OverlayBitstring.text = bitstring.Trim('\\', 'n');
        _OverlayFitness.text = chromosome.Fitness.ToString();
        _OverlayWeight1.text = chromosome.Weight1.ToString();
        // Show weight2 only if it has a weight2
        if (chromosome.Weight2 == -1)
        {
            _OverlayWeight2.SetActive(false);
        }
        else
        {
            _OverlayWeight2.SetActive(true);
            _OverlayWeight2.GetComponentsInChildren<TextMeshProUGUI>()[1].text = chromosome.Weight2.ToString();
        }
    }
}
