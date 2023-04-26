using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayedBar : Bar
{
    [SerializeField] Image _White;

    float _CurrentWhite;
    Coroutine _Delaying;

    public override void ChangeCurrent(float f)
    {
        base.ChangeCurrent(f);
        if (_Delaying != null) StopCoroutine(_Delaying);
        _Delaying = StartCoroutine(Delay());
    }

    public override void InitVal(float f)
    {
        base.InitVal(f);
        _CurrentWhite = f;
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(.5f);

        float startTime = Time.time;
        float startNumber = _CurrentWhite;
        float endNumber = CurrentFIll;

        while (_CurrentWhite != endNumber) // keep checking until currentNumber matches the new targetNumber
        {
            float t = (Time.time - startTime) / .25f;
            _CurrentWhite = Mathf.Lerp(startNumber, endNumber, t);
            _White.fillAmount = _CurrentWhite / Max;
            yield return null;
        }

        _Delaying = null;
    }
}
