using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossoverPoint : MonoBehaviour
{
    [SerializeField] private Toggle _Toggle;
    public bool isOn => _Toggle.isOn;
    [SerializeField] private Image _Line;
    private Color32 _OnColor = new Color32(0, 0, 255, 255);
    private Color32 _OffColor;

    public void SetIsOn(bool newIsOn)
    {
        _Toggle.isOn = newIsOn;
    }

    public void ColorObject()
    {
        _Toggle.gameObject.GetComponent<Image>().color = _Toggle.isOn ? _OnColor : _OffColor;
        _Line.color = _Toggle.isOn ? _OnColor : _OffColor;
    }

    // Set the point whether it's interactable
    public void SetInteractable(bool isInteractable)
    {
        _Toggle.interactable = isInteractable;
        // Make all components transparent by default if not interactable, otherwise, make them opaque
        _OffColor = isInteractable ? new Color32(255, 255, 255, 200) : new Color32(0, 0, 0, 0);
        ColorObject();
    }
}
