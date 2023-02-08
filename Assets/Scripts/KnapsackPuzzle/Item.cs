using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    private string _Name;
    public string Name => _Name;
    [SerializeField] TextMeshProUGUI NameLabel;

    // Start is called before the first frame update
    void Start()
    {
        _Name = "A";
        NameLabel.text = _Name;
    }

    void OnClick()
    {
        Debug.Log("Clicked");
    }
}
