using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ArenaManager : MonoBehaviour
{
    [SerializeField] private Sprite[] _EffectSprites, _WeaponSprites, _WeaponRanks, _BulletSprites;
    public static Sprite[] EffectSprites, WeaponSprites, WeaponRanks, BulletSprites;

    // -1: Not visited, 0: Lose, 1: Win easy, 2: Win hard
    int[] _BattleStreak;
    int _BattleIndex;

    // Assign sprites from serialized field on editor to the static variable
    private void Awake()
    {
        EffectSprites = _EffectSprites;
        WeaponSprites = _WeaponSprites;
        WeaponRanks = _WeaponRanks;
        BulletSprites = _BulletSprites;
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
}
