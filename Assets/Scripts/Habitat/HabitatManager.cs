using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabitatManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _AllCoverGlass;

    List<MechChromo> _CartList;

    private void Awake()
    {
        _CartList = new List<MechChromo>();
        for (int i = 0; i < PlayerManager.FarmDatabase.Length; i++)
        {
            bool isBreeding = (PlayerManager.FarmDatabase[i].Status == Status.BREEDING);
            _AllCoverGlass[i].SetActive(isBreeding);
        }
    }
}
