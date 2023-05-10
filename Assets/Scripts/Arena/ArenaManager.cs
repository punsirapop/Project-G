using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ArenaManager : MonoBehaviour
{
    [SerializeField] private Sprite[] _EffectSprites, _WeaponSprites, _WeaponRanks, _BulletSprites;
    public static Sprite[] EffectSprites, WeaponSprites, WeaponRanks, BulletSprites;

    [SerializeField] GameObject _EntrancePanel, _DefaultPanel, _NotifBox;
    [SerializeField] TextMeshProUGUI _Text;
    [SerializeField] TextMeshProUGUI _EntranceFeeText;

    [SerializeField] private int _EntranceFee;

    public static int EnemyLevel;

    // Assign sprites from serialized field on editor to the static variable
    private void Awake()
    {
        EffectSprites = _EffectSprites;
        WeaponSprites = _WeaponSprites;
        WeaponRanks = _WeaponRanks;
        BulletSprites = _BulletSprites;

        EnemyLevel = 0;

        _EntranceFeeText.text = PlayerManager.Money.ToString() + "/" + _EntranceFee.ToString();
    }

    public static Sprite GetWeaponImage(int weapon, int mode)
    {
        return WeaponSprites[weapon * 2 + mode];
    }

    public static Sprite GetSelfEffectImage(SelfEffects e)
    {
        return EffectSprites[(int)e];
    }

    public static Sprite GetWeaponRankSprite(WeaponChromosome w)
    {
        return WeaponRanks[w.FromFactory * 5 + (int)w.Rank];
    }

    public static Sprite GetBulletSprite(BulletType b)
    {
        return BulletSprites[Mathf.Max((int)b - 2, 0)];
    }

    public void BackToStart()
    {
        switch (BattleManager.WinningStatus)
        {
            case 0:
                PlayerManager.GainMoneyIfValid(500 * EnemyLevel);
                _DefaultPanel.SetActive(true);
                break;
            case 1:
                foreach (Transform item in transform)
                {
                    item.gameObject.SetActive(false);
                }
                _EntrancePanel.SetActive(true);
                break;
            default:
                _DefaultPanel.SetActive(true);
                break;
        }
        EnemyLevel = 0;
    }

    public void ClickStart()
    {
        string s = null;
        bool pass = true;
        if (PlayerManager.Money < _EntranceFee)
        {
            pass = false;
            if (s == null) s = "Not Enough Money";
            else s = string.Join("\n", s, "Not Enough Money");
        }
        if (PlayerManager.FarmDatabase.SelectMany(x => x.MechChromos).Count() < 3)
        {
            pass = false;
            if (s == null) s = "Not Enough Mechs";
            else s = string.Join("\n", s, "Not Enough Mechs");
        }
        if (!PlayerManager.FactoryDatabase.Any(x => x.LockStatus == LockableStatus.Unlock))
        {
            pass = false;
            if (s == null) s = "Not Enough Weapons";
            else s = string.Join("\n", s, "Not Enough Weapons");
        }
        if (pass)
        {
            foreach (Transform item in transform)
            {
                item.gameObject.SetActive(true);
            }
            PlayerManager.SpendMoneyIfEnought(_EntranceFee);
            _EntranceFeeText.text = PlayerManager.Money.ToString() + "/" + _EntranceFee.ToString();
            _EntrancePanel.SetActive(false);
            _DefaultPanel.SetActive(true);
        }
        else
        {
            _Text.text = s;
            _NotifBox.SetActive(true);
        }
    }
}
