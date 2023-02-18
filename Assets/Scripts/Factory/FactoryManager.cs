using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryManager : MonoBehaviour
{
    // Index for current factory. The index vary from 0 to 3
    private int _CurrentFactory;

    // All sprite for different factory
    [SerializeField] private Sprite[] _Backgrounds;

    // Current sprite for curren factory
    [SerializeField] private Image _CurrentBackground;

    void Start()
    {
        _CurrentFactory = 0;
        _CurrentBackground.sprite = _Backgrounds[_CurrentFactory];
    }
}
