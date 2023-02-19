using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryManager : MonoBehaviour
{
    // Index for current factory. The index vary from 0 to 3
    private int _CurrentFactory;

    // All sprites for different factory
    [SerializeField] private Sprite[] _Floors;
    [SerializeField] private Sprite[] _Conveyors;
    [SerializeField] private Sprite[] _Borders;

    // Current sprite renderer of each part of factory
    [SerializeField] private SpriteRenderer _FloorRenderer;
    [SerializeField] private SpriteRenderer _ConveyorRenderer;
    [SerializeField] private SpriteRenderer _BorderRenderer;

    // Panels in factory: Info, Breed, ChromoMenu
    [SerializeField] private Button[] _PanelButtons;
    [SerializeField] private GameObject[] _Panels;

    void Start()
    {
        _CurrentFactory = 0;
        _RenderSprite();
        OpenPanel(0);
    }

    // Temp function to change the factory ////////////////////////////////////////////////////////
    public void AddCurrent()
    {
        _CurrentFactory = _CurrentFactory + 1;
        if (_CurrentFactory >= _Floors.Length)
        {
            _CurrentFactory = 0;
        }
        _RenderSprite();
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////

    // Render the weapon holder sprite for each factory
    private void _RenderSprite()
    {
        _FloorRenderer.sprite = _Floors[_CurrentFactory];
        _ConveyorRenderer.sprite = _Conveyors[_CurrentFactory];
        _BorderRenderer.sprite = _Borders[_CurrentFactory];
    }

    // Change panel
    public void OpenPanel(int i)
    {
        foreach (Button button in _PanelButtons)
        {
            button.interactable = true;
        }
        _PanelButtons[i].interactable = false;
        foreach (var panel in _Panels)
        {
            panel.SetActive(false);
        }
        _Panels[i].SetActive(true);
    }
}
