using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BitHolderHelper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform[] _BitContent;
    private int _ClickedBit;
    private int _BeginBitIndex;
    private int _EndBitIndex;
    private int[] _CrossoverPoints;
    void Start()
    {
        int bitCount = 0;
        foreach (Transform t in this.transform)
        {
            bitCount++;
        }
        _BitContent = new Transform[bitCount];
        int i = 0;
        foreach (Transform t in this.transform)
        {
            _BitContent[i] = t;
            i++;
        }
    }

    // When start dragging, get this object out from its parent
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Calculate which part of chromosome will be dragged
        _CrossoverPoints = ChildrenManager.Instance.GetCrossoverPoints();
        _BeginBitIndex = 0;
        _EndBitIndex = _BitContent.Length - 1;
        Debug.Log("Bitcontent Length = " + _BitContent.Length.ToString());
        for (int i = 0; i < _BitContent.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_BitContent[i].gameObject.GetComponent<RectTransform>(), Input.mousePosition))
            {
                _ClickedBit = i;
                foreach (int crossoverPoint in _CrossoverPoints)
                {
                    if (i > crossoverPoint)
                    {
                        _BeginBitIndex = crossoverPoint + 1;
                    }
                    else if (i <= crossoverPoint)
                    {
                        _EndBitIndex = crossoverPoint;
                        break;
                    }
                }
                break;
            }
        }
        Debug.Log("Begin= " + _BeginBitIndex.ToString());
        Debug.Log("End= " + _BeginBitIndex.ToString());
        // Make other gameObject capture the raycasts (the mouse pointer)
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        // Make the parent ignore this object
        this.GetComponent<LayoutElement>().ignoreLayout = true;
    }

    // While dragging, change position of this object to the same as the mouse pointer
    public void OnDrag(PointerEventData eventData)
    {
        for (int i = 0; i < _BitContent.Length; i++)
        {
            if ((i >= _BeginBitIndex) && (i <= _EndBitIndex))
            {
                _BitContent[i].position = new Vector3(_BitContent[i].transform.position.x, eventData.position.y, 0);
            }
        }
        //this.transform.position = new Vector3(this.transform.position.x, eventData.position.y, 0);
    }

    // When stop dragging, snap it back into the layout parent
    // Reversing everything in the OnBeginDrag()
    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        //this.GetComponent<LayoutElement>().ignoreLayout = false;
    }
}
