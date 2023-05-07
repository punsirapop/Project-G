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
        _DefaultObject.SetActive(true);
        _OnHoverObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _DefaultObject.SetActive(false);
        _OnHoverObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _DefaultObject.SetActive(true);
        _OnHoverObject.SetActive(false);
    }
}
