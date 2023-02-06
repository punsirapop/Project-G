using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Control most of the breeding stuffs
 * From menu display to updating results
 */
public class BreedMenu : MonoBehaviour
{
    // Stuffs to display such as textboxes, sliders, dropdowns, buttons
    #region breeding tab
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

    [SerializeField] Button BreedBtn;
    #endregion
    #region fitness tab
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
    #endregion
    // List storing elite chromosomes
    List<ChromosomeSC> elites;
    int breedPrice = 500;

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
        // Update displays
        CurrentPop.text = PlayerManager.Chromosomes[PlayerManager.CurrentFarm].Count().ToString();
        CurrentGen.text = FarmManager.Instance.CurrentGen.ToString();
        GenerationDisplay.text = GenerationSelect.value.ToString();
        MutationDisplay.text = MutationSelect.value.ToString();
        Elitism.text = (elites.Count > 0) ? string.Join(", ", elites.Select(x => x.ID)) : "None";
        TotalPrice.text = GenerationSelect.value * breedPrice + "G";

        for (int i = 0; i < EnableMe.Length; i++)
        {
            Titles[i].SetActive(EnableMe[i].isOn);
            Holders[i].SetActive(EnableMe[i].isOn);
        }

        HeadDisplay.text = HeadSelect.value.ToString();
        AccDisplay.text = AccSelect.value.ToString();
        BreedBtn.interactable = (PlayerManager.Chromosomes[PlayerManager.CurrentFarm].Count() - elites.Count) % 2 == 0;
        // Yep, all of this
    }

    // Elite/unelite clicked chromosome
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
     * Initiate breeding process
     * 
     * Get Fitness <-- Under development
     * Select Parents
     * Crossover
     * Mutate
     */
    public void BreedMe()
    {
        for (int g = 0; g < GenerationSelect.value; g++)
        {
            List<ChromosomeSC> candidates = PlayerManager.Chromosomes[PlayerManager.CurrentFarm];
            List<ChromosomeSC> parents = new List<ChromosomeSC>();

            // ------- get fitness here -------
            // ------- select parents according to chosen type -------
            // get parents' indexes
            List<int> p = GeneticFunc.Instance.SelectParent(candidates.Count, elites.Count);
            // add them to the list
            for (int i = 0; i < candidates.Count - elites.Count; i++)
            {
                parents.Add(candidates[p[i]]);
            }
            Debug.Log("Parents Count: " + parents.Count);

            // ------- crossover according to chosen type -------
            List<List<int>> parentsEncoded = new List<List<int>>();
            // encode dem parents and add to list
            foreach (ChromosomeSC c in parents)
            {
                parentsEncoded.Add(c.GetChromosome());
                // Debug.Log(string.Join("-", parentsEncoded[parentsEncoded.Count-1]));
            }
            // crossover each pair ex: 0-1, 2-3, ...
            for (int i = 0; i < parentsEncoded.Count - (parentsEncoded.Count % 2); i+=2)
            {
                GeneticFunc.Instance.Crossover(parentsEncoded[i], parentsEncoded[i+1], TypeCrossover.value);
                // Debug.Log(string.Join("-", parentsEncoded[i]));
            }
            Debug.Log("Finished CrossingOver " + parentsEncoded.Count);

            // ------- mutate -------
            for (int i = 0; i < parents.Count; i++)
            {
                if (Random.Range(0, 100) < MutationSelect.value)
                    GeneticFunc.Instance.Mutate(parentsEncoded[i], parents[i].GetMutateCap());
            }

            // ------- clear farm -------
            List<ChromosomeSC> deleteMe = new List<ChromosomeSC>(PlayerManager.Chromosomes[PlayerManager.CurrentFarm]);
            // keep those elites to the next generation
            foreach (var item in elites)
            {
                deleteMe.Remove(item);
            }
            // clear old population
            foreach (var item in deleteMe)
            {
                PlayerManager.Instance.DelChromo(item);
            }
            Debug.Log("Parents Count: " + PlayerManager.Chromosomes[PlayerManager.CurrentFarm].Count);

            // ------- create new chromosomes -------
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
