using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChromosomeRodToggle : MonoBehaviour
{
    public bool isOn => GetComponent<Toggle>().isOn;

    void Start()
    {
        // The argument isn't really needed but it require to have some input
        GetComponent<Toggle>().onValueChanged.AddListener((_BitObject) =>
        {
            ChildrenManager.Instance.UpdateChromosomeRods();
        });
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
