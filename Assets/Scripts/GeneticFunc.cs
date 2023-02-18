using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Store functions related to genetic algorithm
 * - SelectParent *Under development*
 * - Crossover
 * - Mutate
 */
public class GeneticFunc : MonoBehaviour
{
    public static GeneticFunc Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // -------------- General --------------

    /* 
     * Create lists of parents
     * 
     * Input
     *      popCount: population size
     *      eliteCount: amount of elites
     *      
     * Output
     *      list of parents' indexes
     */
    public List<int> SelectParentU(int popCount, int eliteCount)
    {
        List<int> result = new List<int>();
        while ( result.Count < popCount - eliteCount - (popCount - eliteCount) % 2 )
        {
            int r = Random.Range(0, popCount);
            result.Add(r);
        }
        return result;
    }

    /* 
     * Crossover 2 lists
     * 
     * Input
     *      a & b: lists to be crossed over
     *      type: crossover type
     *          0 - one-point
     *          1 - two-point
     *          2 - uniform
     */
    public void Crossover(List<int> a, List<int> b, int type)
    {
        // store temp data
        List<int> temp = new List<int>(b);

        if (type < 2)
        {   // not uniform
            Debug.Log("n-point crossover");
            // set start & end points
            int start = Random.Range(0, a.Count);
            int end = (type == 1) ? Random.Range(start, a.Count) : a.Count - 1;
            Debug.Log("Start: " + start + " End: " + end);
            // swap intervals
            for (int i = start; i <= end; i++)
            {
                b[i] = a[i];
                a[i] = temp[i];
            }
        }
        else
        {   // uniform
            Debug.Log("uniform crossover");
            for (int i = 0; i < a.Count; i++)
            {
                if (Random.Range(0, 100) >= 50)
                {
                    b[i] = a[i];
                    a[i] = temp[i];
                }
            }
        }
        Debug.Log("finished");
    }

    /* 
     * Randomly mutate genes
     * 
     * Input
     *      c: encoded chromosome to be mutated
     *      statCap: list of maximum number for each gene
     */
    public void Mutate(List<int> c, List<int> statCap)
    {
        Debug.Log("IM MUTATING");

        for (int i = 0; i < c.Count; i++)
        {
            if(Random.Range(0, 100) < 100 / c.Count)
            {
                c[i] = Random.Range(0, statCap[i]);
                Debug.Log("I MUTATED AT " + i);
            }
        }
    }

    // -------------- Under Construction --------------
    /*
     * Create list of parents
     * 
     * Input
     *      fv: population fitness val
     *      eliteCount: amount of elites
     *      mode: selection mode
     *          0 - Random
     *          1 - Tournament-based
     *          2 - Roulette Wheel
     *          3 - Rank-based
     *      
     * Output
     *      list of parents' indexes
     */
    public List<ChromosomeSO> SelectParent(Dictionary<ChromosomeSO, float> fv, int eliteCount, int mode, int k)
    {
        List<ChromosomeSO> result = new List<ChromosomeSO>();
        while (result.Count < fv.Count - eliteCount - (fv.Count - eliteCount) % 2)
        {
            switch (mode)
            {
                // random
                case 0:
                    int r1 = Random.Range(0, fv.Count);
                    result.Add(fv.ElementAt(r1).Key);
                    break;
                // tournament-based
                case 1:
                    Dictionary<ChromosomeSO, float> tmp1 = new Dictionary<ChromosomeSO, float>();
                    for (int i = 0; i < k; i++)
                    {
                        int r2 = 0;
                        do r2 = Random.Range(0, fv.Count);
                        while (tmp1.ContainsKey(fv.ElementAt(r2).Key));
                        tmp1.Add(fv.ElementAt(r2).Key, fv.ElementAt(r2).Value);
                    }
                    result.Add(tmp1.OrderBy(x => x.Value).First().Key);
                    break;
                // roulette
                case 2:
                    float r3 = Random.Range(0, fv.Values.Sum());
                    int index = 0;
                    float u = fv.First().Value;
                    while(u < r3)
                    {
                        index++;
                        u += fv.ElementAt(index).Value;
                    }
                    result.Add(fv.ElementAt(index).Key);
                    break;
                // rank-based
                case 3:
                    Dictionary<ChromosomeSO, float> tmp2 = new Dictionary<ChromosomeSO, float>
                        (fv.OrderBy(x => x.Value));
                    for (int i = 0; i < tmp2.Count; i++)
                    {
                        tmp2[tmp2.ElementAt(i).Key] = i;
                    }
                    float r4 = Random.Range(0, fv.Values.Sum());
                    int index2 = 0;
                    float u2 = fv.First().Value;
                    while (u2 < r4)
                    {
                        index2++;
                        u2 += fv.ElementAt(index2).Value;
                    }
                    result.Add(fv.ElementAt(index2).Key);
                    break;
            }

        }

        return result;
    }
}
