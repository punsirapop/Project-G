using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _IndicatorDot;

    public void SetIndicator(int currentStep, int maxStep)
    {
        if ((currentStep > maxStep) ||
            (currentStep < 0))
        {
            return;
        }
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i <= maxStep; i++)
        {
            GameObject newDot = Instantiate(_IndicatorDot, this.transform);
            newDot.GetComponent<Image>().color = (i == currentStep) ? Color.white : Color.gray;
        }
    }
}
