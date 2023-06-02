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
public class FarmManager : FarmMngFunc
{
    public static FarmManager Instance;

    // [SerializeField] private FarmSO[] _FarmsData;
    // public FarmSO[] FarmsData => _FarmsData;

    // Current sprite renderer
    [SerializeField] private SpriteRenderer _BGRenderer;
    [SerializeField] private Image _GaugeRenderer;

    // Transforms indicating display borders of the farm
    [SerializeField] Transform[] border;
    // Transforms storing generated mechs in each farm
    [SerializeField] Transform holder;
    // Mech prefab
    [SerializeField] GameObject preset;

    // Panels in farm: Fitness, Breed, ChromoList
    [SerializeField] GameObject[] _Panels;

    [SerializeField] GameObject[] statusDisplays;
    [SerializeField] TextMeshProUGUI breedingGenDisplay;

    // List storing every mech gameObjects for easy access
    public List<GameObject> mechs;

    private void Awake()
    {
        FarmSO.OnFarmChangeStatus += OnChangeStatus;
        // Init stuffs
        if (Instance == null) Instance = this;

        mechs = new List<GameObject>();
        // Spawn mechs in the farm up to 20 mechs
        if(PlayerManager.CurrentFarmDatabase.MechChromos.Count > 0)
        {
            List<int> spawnIndex = new List<int>();
            for (int i = 0; i < MathF.Min(PlayerManager.CurrentFarmDatabase.MechChromos.Count, 20); i++)
            {
                int r = -1;
                do
                {
                    r = UnityEngine.Random.Range(0, PlayerManager.CurrentFarmDatabase.MechChromos.Count);
                }
                while (spawnIndex.Contains(r));
                spawnIndex.Add(r);
                AddMech(PlayerManager.CurrentFarmDatabase.MechChromos[r]);
            }
        }
        _BGRenderer.sprite = PlayerManager.CurrentFarmDatabase.BG;
        _GaugeRenderer.fillAmount = PlayerManager.CurrentFarmDatabase.BreedGauge / 100;

        OpenPanel(0);
        OnChangeStatus(PlayerManager.CurrentFarmDatabase, PlayerManager.CurrentFarmDatabase.Status);
        OpenPanel(1);
    }

    private void OnDestroy()
    {
        FarmSO.OnFarmChangeStatus -= OnChangeStatus;
    }

    private void OnChangeStatus(FarmSO f, Status s)
    {
        Debug.Log("INVOKED FROM FARMMNG");
        if (f == PlayerManager.CurrentFarmDatabase)
        {
            foreach (var item in statusDisplays) item.SetActive(false);
            if (f.Condition > 0) statusDisplays[(int)s].SetActive(true);
            else statusDisplays[2].SetActive(true);
            // Change behavior depending on status
            switch (s)
            {
                case Status.IDLE:
                    // Activate interactables
                    breedingGenDisplay.text = "";
                    break;
                case Status.BREEDING:
                    // Deactivate interactables
                    breedingGenDisplay.text = "GEN: " + PlayerManager.CurrentFarmDatabase.BreedGen + "/" +
                        PlayerManager.CurrentFarmDatabase.BreedPref.BreedGen;
                    break;
                default:
                    break;
            }
        }
    }

    /*
     * Open/change farm menu tab
     * 
     * Input
     *      i: tab index
     *      - Fitness Menu
     *      - Breeding Menu
     *      - Mech List
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
    private void AddMech(MechChromo c)
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
    private void DelMech(MechChromo c)
    {
        GameObject m = mechs.Find(x => x.GetComponent<MechDisplay>().MySO == c);
        mechs.Remove(m);
        Destroy(m);
    }

}
