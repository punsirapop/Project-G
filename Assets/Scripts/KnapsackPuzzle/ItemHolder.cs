using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHolder : MonoBehaviour, IDropHandler
{
    // Drop Item object on this object by setting this object as the parent
    public void OnDrop(PointerEventData eventData)
    {
        Item item = eventData.pointerDrag.GetComponent<Item>();
        if(item != null)
        {
            item.SetParentReturnTo(this.transform);
        }
    }
}
