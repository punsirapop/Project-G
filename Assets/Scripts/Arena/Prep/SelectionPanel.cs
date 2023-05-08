using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour
{
    [SerializeField] protected Transform _ContentStorage;
    [SerializeField] protected Button[] _Buttons;

    [SerializeField] Image _Panel;
    [SerializeField] Sprite[] _Bgs;
    [SerializeField] GameObject _ItemPrefab;
    // [SerializeField] Dropdown _Pref;

    protected int _CurrentPanel;
    protected ObjectPool<GameObject> _Pool;

    protected virtual void Start()
    {
        _CurrentPanel = 0;
        _Pool = new ObjectPool<GameObject>(
            () => Instantiate(_ItemPrefab, _ContentStorage),
            x =>
            {
                x.SetActive(true);
                x.transform.SetAsLastSibling();
            },
            x =>
            {
                x.SendMessage("AdjustIndicators", 0, SendMessageOptions.DontRequireReceiver);
                x.SetActive(false);
            },
            x => Destroy(x),
            false, 20, 100
            );

    }

    public virtual void OpenPanel(int index)
    {
        if(index != _CurrentPanel)
        {
            _CurrentPanel = index;
            _Panel.sprite = _Bgs[index];
        }
        UpdateValue();
    }

    public virtual void UpdateValue()
    {
        foreach (Transform item in _ContentStorage)
        {
            _Pool.Release(item.gameObject);
        }
    }
}
