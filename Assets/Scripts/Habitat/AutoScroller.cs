using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScroller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ScrollRect _Scroll;
    [SerializeField] bool _ThisSideUp;

    bool _IsHovering;

    void OnEnable()
    {
        _IsHovering = false;
    }

    void Update()
    {
        var disToMove = Time.deltaTime * (_ThisSideUp ? 1 : -1);
        if ((_Scroll.verticalNormalizedPosition < 1f && _ThisSideUp) ||
            (_Scroll.verticalNormalizedPosition > 0f && !_ThisSideUp))
            _Scroll.verticalNormalizedPosition += _IsHovering ? disToMove : 0;
        /*
        if (_IsHovering)
        {
            if (_ThisSideUp)
            {
                if (_Scroll.verticalNormalizedPosition < 1f)
                    _Scroll.verticalNormalizedPosition += disToMove;
            }
            else
            {
                if (_Scroll.verticalNormalizedPosition > 0f)
                    _Scroll.verticalNormalizedPosition -= disToMove;
            }
        }
        */
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ENTERED " + _Scroll.verticalNormalizedPosition);
        _IsHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("LEFT " + _Scroll.verticalNormalizedPosition);
        _IsHovering = false;
    }
}
