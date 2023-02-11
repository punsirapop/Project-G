using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string Name;
    [SerializeField] TextMeshProUGUI NameLabel;
    private Transform _ParentReturnTo;

    // Set the parent where it's supposed to drop on
    public void SetParentReturnTo(Transform newParent)
    {
        _ParentReturnTo = newParent;
    }

    void Start()
    {
        // Display the name of item
        NameLabel.text = Name;
        // Record the initial parent
        _ParentReturnTo = this.transform.parent;
    }

    // When start dragging, get this object out from its parent
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Pop this item out of its parent, 2 step above to make this object be rendered on top of knapsack..
        this.transform.SetParent(_ParentReturnTo.parent.parent);
        // Make the parent ignore this object
        this.GetComponent<LayoutElement>().ignoreLayout = true;
        // Make other gameObject capture the raycasts (the mouse pointer)
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // While dragging, change position of this object to the same as the mouse pointer
    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    // When stop dragging, snap it back into the layout parent
    // Reversing everything in the OnBeginDrag()
    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(_ParentReturnTo);
        this.GetComponent<LayoutElement>().ignoreLayout = false;
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
