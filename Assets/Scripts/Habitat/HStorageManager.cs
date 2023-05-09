using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static ChromoMenu;

public class HStorageManager : MonoBehaviour
{
    public int Index;

    [SerializeField] GameObject _Prefab;
    [SerializeField] Transform _MechHolder;
    [SerializeField] ScrollRect _Scroll;
    [SerializeField] GameObject _AutoScroller;
    [SerializeField] HFitnessMenu _FitnessMenu;
    [SerializeField] Sprite _Locked;
    [SerializeField] Button _NamePlate;

    FarmSO _MyFarm => PlayerManager.FarmDatabase[Index];
    ObjectPool<GameObject> _Pool;

    List<GameObject> _Selected;
    public List<GameObject> Selected => _Selected;

    private void Awake()
    {
        _Pool = new ObjectPool<GameObject>(
            () => Instantiate(_Prefab, _MechHolder),
            mech =>
            {
                mech.SetActive(true);
                mech.transform.SetAsLastSibling();
            },
            mech => 
            {
                mech.SendMessage("AdjustIndicators", 0, SendMessageOptions.DontRequireReceiver);
                mech.SetActive(false); 
            },
            mech => Destroy(mech),
            false, 20, 100
            );
        _Selected = new List<GameObject>();

        if (PlayerManager.FarmDatabase[Index].LockStatus == LockableStatus.Unlock)
        {
            OnValueChange();
        }
        else
        {
            GetComponent<Image>().sprite = _Locked;
            _NamePlate.interactable = false;
        }

        _FitnessMenu.gameObject.SetActive(false);
    }

    public void OnValueChange()
    {
        foreach (Transform item in _MechHolder)
        {
            _Pool.Release(item.gameObject);
        }
        /*
        foreach (var item in _MyFarm.MechChromos)
        {
            GameObject m = _Pool.Get();
            m.GetComponent<MechCanvasDisplay>().SetChromo(item);
        }
        */
        if (_MyFarm.MechChromos.Except(CartSlider.CartChromo).Count() > 0)
        {
            Dictionary<dynamic, float> fvDict = _FitnessMenu.GetFitnessDict();
            fvDict = fvDict.Where(x => !CartSlider.CartChromo.Contains(x.Key)).
                ToDictionary(x => x.Key, x => x.Value);
            List<OrderFormat> fv = new List<OrderFormat>();
            foreach (var item in fvDict)
            {
                MechChromoSO c = item.Key;
                OrderFormat of = new OrderFormat();
                of.name = c.ID;
                of.chromo = c;
                of.fitness = item.Value;
                fv.Add(of);
            }
            // ------- sort -------
            fv = fv.OrderByDescending(x => x.fitness).ThenBy(x => x.name).ToList();
            // ------- display -------
            foreach (var item in fv)
            {
                GameObject m = _Pool.Get();
                m.GetComponent<MechCanvasDisplay>().SetChromo(item.chromo);
                // Debug.Log(item.name + " - " + item.fitness);
            }
        }
    }

    public void SelectMe(GameObject m)
    {
        if (_Selected.Contains(m))
        {
            _Selected.Remove(m);
            m.SendMessage("AdjustIndicators", value: 0);
        }
        else
        {
            _Selected.Add(m);
            m.SendMessage("AdjustIndicators", value: 1);
        }
        Debug.Log(_Selected.Count);
    }

    public void ControlRolling(bool f)
    {
        _Scroll.vertical = f;
        _AutoScroller.SetActive(!f);
    }

    public void ToggleFitness()
    {
        Debug.Log("TOGGLE FIT " + Index);
        _FitnessMenu.gameObject.SetActive(!_FitnessMenu.gameObject.activeSelf);
    }
}
