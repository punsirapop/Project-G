using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChromosomeRod : MonoBehaviour
{
    // Make first 2 variables [SerializeField] to make they able to be copied using Instantiate()
    // used in UpdateSwapping() of ChildrenManager
    [SerializeField] private int[] _ChromosomeValue;
    [SerializeField] private Color32[] _ChromosomeColor;
    [SerializeField] private GameObject _BitContentPrefab;

    public void SetChromosome(int[] newValue, Color32[] newColor, bool isRender=true)
    {
        _ChromosomeValue = newValue;
        _ChromosomeColor = newColor;
        if (isRender)
        {
            RenderRod();
        }
    }

    public int GetValueAtIndex(int valueIndex)
    {
        return _ChromosomeValue[valueIndex];
    }

    public void SetValueAtIndex(int valueIndex, int newValue)
    {
        _ChromosomeValue[valueIndex] = newValue;
    }

    public Color32 GetColorAtIndex(int colorIndex)
    {
        return _ChromosomeColor[colorIndex];
    }

    public void SetColorAtIndex(int colorIndex, Color32 newColor)
    {
        _ChromosomeColor[colorIndex] = newColor;
    }


    // Render the bit object in the rod
    public void RenderRod()
    {
        // Destroy all current bit object
        foreach (Transform t in this.transform)
        {
            Destroy(t.gameObject);
        }
        // Create new bit object
        for (int i = 0; i < _ChromosomeValue.Length; i++)
        {
            GameObject newBitContent = Instantiate(_BitContentPrefab, this.transform);
            newBitContent.GetComponentInChildren<TextMeshProUGUI>().text = _ChromosomeValue[i].ToString();
            newBitContent.GetComponentInChildren<Image>().color = _ChromosomeColor[i];
        }
    }
}
