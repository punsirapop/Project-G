using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _Damage;

    // 1: Weak, 2: Resist, 3: Poison, 4: Heal
    public void SetDamage(float dmg, DamageMode dm)
    {
        _Damage.text = Mathf.Abs((float)System.Math.Round(dmg, 2)).ToString();
        _Damage.outlineWidth = .2f;
        _Damage.outlineColor = Color.black;
        switch (dm)
        {
            case DamageMode.Normal:
                _Damage.color = Color.grey;
                break;
            case DamageMode.Weak:
                _Damage.color = Color.red;
                break;
            case DamageMode.Resist:
                _Damage.color = Color.blue;
                break;
            case DamageMode.Poison:
                _Damage.color = Color.magenta;
                break;
            case DamageMode.Heal:
                _Damage.color = Color.green;
                break;
            default:
                break;
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}

public enum DamageMode
{
    Normal,
    Weak,
    Resist,
    Poison,
    Heal
}
