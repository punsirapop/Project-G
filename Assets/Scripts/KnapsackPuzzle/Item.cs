using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Information of the item
    private string _Name;
    public string Name => _Name;
    private int _Value;
    public int Value => _Value;
    private int _Weight1;
    public int Weight1 => _Weight1;
    private int _Weight2;
    public int Weight2 => _Weight2;

    // UI element to display the information
    [SerializeField] private TextMeshProUGUI _NameText;
    [SerializeField] private TextMeshProUGUI _ValueText;
    [SerializeField] private TextMeshProUGUI _WeightText;

    // Reference to the parent layout for dealing with drag and drop
    private Transform _Mask;
    private Transform _ParentReturnTo;
    private bool _IsDraggable;

    #region Data Feild Setter
    // Set the information of the item
    public void SetItem(string name, int value, int weight1, int weight2)
    {
        _Name = name;
        _Value = value;
        _Weight1 = weight1;
        _Weight2 = weight2;
        _RenderInfo();
    }

    // Set the information of the item
    public void SetItem(ItemSO itemSO)
    {
        _Name = itemSO.Name;
        _Value = itemSO.Value;
        _Weight1 = itemSO.Weight1;
        _Weight2 = itemSO.Weight2;
        _RenderInfo();
    }

    // Set the mask where the item is in
    public void SetMask(Transform mask)
    {
        _Mask = mask;
    }

    // Set the parent where it's supposed to drop on (after it is dragged)
    public void SetParentReturnTo(Transform newParent)
    {
        _ParentReturnTo = newParent;
    }

    // Set draggable
    public void SetDraggable(bool draggable)
    {
        _IsDraggable = draggable;
    }
    #endregion

    public int GetDimension()
    {
        if (_Weight2 > 0)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    void Awake()
    {
        // Set the information to some default value
        SetItem("I", 99, 99, 99);
        _IsDraggable = true;
    }

    void Start()
    {
        // Record the initial parent
        _ParentReturnTo = this.transform.parent;
    }

    // Render the information into the UI
    private void _RenderInfo()
    {
        _NameText.text = _Name;
        _ValueText.text = _Value.ToString();
        string weight2Text = "";
        if (Weight2 > 0)
        {
            weight2Text = " | " + Weight2.ToString();
        }
        _WeightText.text = Weight1.ToString() + weight2Text;
    }

    #region Dragging behavior
    // When start dragging, get this object out from its parent
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_IsDraggable)
        {
            // Pop this item out of its parent, put it in the mask
            this.transform.SetParent(_Mask);
            // Make the parent ignore this object
            this.GetComponent<LayoutElement>().ignoreLayout = true;
            // Make other gameObject capture the raycasts (the mouse pointer)
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    // While dragging, change position of this object to the same as the mouse pointer
    public void OnDrag(PointerEventData eventData)
    {
        if (_IsDraggable)
        {
            this.transform.position = eventData.position;
        }
    }

    // When stop dragging, snap it back into the layout parent
    // Reversing everything in the OnBeginDrag()
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_IsDraggable)
        {
            this.transform.SetParent(_ParentReturnTo);
            this.GetComponent<LayoutElement>().ignoreLayout = false;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
    #endregion
}
