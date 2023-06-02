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

    // Panels in factory: Info, Produce, ChromoMenu
    [SerializeField] private Button[] _PanelButtons;
    [SerializeField] private GameObject[] _Panels;

    // Text in info panel
    [SerializeField] private TextMeshProUGUI[] _InfoTexts;

    // Status panel
    [SerializeField] private GameObject[] _StatusDisplays;
    [SerializeField] private TextMeshProUGUI _BreedingGenDisplay;
    [SerializeField] private Image _GaugeRenderer;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        _RenderSprite();
        _ResetPanels();
        RenderStatusPanel();
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

    // Display the information from FactorySO on the status panel
    public void RenderStatusPanel()
    {
        // Hide all the status
        foreach (var item in _StatusDisplays)
        {
            item.SetActive(false);
        }
        // Show only status of current factory
        Status currentFactoryStatus = PlayerManager.CurrentFactoryDatabase.Status;
        // If the factory is not broken yet, display status
        if (PlayerManager.CurrentFactoryDatabase.Condition > 0)
        {
            _StatusDisplays[(int)currentFactoryStatus].SetActive(true);
        }
        // else, display as broken
        else
        {
            _StatusDisplays[2].SetActive(true);
        }
        // Change behavior depending on status
        switch (currentFactoryStatus)
        {
            case Status.IDLE:
                _BreedingGenDisplay.text = "";
                _GaugeRenderer.fillAmount = 0;
                break;
            // Display breeding progress number only if the status is breeding
            case Status.BREEDING:
                _BreedingGenDisplay.text = "GEN: " + PlayerManager.CurrentFactoryDatabase.BreedGen + "/" + PlayerManager.CurrentFactoryDatabase.BreedPref.BreedGen;
                _GaugeRenderer.fillAmount = PlayerManager.CurrentFactoryDatabase.BreedGauge / 100;
                break;
            default:
                break;
        }
    }
}
