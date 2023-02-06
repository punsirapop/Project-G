using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

/* 
 * Scriptable object for a chromosome
 * - Cosmetics: Head, Body, Accessory
 * - Combat: Atk, Def, Hp, Spd
 * ***
 * Randomized when generated but can be adjusted later
 */
[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/Stat")]
public class ChromosomeSC : ScriptableObject
{
    public static int IDCounter = 0;
    public int ID;
    // ---- Cosmetic ----
    // Head - 20 pcs
    [SerializeField] private int head;
    public int Head => head;
    // Color of the body - RGB
    [SerializeField] private int[] body;
    public int[] Body => body;
    // Accessory - 10 pcs
    [SerializeField] private int acc;
    public int Acc => acc;

    // ---- Combat ----
    // max stat to generate
    [SerializeField] private int cap = 4;
    public int Cap => cap;

    [SerializeField] private int[] atk;
    public int[] Atk => atk;
    [SerializeField] private int[] def;
    public int[] Def => def;
    [SerializeField] private int[] hp;
    public int[] Hp => hp;
    [SerializeField] private int[] spd;
    public int[] Spd => spd;

    private void Awake()
    {
        // set id
        ID = IDCounter;
        IDCounter++;

        // init stuffs
        head = Random.Range(0, 20);
        body = new int[3];
        for (int i = 0; i < 3; i++)  body[i] = Random.Range(0, 256);
        acc = Random.Range(0, 10);

        atk = new int[3];
        def = new int[3];
        hp = new int[3];
        spd = new int[3];
        for (int i = 0; i < atk.Length; i++) atk[i] = Random.Range(1, cap);
        for (int i = 0; i < def.Length; i++) def[i] = Random.Range(1, cap);
        for (int i = 0; i < hp.Length; i++) hp[i] = Random.Range(1, cap);
        for (int i = 0; i < spd.Length; i++) spd[i] = Random.Range(1, cap);
    }

    // Set properties according to encoded chromosome
    public void SetChromosome(List<int> encoded)
    {
        if(encoded != null)
        {
            this.head = encoded[0];
            for (int i = 0; i < 3; i++) this.body[i] = encoded[1 + i];
            this.acc = encoded[4];
            for (int i = 0; i < 3; i++) this.atk[i] = encoded[5 + i];
            for (int i = 0; i < 3; i++) this.def[i] = encoded[8 + i];
            for (int i = 0; i < 3; i++) this.hp[i] = encoded[11 + i];
            for (int i = 0; i < 3; i++) this.spd[i] = encoded[14 + i];
        }
    }

    // Encode properties into chromosome
    public List<int> GetChromosome()
    {
        List<int> c = new List<int>();

        c.Add(head);
        foreach (int item in body)
        {
            c.Add(item);
        }
        c.Add(acc);

        foreach (int item in atk)
        {
            c.Add(item);
        }
        foreach (int item in def)
        {
            c.Add(item);
        }
        foreach (int item in hp)
        {
            c.Add(item);
        }
        foreach (int item in spd)
        {
            c.Add(item);
        }

        return c;
    }

    // Get properties' limit
    // Might move to another file
    public List<int> GetMutateCap()
    {
        List<int> c = new List<int>();

        c.Add(20);
        for (int i = 0; i < 3; i++)
        {
            c.Add(256);
        }
        c.Add(10);

        for (int i = 0; i < 12; i++)
        {
            c.Add(cap);
        }

        return c;
    }
}
