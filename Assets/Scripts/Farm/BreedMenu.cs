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
    [SerializeField] TextMeshProUGUI CurrentPopDisplay;
    [SerializeField] TextMeshProUGUI CurrentGenDisplay;

    // Random-Tournament-Roulette-Rank
    [SerializeField] TMP_Dropdown TypeParentSelect;
    // For tournament
    [SerializeField] TextMeshProUGUI KTitle;
    [SerializeField] TextMeshProUGUI KDisplay;
    [SerializeField] Slider KSelect;
    [SerializeField] GameObject KHolder;
    // Single-Two-Uniform
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
    // R-G-B-W-B
    [SerializeField] TMP_Dropdown Body;
    [SerializeField] Slider AccSelect;
    [SerializeField] TextMeshProUGUI AccDisplay;
    // Atk-Def-Hp-Spd
    [SerializeField] TMP_Dropdown Combat;
    #endregion
    // List storing elite chromosomes
    FarmSO myFarm;
    List<MechChromoSO> elites;
    int breedPrice = 500;

    private void Awake()
    {
        elites = new List<MechChromoSO>();
        ChromosomeController.OnSelectChromo += EliteMe;
    }

    private void OnDestroy()
    {
        ChromosomeController.OnSelectChromo -= EliteMe;
    }

    private void Update()
    {
        myFarm = FarmManager.Instance.FarmsData[PlayerManager.CurrentFarm];
        // Update displays
        CurrentPopDisplay.text = myFarm.MechChromos.Count().ToString();
        CurrentGenDisplay.text = myFarm.Generation.ToString();
        KTitle.gameObject.SetActive(TypeParentSelect.value == 1);
        KHolder.gameObject.SetActive(TypeParentSelect.value == 1);
        KDisplay.text = KSelect.value.ToString();
        KSelect.minValue = 1;
        KSelect.maxValue = myFarm.MechChromos.Count();
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
        BreedBtn.interactable = myFarm.MechChromos.Count() > 0 &&
            (myFarm.MechChromos.Count() - elites.Count) % 2 == 0;
        // Yep, all of this
    }

    // Elite/unelite clicked chromosome
    private void EliteMe(MechChromoSO c)
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
     * Get current fitness preferences
     * 
     * Output
     *      list of preferred values (default = -1)
     */
    private List<int> CurrentPref()
    {
        List<int> list = new List<int>();

        list.Add(EnableMe[0].isOn ? (int)HeadSelect.value : -1);
        list.Add(EnableMe[1].isOn ? Body.value : -1);
        list.Add(EnableMe[2].isOn ? (int)AccSelect.value : -1);
        list.Add(EnableMe[3].isOn ? Combat.value : -1);

        return list;
    }

    /*
     * Initiate breeding process
     * 
     * Get Fitness
     * Select Parents
     * Crossover
     * Mutate
     */
    public void BreedMe()
    {
        for (int g = 0; g < GenerationSelect.value; g++)
        {
            List<MechChromoSO> candidates = myFarm.MechChromos;

            // ------- get fitness -------
            Dictionary<MechChromoSO, float> fv = new Dictionary<MechChromoSO, float>();
            foreach (MechChromoSO c in candidates)
            {
                fv.Add(c, c.GetFitness(CurrentPref()));
            }
            // ------- select parents according to chosen type -------
            List<MechChromoSO> parents = new List<MechChromoSO>
                (GeneticFunc.Instance.SelectParent(fv, elites.Count, TypeParentSelect.value, (int)KSelect.value));
            Debug.Log("Parents Count: " + parents.Count);

            // ------- crossover according to chosen type -------
            List<List<int>> parentsEncoded = new List<List<int>>();
            // encode dem parents and add to list
            foreach (MechChromoSO c in parents)
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
            List<MechChromoSO> deleteMe = new List<MechChromoSO>(myFarm.MechChromos);
            // keep those elites to the next generation
            foreach (var item in elites)
            {
                deleteMe.Remove(item);
            }
            // clear old population
            foreach (var item in deleteMe)
            {
                FarmManager.Instance.DelChromo(item);
            }
            Debug.Log("Parents Count: " + myFarm.MechChromos.Count);

            // ------- create new chromosomes -------
            List<MechChromoSO> children = new List<MechChromoSO>();
            foreach (var item in parentsEncoded)
            {
                FarmManager.Instance.AddChromo();
                children.Add(myFarm.MechChromos[myFarm.MechChromos.Count - 1]);
                children[children.Count - 1].SetChromosome(item);
            }
                
            // Debug.Log("Children Count: " + children.Count);
        }

        myFarm.AddGen((int)GenerationSelect.value);
        elites.Clear();
    }
}
