using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryManager : MonoBehaviour
{
    public static FactoryManager Instance;

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

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _RenderSprite();
        _ResetPanels();
    }

    // Render the weapon holder sprite for each factory
    private void _RenderSprite()
    {
        _FloorRenderer.sprite = PlayerManager.CurrentFactoryDatabase.Floor;
        _ConveyorRenderer.sprite = PlayerManager.CurrentFactoryDatabase.Conveyor;
        _BorderRenderer.sprite = PlayerManager.CurrentFactoryDatabase.Border;
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
        _InfoTexts[0].text = PlayerManager.CurrentFactoryDatabase.Name;
        _InfoTexts[1].text = PlayerManager.CurrentFactoryDatabase.Description;
        OpenPanel(0);
    }

    // Return all WeaponChromosome of current factory, just a wrapper function for FactorySO
    public WeaponChromosome[] GetAllWeapon()
    {
        return PlayerManager.CurrentFactoryDatabase.GetAllWeapon();
    }
}
