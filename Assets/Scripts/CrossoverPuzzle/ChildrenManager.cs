using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildrenManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _CrossoverPoints;

    void Update()
    {
        foreach (GameObject crossoverPoint in _CrossoverPoints)
        {
            bool isOn = crossoverPoint.GetComponent<Toggle>().isOn;
            Image[] images = crossoverPoint.GetComponentsInChildren<Image>();
            Color32 color = isOn ? new Color32(255, 255, 0, 255) : new Color32(255, 255, 255, 255);
            foreach (Image image in images)
            {
                image.color = color;
            }
        }
    }
}
