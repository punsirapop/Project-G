using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static ChromoMenu;

public class QMechSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject _Prefab;
    [SerializeField] Transform _MechHolder;
    [SerializeField] ScrollRect _Scroll;
    [SerializeField] GameObject _AutoScroller;

    ObjectPool<GameObject> _Pool;
    // public List<GameObject> Selected { get; private set; }
    public GameObject Selected { get; private set; }

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
            false, 20, 400
            );
        // Selected = new List<GameObject>();
        Selected = null;
    }

    public void Set(SideQuestSO sq, Tuple<MechChromo, int> selected)
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
        List<Tuple<MechChromo, int>> list = new List<Tuple<MechChromo, int>>();
        for (int i = 0; i < PlayerManager.FarmDatabase.Length; i++)
        {
            if (PlayerManager.FarmDatabase[i].Status != Status.BREEDING &&
                PlayerManager.FarmDatabase[i].LockStatus == LockableStatus.Unlock)
            {
                foreach (var item in PlayerManager.FarmDatabase[i].MechChromos)
                {
                    list.Add(Tuple.Create(item, i));
                }
            }
        }
        if (list.Count() > 0)
        {
            Debug.Log("----------Check sorted list-----------");
            list = list.OrderByDescending(x => sq.WantedMech.CompareMechQuest(x.Item1)).ToList();
            foreach (var item in list)
            {
                Debug.Log(item.Item1.CompareMechQuest(sq.WantedMech));
            }
            // ------- display -------
            foreach (var item in list)
            {
                GameObject m = _Pool.Get();
                m.GetComponent<MechCanvasDisplay>().SetChromo(item.Item1);
                m.GetComponent<QMechSelector>().FromFarm = item.Item2;
            }
            foreach (Transform item in _MechHolder)
            {
                if (selected != null && item.GetComponent<QMechSelector>().FromFarm == selected.Item2 &&
                    item.GetComponent<QMechSelector>().MyMechSO == selected.Item1)
                    SelectMe(item.gameObject);
            }
        }
    }

    /*
    public void SelectMe(GameObject m)
    {
        if (Selected.Contains(m))
        {
            Selected.Remove(m);
            m.SendMessage("AdjustIndicators", value: 0);
        }
        else
        {
            Selected.Add(m);
            m.SendMessage("AdjustIndicators", value: 1);
        }
        Debug.Log(Selected.Count);
    }
    */

    public void SelectMe(GameObject m)
    {
        if (Selected == m)
        {
            Selected = null;
            m.SendMessage("AdjustIndicators", value: 0);
        }
        else
        {
            if (Selected != null) Selected.SendMessage("AdjustIndicators", value: 0);
            Selected = m;
            m.SendMessage("AdjustIndicators", value: 1);
        }
    }

    public void ControlRolling(bool f)
    {
        _Scroll.vertical = f;
        _AutoScroller.SetActive(!f);
    }

    public void ClosePanel()
    {
        if (Selected != null)
        {
            QMechSelector s = Selected.GetComponent<QMechSelector>();
            transform.parent.SendMessage("UpdateSelection", Tuple.Create(s.MyMechSO, s.FromFarm));
        }
        else
        {
            transform.parent.SendMessage("UpdateSelection", Tuple.Create<MechChromo, int>(null, -1));
        }
    }
}
