using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AWeaponSelectDisplay : MonoBehaviour
{
    public WeaponChromosome MyWeaponSO;

    [SerializeField] Image _WeaponDisplay;
    [SerializeField] Image[] _WeaponModes;
    [SerializeField] GameObject[] _Indicators;
    [SerializeField] GameObject[] _StatHolders;
    [SerializeField] TextMeshProUGUI[] _Stats;
    [SerializeField] GameObject _ModeSelector;

    public void OpenModeSelector()
    {
        if (_Indicators.Any(x => x.activeSelf))
        {
            AllySelectionManager.Instance.SelectingWeapon(MyWeaponSO);
        }
        else
        {
            _ModeSelector.SetActive(!_ModeSelector.activeSelf);
        }
    }

    public void SelectWeapon(bool mode)
    {
        MyWeaponSO.SetIsMode1Active(mode);
        AllySelectionManager.Instance.SelectingWeapon(MyWeaponSO);
    }

    public void SetChromo(WeaponChromosome w)
    {
        MyWeaponSO = w;
        _WeaponDisplay.sprite = w.Image;

        _Stats[0].text = (w.Efficiency * 100).ToString();
        _Stats[1].text = w.Cooldown.ToString();
        _Stats[2].text = w.BonusStat.Atk.ToString();
        _Stats[3].text = w.BonusStat.Def.ToString();
        _Stats[4].text = w.BonusStat.Hp.ToString();
        _Stats[5].text = w.BonusStat.Spd.ToString();

        foreach (var item in _StatHolders)
        {
            item.SetActive(false);
        }
        _ModeSelector.SetActive(false);

        switch (w.FromFactory)
        {
            case 0:
                _StatHolders[1].SetActive(true);
                _StatHolders[2].SetActive(true);
                break;
            case 1:
                _StatHolders[0].SetActive(true);
                _StatHolders[3].SetActive(true);
                break;
            case 2:
                _StatHolders[1].SetActive(true);
                _StatHolders[3].SetActive(true);
                break;
            case 3:
                _StatHolders[0].SetActive(true);
                _StatHolders[2].SetActive(true);
                break;
        }

        _WeaponModes[0].sprite = ArenaManager.GetWeaponImage(w.FromFactory, 0);
        _WeaponModes[1].sprite = ArenaManager.GetWeaponImage(w.FromFactory, 1);
    }

    public void AdjustingTeam()
    {
        if (AllySelectionManager.Instance.Buttons[AllySelectionManager.Instance.CurrentSelection]
            .MyWeaponSO == MyWeaponSO)
            AdjustIndicators(2);
        else if (AllySelectionManager.Instance.AllyWeapon.Contains(MyWeaponSO))
            AdjustIndicators(1);
        else AdjustIndicators(0);
    }

    /*
     * 0 - none
     * 1 - select
     * 2 - alr in team
     */
    public void AdjustIndicators(int mode)
    {
        foreach (var item in _Indicators)
        {
            item.SetActive(false);
        }
        if (mode > 0) _Indicators[mode - 1].SetActive(true);
    }
}
