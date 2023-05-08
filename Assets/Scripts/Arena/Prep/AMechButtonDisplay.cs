    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class AMechButtonDisplay : AMechDisplay
{
    [SerializeField] Button _Button;

    public void SetButton(bool b)
    {
        _Button.interactable = b;
    }
}
