using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FitnessMenu;

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
    [SerializeField] Slider ElitismSelect;
    [SerializeField] TextMeshProUGUI ElitismDisplay;
    [SerializeField] Slider MutationSelect;
    [SerializeField] TextMeshProUGUI MutationDisplay;

    [SerializeField] TextMeshProUGUI GenerationDisplay;
    [SerializeField] TextMeshProUGUI TotalPrice;
    [SerializeField] Button[] GenerationAdjustors;
    [SerializeField] Button BreedBtn;
    #endregion
    [SerializeField] FitnessMenu fitnessMenu;
    
    // List storing elite chromosomes
    FarmSO myFarm => PlayerManager.CurrentFarmDatabase;
    // List<MechChromoSO> elites;
    int breedGen;
    int breedPrice = 500;

    private void Awake()
    {
        FarmSO.OnFarmChangeStatus += OnChangeStatus;
        // elites = new List<MechChromoSO>();
        breedGen = 1;
        OnChangeStatus(myFarm, myFarm.Status);
    }

    private void OnDisable()
    {
        FarmSO.OnFarmChangeStatus -= OnChangeStatus;
    }

    private void Update()
    {
        // Update displays
        CurrentPopDisplay.text = myFarm.MechChromos.Count().ToString();
        CurrentGenDisplay.text = myFarm.Generation.ToString();
        KTitle.gameObject.SetActive(TypeParentSelect.value == 1);
        KHolder.gameObject.SetActive(TypeParentSelect.value == 1);
        KDisplay.text = KSelect.value.ToString();
        KSelect.minValue = 1;
        KSelect.maxValue = myFarm.MechChromos.Count() / 2;
        GenerationDisplay.text = breedGen.ToString();
        MutationDisplay.text = MutationSelect.value.ToString();
        ElitismDisplay.text = ElitismSelect.value.ToString();
        TotalPrice.text = breedGen * breedPrice + "G";
        BreedBtn.interactable = (myFarm.Status == Status.IDLE) ? myFarm.MechChromos.Count() > 1 : false;
        if (myFarm.Status != Status.BREEDING)
        {
            GenerationAdjustors[0].interactable = breedGen < 10;
            GenerationAdjustors[1].interactable = breedGen > 1;
        }
    }

    private void OnChangeStatus(FarmSO f, Status s)
    {
        if (f == myFarm)
        {
            // Change behavior depending on status
            switch (s)
            {
                case Status.IDLE:
                    // Activate interactables
                    TypeParentSelect.interactable = true;
                    TypeCrossover.interactable = true;
                    KSelect.interactable = true;
                    ElitismSelect.interactable = true;
                    MutationSelect.interactable = true;
                    // foreach (var item in GenerationAdjustors) item.interactable = true;
                    // BreedBtn.interactable = true;
                    break;
                case Status.BREEDING:
                    // Deactivate interactables
                    TypeParentSelect.interactable = false;
                    TypeCrossover.interactable = false;
                    KSelect.interactable = false;
                    ElitismSelect.interactable = false;
                    MutationSelect.interactable = false;
                    foreach (var item in GenerationAdjustors) item.interactable = false;
                    // BreedBtn.interactable = false;
                    SetDisplay();
                    break;
                default:
                    break;
            }
        }
    }

    private void SetDisplay()
    {
        TypeParentSelect.value = myFarm.BreedInfo.TypeParentSelect;
        TypeCrossover.value = myFarm.BreedInfo.TypeCrossover;
        KSelect.value = myFarm.BreedInfo.KSelect;
        ElitismSelect.value = myFarm.BreedInfo.ElitismRate;
        MutationSelect.value = myFarm.BreedInfo.MutationRate;
        breedGen = myFarm.BreedInfo.BreedGen;
    }

    public void AdjustGen(int i)
    {
        breedGen += i;
    }

    // Change farm status to breeding
    public void SetBreedRequest()
    {
        Dictionary<dynamic, float> fv = fitnessMenu.GetFitnessDict();
        BreedInfo breedInfo = new BreedInfo(myFarm, breedGen, fitnessMenu.CurrentPref(), (int)ElitismSelect.value,
            TypeParentSelect.value, (int)KSelect.value, TypeCrossover.value, (int)MutationSelect.value);
        myFarm.SetBreedRequest(breedInfo);
        Debug.Log("Setting breed req for " + breedInfo.MyFarm.Name);
        myFarm.SetStatus(Status.BREEDING);
    }

    [Serializable] public struct BreedInfo
    {
        FarmSO myFarm;
        public FarmSO MyFarm => myFarm;
        int breedGen;
        public int BreedGen => breedGen;
        List<Tuple<Properties, int>> currentPref;
        public List<Tuple<Properties, int>> CurrentPref => currentPref;
        int elitismRate;
        public int ElitismRate => elitismRate;
        int typeParentSelect;
        public int TypeParentSelect => typeParentSelect;
        int kSelect;
        public int KSelect => kSelect;
        int typeCrossover;
        public int TypeCrossover => typeCrossover;
        int mutationRate;
        public int MutationRate => mutationRate;

        public BreedInfo (FarmSO MyFarm, int BreedGen, List<Tuple<Properties, int>> CurrentPref, int ElitismRate,
            int TypeParentSelect, int KSelect, int TypeCrossover, int MutationRate)
        {
            myFarm = MyFarm;
            breedGen = BreedGen;
            currentPref = CurrentPref;
            elitismRate = ElitismRate;
            typeParentSelect = TypeParentSelect;
            kSelect = KSelect;
            typeCrossover = TypeCrossover;
            mutationRate = MutationRate;
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
            List<MechChromoSO> elites = new List<MechChromoSO>();

            // ------- get fitness -------
            Dictionary<dynamic, float> fv = new Dictionary<dynamic, float>();
            foreach (MechChromoSO c in myFarm.MechChromos)
            {
                fv.Add(c, c.GetFitness(currentPref));
            }

            // ------- get elites -------
            for (int i = 0; i < Mathf.RoundToInt(fv.Count * elitismRate / 100); i++)
            {
                elites.Add(fv.ElementAt(i).Key);
            }
            if ((fv.Count - elites.Count) % 2 != 0)
            {
                if(elites.Count > 0) elites.Remove(elites.Last());
                else elites.Add(fv.ElementAt(0).Key);
            }

            // ------- select parents according to chosen type -------
            List<dynamic> parents = new List<dynamic>
                (GeneticFunc.Instance.SelectParent(fv, elites.Count, typeParentSelect, kSelect));
            // Debug.Log("Parents Count: " + parents.Count);

            // ------- crossover according to chosen type -------
            List<List<List<int>>> parentsEncoded = new List<List<List<int>>>();
            // encode dem parents and add to list
            foreach (MechChromoSO c in parents)
            {
                parentsEncoded.Add(c.GetChromosome());
                // Debug.Log(string.Join("-", parentsEncoded[parentsEncoded.Count-1]));
            }
            // crossover each pair ex: 0-1, 2-3, ...
            for (int i = 0; i < parentsEncoded.Count - (parentsEncoded.Count % 2); i += 2)
            {
                GeneticFunc.Instance.Crossover(parentsEncoded[i], parentsEncoded[i + 1], typeCrossover);
                // Debug.Log(string.Join("-", parentsEncoded[i]));
            }
            // Debug.Log("Finished CrossingOver " + parentsEncoded.Count);

            // ------- mutate -------
            for (int i = 0; i < parents.Count; i++)
            {
                if (UnityEngine.Random.Range(0, 100) < mutationRate)
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
                FarmManager.Instance.DelChromo(MyFarm, item);
            }
            // Debug.Log("Parents Count: " + myFarm.MechChromos.Count);

            // ------- create new chromosomes -------
            List<MechChromoSO> children = new List<MechChromoSO>();
            foreach (var item in parentsEncoded)
            {
                FarmManager.Instance.AddChromo(MyFarm);
                children.Add(myFarm.MechChromos.Last());
                children.Last().SetChromosome(item);
                // FarmManager.Instance.mechs.Last().GetComponent<MechDisplay>().SetChromo(children.Last());
            }

            // Debug.Log("Children Count: " + children.Count);
        }
    }
}
