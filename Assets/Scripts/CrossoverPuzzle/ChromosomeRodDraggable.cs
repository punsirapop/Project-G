using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChromosomeRodDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Transform[] _BitContent;
    private int[] _CrossoverPoints;
    private float _OriginYPosition;
    private Transform[] _DraggedSection;

    void Start()
    {
        _OriginYPosition = this.transform.position.y;
        _GetBitContent();
    }

    private void _GetBitContent()
    {
        // Keep track of all bit in this chromosome
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

    // Calculate which part of chromosome will be dragged according to mouse position and crossover points
    private int[] _GetSectionIndexes()
    {
        _CrossoverPoints = ChildrenManager.Instance.GetCrossoverPoints();
        int beginBitIndex = 0;
        int endBitIndex = _BitContent.Length - 1;
        for (int i = 0; i < _BitContent.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_BitContent[i].gameObject.GetComponent<RectTransform>(), Input.mousePosition))
            {
                foreach (int crossoverPoint in _CrossoverPoints)
                {
                    if (i > crossoverPoint)
                    {
                        beginBitIndex = crossoverPoint + 1;
                    }
                    else if (i <= crossoverPoint)
                    {
                        endBitIndex = crossoverPoint;
                        break;
                    }
                }
                break;
            }
        }
        return new int[] { beginBitIndex, endBitIndex };
    }

    private Transform[] _GetClickedSection(int beginBitIndex, int endBitIndex)
    {
        Transform[] draggedSection = new Transform[endBitIndex - beginBitIndex + 1];
        // Calculate the actual dragged GameObjects
        for (int i = 0; i < _BitContent.Length; i++)
        {
            if ((i >= beginBitIndex) && (i <= endBitIndex))
            {
                draggedSection[i - beginBitIndex] = _BitContent[i];
            }
        }
        return draggedSection;
    }

    // Calculate the part to be dragged
    public void OnBeginDrag(PointerEventData eventData)
    {
        _OriginYPosition = this.transform.position.y;
        _GetBitContent();
        // Record the origin position
        _OriginYPosition = this.transform.position.y;
        int[] sectionIndexes = _GetSectionIndexes();
        _DraggedSection = _GetClickedSection(sectionIndexes[0], sectionIndexes[1]);
        ChildrenManager.Instance.SetDraggedIndexes(sectionIndexes);
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;    // Make other gameObject capture the raycasts (the mouse pointer)
        this.GetComponent<HorizontalLayoutGroup>().enabled = false;
    }

    // While dragging, change position in y axis of the dragged section to the same as the mouse pointer
    public void OnDrag(PointerEventData eventData)
    {
        foreach (Transform dragged in _DraggedSection)
        {
            dragged.position = new Vector3(dragged.transform.position.x, eventData.position.y, 0);
        }
    }

    // OnDrop() is called before OnEndDrag(), swap content of bit if another Draggable drop on this object
    // Note that the dragged object is not the same object that call OnDrop()
    public void OnDrop(PointerEventData eventData)
    {
        ChildrenManager.Instance.UpdateSwapping();
    }

    // When stop dragging, change its position back to the origin
    public void OnEndDrag(PointerEventData eventData)
    {
        // Align all bit content back in to the position before drag
        foreach (Transform dragged in _BitContent)
        {
            dragged.position = new Vector3(dragged.transform.position.x, _OriginYPosition, 0);
        }
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        this.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
