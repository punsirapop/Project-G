using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Debugger
 * Nothing special
 */
public class Debugger : MonoBehaviour
{
    [SerializeField] int mode;
    [SerializeField] List<int> a;
    [SerializeField] List<int> b;
    // Start is called before the first frame update
    void Start()
    {
        // GeneticFunc.Instance.Crossover(a, b, mode);
    }
}
