using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleBodyGenerator : MonoBehaviour
{
    [SerializeField] private Image _RightBodyColor;
    [SerializeField] private Image _LeftBodyColor;
    [SerializeField] private Image _LeftSpareBodyColor;

    // Start is called before the first frame update
    void Start()
    {
        _RightBodyColor.color = Random.ColorHSV();
        _LeftBodyColor.color = Random.ColorHSV();
        _LeftSpareBodyColor.color = Random.ColorHSV();
    }

    public void RefreshColor()
    {
        _RightBodyColor.color = _LeftBodyColor.color;
        _LeftBodyColor.color = _LeftSpareBodyColor.color;
        _LeftSpareBodyColor.color = Random.ColorHSV();
    }
}
