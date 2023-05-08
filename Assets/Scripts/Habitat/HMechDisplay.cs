using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HMechDisplay : MechCanvasDisplay, IPointerDownHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] protected GameObject[] _Indicators;

    protected static int[] _HoldRange;
    protected static int _IsHeld;

    protected int _MyPlace;

    protected bool _IsTriggered;
    protected bool _IsSelected;
    protected Coroutine _HoldCoroutine;

    protected Transform _Manager;

    protected virtual void Start()
    {
        _Manager = transform.parent.parent;

        _IsTriggered = false;
        _IsHeld = -1;
        _HoldRange = new int[2];
        _MyPlace = _Manager.GetComponent<HStorageManager>().Index;
    }

    protected void Update()
    {
        if (Input.GetMouseButtonUp(0) && _HoldCoroutine != null)
        {
            _ShuttingDown();
        }

        int x = transform.GetSiblingIndex();
        if (_IsHeld == _MyPlace && Mathf.Clamp(x, _HoldRange.Min(), _HoldRange.Max()) == x)
        {
            AdjustIndicators(2);
        }
        else
        {
            AdjustIndicators(_IsSelected ? 1 : 0);
        }
    }

    protected void _ShuttingDown()
    {
        if (_HoldCoroutine != null)
        {
            StopCoroutine(_HoldCoroutine);
            _HoldCoroutine = null;
        }
        if (_IsHeld == -1)
        {
            // StopCoroutine(_HoldCoroutine);
            _Manager.SendMessage("SelectMe", gameObject);
            _HoldCoroutine = null;
        }
        else if (_IsHeld == _MyPlace)
        {
            for (int i = _HoldRange.Min(); i <= _HoldRange.Max(); i++)
            {
                _Manager.SendMessage("SelectMe", transform.parent.GetChild(i).gameObject);
            }
        }
        _IsHeld = -1;
        _IsTriggered = false;
        _HoldRange = new int[2];
        _Manager.SendMessage("ControlRolling", true);
        Debug.Log("LIFTED");
    }

    protected IEnumerator _Holding()
    {
        if (_IsTriggered)
        {
            yield return new WaitForSeconds(1f);
            _IsHeld = _MyPlace;
            _HoldRange[0] = transform.GetSiblingIndex();
            _HoldRange[1] = transform.GetSiblingIndex();
            Debug.Log("HELD - " + _HoldRange[0]);
            _Manager.SendMessage("ControlRolling", false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PRESSED");
        _IsTriggered = true;
        _IsHeld = -1;
        _HoldCoroutine = StartCoroutine(_Holding());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_HoldCoroutine != null && _IsHeld == -1)
        {
            _ShuttingDown();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_IsHeld == _MyPlace)
        {
            _HoldRange[1] = transform.GetSiblingIndex();
            Debug.Log("ENTERED - " + _HoldRange[0] + "/" + _HoldRange[1]);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(_IsHeld == -1 && _HoldCoroutine != null)
        {
            StopCoroutine(_HoldCoroutine);
            _HoldCoroutine = null;
            _IsHeld = -1;
            _IsTriggered = false;
            _HoldRange = new int[2];
        }
    }

    /*
     * 0 - clear
     * 1 - select
     * 2 - select batch
     */
    public void AdjustIndicators(int mode)
    {
        switch (mode)
        {
            case 0:
                foreach (var item in _Indicators)
                {
                    item.SetActive(false);
                }
                _IsSelected = false;
                break;
            case 1:
                foreach (var item in _Indicators)
                {
                    item.SetActive(false);
                }
                _Indicators[0].SetActive(true);
                _IsSelected = true;
                break;
            case 2:
                _Indicators[1].SetActive(true);
                break;
        }
    }
}
