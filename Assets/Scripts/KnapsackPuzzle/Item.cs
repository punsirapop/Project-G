using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Item : MonoBehaviour, IDragHandler
{
    //private string _Name;
    public string Name;
    [SerializeField] TextMeshProUGUI NameLabel;

    void Start()
    {
        // Display the name of item
        NameLabel.text = Name;
    }

    // Make the item draggable
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }
}
