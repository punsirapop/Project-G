using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CartSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] Slider _CartSlider;
    [SerializeField] Image _CartHandle;
    [SerializeField] Sprite[] _CartImages;
    [SerializeField] HStorageManager[] _Storages;

    int _WhereDidITakeIt;
    float _InitVal;

    static List<MechChromo> _CartChromo;
    public static List<MechChromo> CartChromo => _CartChromo;

    private void Awake()
    {
        _CartChromo = new List<MechChromo>();
        _WhereDidITakeIt = -1;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_InitVal != _CartSlider.value) _CartSlider.value = Mathf.Round(_CartSlider.value);
        else if (_CartChromo.Count == 0 && _Storages[(int)_CartSlider.value].Selected.Count > 0)
        {
            _CartHandle.sprite = _CartImages[1];
            _CartChromo.AddRange(_Storages[(int)_CartSlider.value].Selected.
                Select(x => x.GetComponent<MechCanvasDisplay>().MyMechSO));
            _WhereDidITakeIt = (int)_CartSlider.value;
            /*
            foreach (var item in _CartChromo)
            {
                PlayerManager.FarmDatabase[(int)_CartSlider.value].DelChromo(item);
            }
            */
            _Storages[(int)_CartSlider.value].Selected.Clear();
            _Storages[(int)_CartSlider.value].OnValueChange();
        }
        else if (_CartChromo.Count > 0 &&
            PlayerManager.FarmDatabase[(int)_CartSlider.value].LockStatus != LockableStatus.Lock)
        {
            _CartHandle.sprite = _CartImages[0];
            foreach (var item in _CartChromo)
            {
                PlayerManager.FarmDatabase[_WhereDidITakeIt].DelChromo(item);
            }
            foreach (var item in _CartChromo)
            {
                PlayerManager.FarmDatabase[(int)_CartSlider.value].AddChromo(item);
            }
            _CartChromo.Clear();
            _Storages[(int)_CartSlider.value].OnValueChange();
        }
        SoundEffectManager.Instance.PlaySoundEffect("Pick");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _InitVal = Mathf.Round(_CartSlider.value);
        SoundEffectManager.Instance.PlaySoundEffect("Drop");
    }
}
