using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChromoButton : MonoBehaviour
{
    public int Length => _BitContent.Length;
    private int[] _BitContent;
    public bool isOn => GetComponent<Toggle>().isOn;
    [SerializeField] private GameObject _BitPrefab;
    [SerializeField] private GameObject _BitHolder;
    private GameObject[] _BitObject;

    void Start()
    {
        // The argument isn't really needed but it require to have some input
        GetComponent<Toggle>().onValueChanged.AddListener((_BitObject) =>
        {
            ChildrenManager.Instance.UpdateChromoButton();
        });
    }

    public GameObject GetBitHolder()
    {
        return _BitHolder;
    }

    public void SetChromoButton(int[] newContent, Color32 newColor)
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _BitHolder.transform)
        {
            Destroy(child.gameObject);
        }

        _BitContent = newContent;
        _BitObject = new GameObject[newContent.Length];
        for (int i = 0; i < _BitContent.Length; i++)
        {
            _BitObject[i] = Instantiate(_BitPrefab, _BitHolder.transform);
            _BitObject[i].GetComponentInChildren<TextMeshProUGUI>().text = _BitContent[i].ToString();
            _BitObject[i].GetComponentInChildren<Image>().color = newColor;
        }
    }

    public void SetIsOn(bool newIsOn)
    {
        GetComponent<Toggle>().isOn = newIsOn;
    }

    public void SetInteractable(bool newInteractable)
    {
        GetComponent<Toggle>().interactable = newInteractable;
    }
}
