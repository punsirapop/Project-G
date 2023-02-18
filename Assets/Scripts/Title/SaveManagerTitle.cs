using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveManagerTitle : SaveManager
{
    [SerializeField] Button LoadBtn;

    void Start()
    {
        SetPaths();
    }

    void Update()
    {
        LoadBtn.interactable = SaveReady();
    }
}