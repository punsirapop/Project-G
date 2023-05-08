using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] Image _Color;

    public float Max {get ; protected set;}
    public float CurrentFill {get ; protected set; }
    
    protected BattleMechManager _MyMech;

    protected virtual void Update()
    {
        GetCurrentFill();
    }

    private void GetCurrentFill()
    {
        float fillAmount = CurrentFill / Max;
        _Color.fillAmount = fillAmount;
    }

    public virtual void ChangeCurrent(float f)
    {
        CurrentFill = f;
    }

    public virtual void InitVal(float f, BattleMechManager m)
    {
        Max = f;
        CurrentFill = f;
        _MyMech = m;
    }

    public virtual void Dead()
    {
        CurrentFill = 0f;
        // StopAllCoroutines();
    }
}
