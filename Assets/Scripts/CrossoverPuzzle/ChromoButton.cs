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
    [SerializeField] private Transform _BitHolder;
    private GameObject[] _BitObject;

    public void SetChromoButton(int[] newContent, Color32 newColor)
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _BitHolder)
        {
            Destroy(child.gameObject);
        }

        _BitContent = newContent;
        _BitObject = new GameObject[newContent.Length];
        for (int i = 0; i < _BitContent.Length; i++)
        {
            _BitObject[i] = Instantiate(_BitPrefab, _BitHolder);
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
