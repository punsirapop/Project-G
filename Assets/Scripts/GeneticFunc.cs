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
    public List<int> SelectParent(int popCount, int eliteCount)
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
    public List<int> SelectParentU(List<float> fv, int eliteCount, int mode)
    {
        List<int> result = new List<int>();

        switch (mode)
        {
            // random
            case 0:
                while (result.Count < fv.Count - eliteCount - (fv.Count - eliteCount) % 2)
                {
                    int r = Random.Range(0, fv.Count);
                    result.Add(r);
                }
                break;
            // tournament-based
            case 1:
                break;
            // roulette
            case 2:
                break;
            // rank-based
            case 3:
                break;
        }

        return result;
    }
}
