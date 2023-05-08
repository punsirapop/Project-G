using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChromoMenu : MonoBehaviour
{
    [Header("Panel Information")]
    [SerializeField] private GameObject _WeaponButtonPrefab;
    [SerializeField] private RectTransform _SpawnButtonArea;
    [SerializeField] private Toggle _ToggleSortByFitness;
    [SerializeField] private Toggle _ToggleSortDescending;
    // Overlay for displaying 1 weapon information
    [Space(10)]
    [Header("Overlay Information")]
    [SerializeField] private GameObject _BigWeaponOverlay;
    [SerializeField] private TextMeshProUGUI _OverlayName;
    [SerializeField] private Image _OverlayImage;
    [Header("Chromosome")]
    [SerializeField] private TextMeshProUGUI _Bitstring;
    [SerializeField] private TextMeshProUGUI _Fitness;
    [SerializeField] private TextMeshProUGUI _Weight1;
    [SerializeField] private GameObject _Weight2;
    [Header("Bonus Stat")]
    [SerializeField] private TextMeshProUGUI _Rank;
    [SerializeField] private GameObject _Atk;
    [SerializeField] private GameObject _Def;
    [SerializeField] private GameObject _Hp;
    [SerializeField] private GameObject _Spd;
    [Header("Skill")]
    [SerializeField] private Sprite[] _WeaponIcons;
    [SerializeField] private Image _Mode1Icon;
    [SerializeField] private Image _Mode2Icon;
    [SerializeField] private TextMeshProUGUI _Mode1Name;
    [SerializeField] private TextMeshProUGUI _Mode2Name;
    [SerializeField] private TextMeshProUGUI _Mode1Desc;
    [SerializeField] private TextMeshProUGUI _Mode2Desc;
    [SerializeField] private TextMeshProUGUI _Cooldown;
    private WeaponChromosome _OverlayChromosome;


    public void Start()
    {
        _BigWeaponOverlay.SetActive(false);
        ResetPanel();
    }

    // Re-generate all buttons in panels
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
        _Bitstring.text = bitstring.Trim('\\', 'n');
        _Fitness.text = chromosome.Fitness.ToString();
        _Weight1.text = chromosome.Weight1.ToString();
        // Show weight2 only if it has a weight2
        if (chromosome.Weight2 == -1)
        {
            _Weight2.SetActive(false);
        }
        else
        {
            _Weight2.SetActive(true);
            _Weight2.GetComponentsInChildren<TextMeshProUGUI>()[1].text = chromosome.Weight2.ToString();
        }
        // Assign weapon rank and bonus stat
        _Rank.text = chromosome.Rank.ToString();
        _Atk.SetActive(chromosome.BonusStat.Atk > 0);
        _Atk.GetComponentsInChildren<TextMeshProUGUI>()[1].text = chromosome.BonusStat.Atk.ToString();
        _Def.SetActive(chromosome.BonusStat.Def > 0);
        _Def.GetComponentsInChildren<TextMeshProUGUI>()[1].text = chromosome.BonusStat.Def.ToString();
        _Hp.SetActive(chromosome.BonusStat.Hp > 0);
        _Hp.GetComponentsInChildren<TextMeshProUGUI>()[1].text = chromosome.BonusStat.Hp.ToString();
        _Spd.SetActive(chromosome.BonusStat.Spd > 0);
        _Spd.GetComponentsInChildren<TextMeshProUGUI>()[1].text = chromosome.BonusStat.Spd.ToString();
        // Assign weapon skill (mode) and cooldown
        _Mode1Icon.sprite = _WeaponIcons[(int)chromosome.Mode1];
        _Mode2Icon.sprite = _WeaponIcons[(int)chromosome.Mode2];
        _Mode1Name.text = chromosome.Mode1.ToString();
        _Mode2Name.text = chromosome.Mode2.ToString();
        _Mode1Desc.text = DescribeWeapon(chromosome, chromosome.Mode1);
        _Mode2Desc.text = DescribeWeapon(chromosome, chromosome.Mode2);
        _Cooldown.text = chromosome.Cooldown.ToString("F1");
    }

    public static string DescribeWeapon(WeaponChromosome w, WeaponMode m)
    {
        switch (m)
        {
            case WeaponMode.Taunt:
                return $"Increase chances of being targeted by {20f + w.Efficiency * 30f}% for 3 seconds";
            case WeaponMode.Stealth:
                return $"Decrease chances of being targeted by {20f + w.Efficiency * 30f}% for 3 seconds";
            case WeaponMode.Snipe:
                return $"Increase chances of targeting the furthest opponent by {20f + w.Efficiency * 30f}% for 3 seconds";
            case WeaponMode.Pierce:
                return $"Attack ignores {20f + w.Efficiency * 30f}% of an opponent's defense for 3 seconds";
            case WeaponMode.Sleep:
                return $"Stop an opponent from charging attacks and using skills for {2f + w.Efficiency * 3f} seconds";
            case WeaponMode.Poison:
                return $"Give 'Deal {20f + w.Efficiency * 30f}% of the effect giver's ATK when attacking' to an opponent for 3 seconds";
            case WeaponMode.AOEHeal:
                return $"Heal everyone in the team for {20f + w.Efficiency * 30f}% of the user's ATK";
            case WeaponMode.AOEDamage:
                return $"Deal {Mathf.Round(((2f / 3f) + w.Efficiency / 3f) * 100)}% of the user's ATK to everyone in the opposing team";
            default:
                return "Unknown weapon mode";
        }
    }
}
