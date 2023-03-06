using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChromosomeRod : MonoBehaviour
{
    // Make first 3 variables [SerializeField] to make they able to be copied using Instantiate()
    // used in UpdateSwapping() of ChildrenManager
    [SerializeField] private int[] _ChromosomeValue;
    [SerializeField] private Color32[] _ChromosomeColor;
    [SerializeField] private bool _IsMech;

    [SerializeField] private GameObject _BitContentPrefab;
    [SerializeField] private Sprite[] _MechHeadSprites;
    [SerializeField] private Sprite[] _MechAccSprites;

    public void SetChromosome(int[] newValue, Color32[] newColor, bool newIsMech=false)
    {
        _ChromosomeValue = newValue;
        _ChromosomeColor = newColor;
        _IsMech = newIsMech;
    }

    public int GetValueAtIndex(int valueIndex)
    {
        return _ChromosomeValue[valueIndex];
    }

    public int[] GetChromosomeValue()
    {
        return _ChromosomeValue;
    }

    public void SetValueAtIndex(int valueIndex, int newValue)
    {
        _ChromosomeValue[valueIndex] = newValue;
    }

    public Color32 GetColorAtIndex(int colorIndex)
    {
        return _ChromosomeColor[colorIndex];
    }

    public Color32[] GetColorValue()
    {
        return _ChromosomeColor;
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
        // If it's not a mech, render as plain color
        if (!_IsMech)
        {
            // Create new bit object
            for (int i = 0; i < _ChromosomeValue.Length; i++)
            {
                GameObject newBitContent = Instantiate(_BitContentPrefab, this.transform);
                newBitContent.GetComponentInChildren<TextMeshProUGUI>().text = _ChromosomeValue[i].ToString();
                newBitContent.GetComponentsInChildren<Image>()[1].color = _ChromosomeColor[i];
            }
        }
        // If it's a mech, render as mech chromosome
        else
        {
            // Create new head section
            GameObject headBitContent = Instantiate(_BitContentPrefab, this.transform);
            headBitContent.GetComponentInChildren<TextMeshProUGUI>().text = "";
            headBitContent.GetComponentsInChildren<Image>()[1].color = _ChromosomeColor[0];
            headBitContent.GetComponentsInChildren<Image>()[2].sprite = _MechHeadSprites[_ChromosomeValue[0]];
            headBitContent.GetComponentsInChildren<Image>()[2].color = Color.white;
            // Create new body color section
            Color32 bodyColor = new Color32((byte)_ChromosomeValue[1], (byte)_ChromosomeValue[2], (byte)_ChromosomeValue[3], 255);
            for (int i = 1; i <= 3; i++)
            {
                GameObject newBitContent = Instantiate(_BitContentPrefab, this.transform);
                newBitContent.GetComponentInChildren<TextMeshProUGUI>().text = _ChromosomeValue[i].ToString();
                // Don't no why cann't add text outline in the editor so I add it through script here
                newBitContent.GetComponentInChildren<TextMeshProUGUI>().outlineWidth = 0.3f;
                newBitContent.GetComponentInChildren<TextMeshProUGUI>().outlineColor = Color.white;
                newBitContent.GetComponentsInChildren<Image>()[1].color = bodyColor;
            }
            // Create new accessory section
            GameObject accBitContent = Instantiate(_BitContentPrefab, this.transform);
            accBitContent.GetComponentInChildren<TextMeshProUGUI>().text = "";
            accBitContent.GetComponentsInChildren<Image>()[1].color = _ChromosomeColor[0];
            accBitContent.GetComponentsInChildren<Image>()[3].sprite = _MechAccSprites[_ChromosomeValue[4]];
            accBitContent.GetComponentsInChildren<Image>()[3].color = Color.white;
        }
    }
}
