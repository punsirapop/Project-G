using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Class for swapping object upon hover on something
public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _DefaultObject;
    [SerializeField] private GameObject _OnHoverObject;

    private void OnEnable()
    {
        if (_DefaultObject != null) _DefaultObject.SetActive(true);
        if (_OnHoverObject != null) _OnHoverObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_DefaultObject != null) _DefaultObject.SetActive(false);
        if (_OnHoverObject != null) _OnHoverObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_DefaultObject != null) _DefaultObject.SetActive(true);
        if (_OnHoverObject != null) _OnHoverObject.SetActive(false);
    }
}
