using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Control over farms and panels
 */
public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance;

    [SerializeField] Transform[] border;
    [SerializeField] GameObject preset;
    [SerializeField] GameObject[] panels;
    [SerializeField] Transform[] holders;

    List<GameObject> mechs;

    int currentGen;
    public int CurrentGen => currentGen;

    private void Awake()
    {
        if(Instance == null) Instance = this;

        mechs = new List<GameObject>();
        currentGen = 0;

        PlayerManager.OnAddChromosome += AddMech;
        PlayerManager.OnRemoveChromosome += DelMech;
    }

    private void OnDestroy()
    {
        PlayerManager.OnAddChromosome -= AddMech;
        PlayerManager.OnRemoveChromosome -= DelMech;
    }

    private void AddMech(ChromosomeSC c)
    {
        Vector2 spawnPoint = new Vector2(UnityEngine.Random.Range(border[0].position.x, border[1].position.x),
            UnityEngine.Random.Range(border[0].position.y, border[1].position.y));
        GameObject mech = Instantiate(preset, spawnPoint, Quaternion.identity, holders[PlayerManager.CurrentFarm - 1]);
        mechs.Add(mech);
        mech.GetComponent<ChromosomeController>().mySC = c;
        mech.SetActive(true);
    }

    private void DelMech(ChromosomeSC c)
    {
        GameObject m = mechs.Find(x => x.GetComponent<ChromosomeController>().mySC == c);
        mechs.Remove(m);
        Destroy(m);
    }

    public void OpenPanel(int i)
    {
        foreach (var item in panels)
        {
            item.SetActive(false);
        }
        panels[i].SetActive(true);
    }

    public void IncreaseGen(int g)
    {
        currentGen += g;
    }
}
