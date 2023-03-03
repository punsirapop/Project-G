using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentManager : MonoBehaviour
{
    [SerializeField] private GameObject ChromoButtonPrefab;
    [SerializeField] private Transform _ChromoButtonHolder;
    [SerializeField] private Color32[] _Colors;
    private ChromoButton[] _ChromoButtons;

    void Start()
    {
        // Destroy all previous object in the holder
        foreach (Transform child in _ChromoButtonHolder)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 6; i++)
        {
            int[] content = new int[5];
            for (int j = 0; j < content.Length; j++)
            {
                content[j] = Random.Range(0, 2);
            }
            GameObject newChromoButton = Instantiate(ChromoButtonPrefab, _ChromoButtonHolder);
            newChromoButton.GetComponent<ChromoButton>().SetChromoButton(content, _Colors[i]);
        }
    }

    void Update()
    {
        // Make sure that there are at most 2 button selected
        _ChromoButtons = GetComponentsInChildren<ChromoButton>();
        // Count the number of selected button
        int selectCount = 0;
        foreach (ChromoButton chromoButton in _ChromoButtons)
        {
            selectCount += chromoButton.isOn ? 1 : 0;
            chromoButton.SetInteractable(true);
        }
        // Disable other if there are already 2 button selected
        if (selectCount < 2)
        {
            return;
        }
        selectCount = 0;
        foreach (ChromoButton chromoButton in _ChromoButtons)
        {
            if (!chromoButton.isOn)
            {
                chromoButton.SetInteractable(false);
            }
            else
            {
                if (selectCount >= 2)
                {
                    chromoButton.SetIsOn(false);
                    chromoButton.SetInteractable(false);
                }
                selectCount++;
            }
        }
    }
}
