using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] Image _Color;

    public float Max {get ; protected set;}
    public float CurrentFIll {get ; protected set; }

    private void Update()
    {
        GetCurrentFill();
    }

    private void GetCurrentFill()
    {
        float fillAmount = CurrentFIll / Max;
        _Color.fillAmount = fillAmount;
    }

    public virtual void ChangeCurrent(float f)
    {
        CurrentFIll = f;
    }

    public virtual void InitVal(float f)
    {
        Max = f;
        CurrentFIll = f;
    }
}
