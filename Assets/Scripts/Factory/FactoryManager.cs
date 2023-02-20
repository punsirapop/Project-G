using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryManager : MonoBehaviour
{
    public static FactoryManager Instance;

    // Index for current factory. The index vary from 0 to 3
    private int _CurrentFactory;
    [SerializeField] private FactorySO[] _FactoriesData;

    // Current sprite renderer
    [SerializeField] private SpriteRenderer _FloorRenderer;
    [SerializeField] private SpriteRenderer _ConveyorRenderer;
    [SerializeField] private SpriteRenderer _BorderRenderer;

    #region Panels
    // Panels in factory: Info, Produce, ChromoMenu
    [SerializeField] private Button[] _PanelButtons;
    [SerializeField] private GameObject[] _Panels;

    // Text in each panel
    [SerializeField] private TextMeshProUGUI[] _InfoTexts;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _CurrentFactory = 0;
        _RenderSprite();
        _ResetPanels();
    }

    #region Temp function to change the factory #############################################################
    public void AddCurrent()
    {
        _CurrentFactory = _CurrentFactory + 1;
        if (_CurrentFactory >= _FactoriesData.Length)
        {
            _CurrentFactory = 0;
        }
        _RenderSprite();
        _ResetPanels();
    }
    #endregion ########################################################################################

    // Render the weapon holder sprite for each factory
    private void _RenderSprite()
    {
        _FloorRenderer.sprite = _FactoriesData[_CurrentFactory].Floor;
        _ConveyorRenderer.sprite = _FactoriesData[_CurrentFactory].Conveyor;
        _BorderRenderer.sprite = _FactoriesData[_CurrentFactory].Border;
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

    // Reset data to each panel according to the FactorySO
    private void _ResetPanels()
    {
        _InfoTexts[0].text = _FactoriesData[_CurrentFactory].Name;
        _InfoTexts[1].text = _FactoriesData[_CurrentFactory].Problem;
        OpenPanel(0);
    }

    // Return all WeaponChromosome of current factory, just a wrapper function for FactorySO
    public WeaponChromosome[] GetAllWeapon()
    {
        return _FactoriesData[_CurrentFactory].GetAllWeapon();
    }
}
