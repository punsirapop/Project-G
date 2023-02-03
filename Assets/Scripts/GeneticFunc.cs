using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Store functions related to genetic algorithm
 * - SelectParent
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

    /* Create new list of parents
     */
    public List<ChromosomeSC> SelectParent(List<ChromosomeSC> candidates, int eliteCount)
    {
        List<ChromosomeSC> result = new List<ChromosomeSC>();
        while ( result.Count < candidates.Count - eliteCount - (candidates.Count - eliteCount) % 2 )
        {
            int r = Random.Range(0, candidates.Count);
            result.Add(candidates[r]);
        }
        return result;
    }

    /* Crossover 2 lists
     * 0 - one-point
     * 1 - two-point
     * 2 - uniform
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

    /* Randomly mutate gene
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
}
