using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectDisplay : MonoBehaviour
{
    public SelfEffects Type;
    // Team - Index
    public int[] Giver;
    public BattleMechManager Holder;
    public float Duration;

    Image _Sprite;

    private void Awake()
    {
        _Sprite = GetComponent<Image>();
    }

    private void Update()
    {
        if (Duration >= 0) Duration -= Time.deltaTime;
        else
        {
            Holder.Effects.Remove(this);
            Destroy(gameObject);
        }
    }

    public void Set(SelfEffects type, int[] giver, BattleMechManager holder, float duration)
    {
        Type = type;
        Giver = giver;
        Holder = holder;
        Duration = duration;

        _Sprite.sprite = ArenaManager.GetSelfEffectImage(type);
    }
}

public enum SelfEffects
{
    Taunt,      // increase chance of being attacked
    Stealth,    // decrease chance of being attacked
    Snipe,      // buff next attack - focus furthest opponent
    Pierce,     // buff next attack - ignore opponent def
    Sleep,      // prevent speed gauge from refilling
    Poison,     // decrease hp every attack
}

public enum BulletType
{
    Default,    // no effect
    Snipe,      // focus furthest opponent
    Pierce,     // attack ignore opponent def
    Sleep,      // give opponent sleep
    Poison,     // give opponent poison
    AOEHeal,    // aoe heal
    AOEDamage   // aoe dmg
}
