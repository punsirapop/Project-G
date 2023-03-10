using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
/*
 * Control over farms and their panels
 */
public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance;

    public static event Action OnEditChromo;

    [SerializeField] private FarmSO[] _FarmsData;
    public FarmSO[] FarmsData => _FarmsData;

    // Current sprite renderer
    [SerializeField] private SpriteRenderer _BGRenderer;

    // Transforms indicating display borders of the farm
    [SerializeField] Transform[] border;
    // Transforms storing generated mechs in each farm
    [SerializeField] Transform holder;
    // Mech prefab
    [SerializeField] GameObject preset;
    // Game panels
    // BreedMenu, FitnessMenu, ChromoMenu, ChromoDetail

    #region Panels
    // Panels in factory: Info, Produce, ChromoMenu
    [SerializeField] private Button[] _PanelButtons;
    [SerializeField] private GameObject[] _Panels;
    #endregion

    // List storing every mech gameObjects for easy access
    List<GameObject> mechs;

    private void Awake()
    {
        if(Instance == null) Instance = this;

        mechs = new List<GameObject>();
        foreach (var item in FarmsData[PlayerManager.CurrentFarm].MechChromos)
        {
            AddMech(item);
        }
        _RenderSprite();
    }

    // Render the weapon holder sprite for each factory
    private void _RenderSprite()
    {
        _BGRenderer.sprite = _FarmsData[PlayerManager.CurrentFarm].BG;
    }

    /*
     * Open/change farm menu tab
     * 
     * Input
     *      i: tab index
     *      - Breeding Menu
     *      - Fitness Menu
     *      - Mech List
     *      - Mech Details
     */
    public void OpenPanel(int i)
    {
        foreach (var item in _Panels)
        {
            item.SetActive(false);
        }
        _Panels[i].SetActive(true);
    }

    /*
     * Instantiate new mech to the farm in randomized position
     * 
     * Input
     *      c: chromosome scriptable object
     */

    // Add new random chromosome to the current space
    public void AddChromo()
    {
        MechChromoSO c = (MechChromoSO)ScriptableObject.CreateInstance("MechChromoSO");
        _FarmsData[PlayerManager.CurrentFarm].AddChromo(c);
        AddMech(c);
        OnEditChromo?.Invoke();
    }

    // Delete a chromosome from the current space
    public void DelChromo(MechChromoSO c)
    {
        _FarmsData[PlayerManager.CurrentFarm].DelChromo(c);
        DelMech(c);
        OnEditChromo?.Invoke();
    }

    private void AddMech(MechChromoSO c)
    {
        Vector2 spawnPoint = new Vector2(UnityEngine.Random.Range(border[0].position.x, border[1].position.x),
            UnityEngine.Random.Range(border[0].position.y, border[1].position.y));
        GameObject mech = Instantiate(preset, spawnPoint, Quaternion.identity, holder);
        mechs.Add(mech);
        mech.GetComponent<MechDisplay>().SetChromo(c);
        mech.SetActive(true);
    }

    /*
     * Search and delete a mech from a farm
     * 
     * Input
     *      c: chromosome scriptable object
     */
    private void DelMech(MechChromoSO c)
    {
        GameObject m = mechs.Find(x => x.GetComponent<MechDisplay>().MySO == c);
        mechs.Remove(m);
        Destroy(m);
    }

}
