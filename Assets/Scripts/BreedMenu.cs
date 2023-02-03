using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreedMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CurrentPop;
    [SerializeField] TextMeshProUGUI CurrentGen;

    [SerializeField] TMP_Dropdown TypeParentSelect;
    [SerializeField] TMP_Dropdown TypeCrossover;
    [SerializeField] Slider GenerationSelect;
    [SerializeField] TextMeshProUGUI GenerationDisplay;
    [SerializeField] TextMeshProUGUI Elitism;
    [SerializeField] Slider MutationSelect;
    [SerializeField] TextMeshProUGUI MutationDisplay;

    [SerializeField] TextMeshProUGUI TotalPrice;

    // Head/Body/Acc/Combat
    [SerializeField] Toggle[] EnableMe;
    [SerializeField] GameObject[] Titles;
    [SerializeField] GameObject[] Holders;

    [SerializeField] Slider HeadSelect;
    [SerializeField] TextMeshProUGUI HeadDisplay;
    [SerializeField] TMP_Dropdown Body;
    [SerializeField] Slider AccSelect;
    [SerializeField] TextMeshProUGUI AccDisplay;
    [SerializeField] TMP_Dropdown Combat;

    [SerializeField] Button BreedBtn;

    List<ChromosomeSC> elites;

    private void Awake()
    {
        elites = new List<ChromosomeSC>();
        ChromosomeController.OnSelectChromo += EliteMe;
    }

    private void OnDestroy()
    {
        ChromosomeController.OnSelectChromo -= EliteMe;
    }

    private void Update()
    {
        CurrentPop.text = PlayerManager.Chromosomes[PlayerManager.CurrentFarm].Count().ToString();
        CurrentGen.text = FarmManager.Instance.CurrentGen.ToString();
        GenerationDisplay.text = GenerationSelect.value.ToString();
        MutationDisplay.text = MutationSelect.value.ToString();
        Elitism.text = (elites.Count > 0) ? string.Join(", ", elites.Select(x => x.ID)) : "None";
        TotalPrice.text = GenerationSelect.value * 500 + "G";

        for (int i = 0; i < EnableMe.Length; i++)
        {
            Titles[i].SetActive(EnableMe[i].isOn);
            Holders[i].SetActive(EnableMe[i].isOn);
        }

        HeadDisplay.text = HeadSelect.value.ToString();
        AccDisplay.text = AccSelect.value.ToString();
        BreedBtn.interactable = (PlayerManager.Chromosomes[PlayerManager.CurrentFarm].Count() - elites.Count) % 2 == 0;
    }

    private void EliteMe(ChromosomeSC c)
    {
        if (!elites.Contains(c))
        {
            elites.Add(c);
        }
        else
        {
            elites.Remove(c);
        }
    }

    /*
     * Get Fitness
     * Select Parents
     * Crossover
     * Mutate
     */
    public void BreedMe()
    {
        for (int g = 0; g < GenerationSelect.value; g++)
        {
            // get fitness here
            // select parents according to chosen type
            List<ChromosomeSC> parents = new List<ChromosomeSC>
                (GeneticFunc.Instance.SelectParent(PlayerManager.Chromosomes[PlayerManager.CurrentFarm], elites.Count));
            Debug.Log("Parents Count: " + parents.Count);

            // crossover according to chosen type
            List<List<int>> parentsEncoded = new List<List<int>>();
            foreach (ChromosomeSC c in parents)
            {
                parentsEncoded.Add(c.GetChromosome());
                // Debug.Log(string.Join("-", parentsEncoded[parentsEncoded.Count-1]));
            }
            for (int i = 0; i < parentsEncoded.Count - (parentsEncoded.Count % 2); i+=2)
            {
                GeneticFunc.Instance.Crossover(parentsEncoded[i], parentsEncoded[i+1], TypeCrossover.value);
                // Debug.Log(string.Join("-", parentsEncoded[i]));
            }
            Debug.Log("Finished CrossingOver " + parentsEncoded.Count);

            // mutate
            for (int i = 0; i < parents.Count; i++)
            {
                if (Random.Range(0, 100) < MutationSelect.value)
                    GeneticFunc.Instance.Mutate(parentsEncoded[i], parents[i].GetMutateCap());
            }

            // clear farm
            List<ChromosomeSC> deleteMe = new List<ChromosomeSC>(PlayerManager.Chromosomes[PlayerManager.CurrentFarm]);
            foreach (var item in elites)
            {
                deleteMe.Remove(item);
            }
            foreach (var item in deleteMe)
            {
                PlayerManager.Instance.DelChromo(item);
            }
            Debug.Log("Parents Count: " + PlayerManager.Chromosomes[PlayerManager.CurrentFarm].Count);

            // create new chromosomes
            List<ChromosomeSC> children = new List<ChromosomeSC>();
            foreach (var item in parentsEncoded)
            {
                PlayerManager.Instance.AddChromo();
                children.Add(PlayerManager.Chromosomes[PlayerManager.CurrentFarm]
                    [PlayerManager.Chromosomes[PlayerManager.CurrentFarm].Count - 1]);
                children[children.Count - 1].SetChromosome(item);
            }

            // Debug.Log("Children Count: " + children.Count);
        
            FarmManager.Instance.IncreaseGen(1);
        }

        elites.Clear();
    }
}
