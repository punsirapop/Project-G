using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Control over farms and their panels
 */
public class FarmManager : PlayerManager
{
    public static FarmManager Instance;

    // Transforms indicating display borders of the farm
    [SerializeField] Transform[] border;
    // Transforms storing generated mechs in each farm
    [SerializeField] Transform[] holders;
    // Mech prefab
    [SerializeField] GameObject preset;
    // Game panels
    // BreedMenu, FitnessMenu, ChromoMenu, ChromoDetail
    [SerializeField] GameObject[] panels;

    // List storing every mech gameObjects for easy access
    List<GameObject> mechs;

    private void Awake()
    {
        if(Instance == null) Instance = this;

        mechs = new List<GameObject>();

        PlayerManager.OnAddChromosome += AddMech;
        PlayerManager.OnRemoveChromosome += DelMech;
    }

    private void OnDestroy()
    {
        PlayerManager.OnAddChromosome -= AddMech;
        PlayerManager.OnRemoveChromosome -= DelMech;
    }

    /*
     * Instantiate new mech to the farm in randomized position
     * 
     * Input
     *      c: chromosome scriptable object
     */
    private void AddMech(ChromosomeSO c)
    {
        Vector2 spawnPoint = new Vector2(UnityEngine.Random.Range(border[0].position.x, border[1].position.x),
            UnityEngine.Random.Range(border[0].position.y, border[1].position.y));
        GameObject mech = Instantiate(preset, spawnPoint, Quaternion.identity, holders[PlayerManager.CurrentPlace - 1]);
        mechs.Add(mech);
        mech.GetComponent<ChromosomeController>().MySC = c;
        mech.SetActive(true);
    }

    /*
     * Search and delete a mech from a farm
     * 
     * Input
     *      c: chromosome scriptable object
     */
    private void DelMech(ChromosomeSO c)
    {
        GameObject m = mechs.Find(x => x.GetComponent<ChromosomeController>().MySC == c);
        mechs.Remove(m);
        Destroy(m);
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
        foreach (var item in panels)
        {
            item.SetActive(false);
        }
        panels[i].SetActive(true);
    }
}
