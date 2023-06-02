using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AMechDisplay : MechCanvasDisplay
{
    [SerializeField] GameObject _MechIcon;
    [SerializeField] GameObject _MechStats;

    public WeaponChromosome MyWeaponSO;
    [SerializeField] GameObject _WeaponStats;
    [SerializeField] Image _WeaponIcon;

    [SerializeField] TextMeshProUGUI[] _Stats;

    public override void SetChromo(MechChromo c)
    {
        if (MyMechSO != c)
        {
            base.SetChromo(c);

            _MechIcon.SetActive(true);
            _MechStats.SetActive(true);

            _Stats[0].text = c.Atk.Sum().ToString();
            _Stats[1].text = c.Def.Sum().ToString();
            _Stats[2].text = c.Hp.Sum().ToString();
            _Stats[3].text = c.Spd.Sum().ToString();
        }
        else
        {
            MyMechSO = null;
            _MechIcon.SetActive(false);
            _MechStats.SetActive(false);
        }
    }

    public void SetWeapon(WeaponChromosome w)
    {
        if (MyWeaponSO != w)
        {
            MyWeaponSO = w;

            _WeaponIcon.gameObject.SetActive(true);
            _WeaponStats.SetActive(true);
            _WeaponIcon.sprite = w.Image;
            _Stats[4].text = w.IsMode1Active ? "1" : "2";
            _Stats[5].text = (w.Efficiency * 100).ToString();

            foreach (var item in _Stats)
            {
                item.color = Color.black;
            }

            switch (w.FromFactory)
            {
                case 0:
                    _Stats[1].color = Color.blue;
                    _Stats[2].color = Color.blue;
                    _Stats[1].text = (MyWeaponSO.BonusStat.Def + MyMechSO?.Def.Sum()).ToString();
                    _Stats[2].text = (MyWeaponSO.BonusStat.Hp + MyMechSO?.Hp.Sum()).ToString();
                    break;
                case 1:
                    _Stats[0].color = Color.blue;
                    _Stats[3].color = Color.blue;
                    _Stats[0].text = (MyWeaponSO.BonusStat.Atk + MyMechSO?.Atk.Sum()).ToString();
                    _Stats[3].text = (MyWeaponSO.BonusStat.Spd + MyMechSO?.Spd.Sum()).ToString();
                    break;
                case 2:
                    _Stats[1].color = Color.blue;
                    _Stats[3].color = Color.blue;
                    _Stats[1].text = (MyWeaponSO.BonusStat.Def + MyMechSO?.Def.Sum()).ToString();
                    _Stats[3].text = (MyWeaponSO.BonusStat.Spd + MyMechSO?.Spd.Sum()).ToString();
                    break;
                case 3:
                    _Stats[0].color = Color.blue;
                    _Stats[2].color = Color.blue;
                    _Stats[0].text = (MyWeaponSO.BonusStat.Atk + MyMechSO?.Atk.Sum()).ToString();
                    _Stats[2].text = (MyWeaponSO.BonusStat.Hp + MyMechSO?.Hp.Sum()).ToString();
                    break;
            }
        }
        else
        {
            MyWeaponSO = null;
            _WeaponIcon.gameObject.SetActive(false);
            _WeaponStats.SetActive(false);

            foreach (var item in _Stats)
            {
                item.color = Color.black;
            }
            _Stats[0].text = MyMechSO?.Atk.Sum().ToString();
            _Stats[1].text = MyMechSO?.Def.Sum().ToString();
            _Stats[2].text = MyMechSO?.Hp.Sum().ToString();
            _Stats[3].text = MyMechSO?.Spd.Sum().ToString();
        }
    }
}
