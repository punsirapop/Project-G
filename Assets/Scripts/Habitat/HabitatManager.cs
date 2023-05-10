using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HabitatManager : MonoBehaviour
{
    List<MechChromo> _CartList;

    private void Awake()
    {
        _CartList = new List<MechChromo>();
    }
}
