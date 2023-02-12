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

    // Return the total value of the item in the holder
    public int[] GetTotalProperties()
    {
        int[] properties = { 0, 0, 0 }; // value, weight1, weight2
        Item[] items = GetComponentsInChildren<Item>();
        foreach (Item item in items)
        {
            properties[0] = properties[0] + item.Value;
            properties[1] = properties[1] + item.Weight1;
            properties[2] = properties[2] + item.Weight2;
        }
        return properties;
    }
}
