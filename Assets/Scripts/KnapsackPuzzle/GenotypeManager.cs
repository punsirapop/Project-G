using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenotypeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI BitCountText;
    private int _BitCount;

    // Start is called before the first frame update
    void Start()
    {
        _BitCount = 0;
    }

    private void _UpdateValue()
    {
        BitCountText.text = _BitCount.ToString();
    }

    public void AddBit()
    {
        _BitCount = _BitCount + 1;
        _UpdateValue();
    }

    public void RemoveBit()
    {
        _BitCount = _BitCount - 1;
        _UpdateValue();
    }
}
