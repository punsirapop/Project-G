// using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using static FitnessMenu;
using static Unity.Burst.Intrinsics.X86.Avx;

/* 
 * Scriptable object for a chromosome
 * - Cosmetics: Head, Body, Accessory
 * - Combat: Atk, Def, Hp, Spd
 * ***
 * Randomized when generated but can be adjusted later
 */
public class MechChromo
{
    public enum Ranks { C, B, A, S }
    public enum Elements { Fire, Plant, Water, Light, Dark, None }
    public int ID;
    // ---- Cosmetic ----
    // Head - 20 pcs
    public int Head { get; private set; }
    // Color of the body - RGB
    public int[] Body { get; private set; }
    // Accessory - 10 pcs
    public int Acc { get; private set; }

    // ---- Combat ----
    // max stat to generate

    public int[] Atk { get; private set; }
    public int[] Def { get; private set; }
    public int[] Hp { get; private set; }
    public int[] Spd { get; private set; }
    public Elements Element { get; private set; }
    public Ranks Rank { get; private set; }

    public MechChromo(MechSaver m)
    {
        if (m == null)
        {
            ID = PlayerManager.MechIDCounter;
            PlayerManager.MechIDCounter++;

            // init stuffs
            Head = Random.Range(0, 20);
            Body = new int[3];
            for (int i = 0; i < 3; i++) Body[i] = Random.Range(0, 256);
            Acc = Random.Range(0, 10);

            SetRandomStat(PlayerManager.MechCap);
        }
        else
        {
            ID = m.ID;
            Head = m.Head;
            Body = m.Body.ToArray();
            Acc = m.Acc;
            Atk = m.Atk.ToArray();
            Def = m.Def.ToArray();
            Hp = m.Hp.ToArray();
            Spd = m.Spd.ToArray();
        }
        SetRank();
        SetElement();
    }

    /*
    private void Awake()
    {
        SaveManager.OnReset += ResetMe;
        // set id
        ID = IDCounter;
        IDCounter++;

        // init stuffs
        Head = Random.Range(0, 20);
        Body = new int[3];
        for (int i = 0; i < 3; i++)  Body[i] = Random.Range(0, 256);
        Acc = Random.Range(0, 10);

        Cap = Cap == 0 ? 4 : Cap;

        SetRandomStat(Cap);
        SetRank();
        SetElement();
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= ResetMe;
    }
    */

    public void SetRandomStat(int c)
    {
        Atk = new int[3];
        Def = new int[3];
        Hp = new int[3];
        Spd = new int[3];
        for (int i = 0; i < Atk.Length; i++) Atk[i] = Random.Range(1, c+1);
        for (int i = 0; i < Def.Length; i++) Def[i] = Random.Range(1, c+1);
        for (int i = 0; i < Hp.Length; i++) Hp[i] = Random.Range(1, c+1);
        for (int i = 0; i < Spd.Length; i++) Spd[i] = Random.Range(1, c+1);
        SetRank();
        SetElement();
    }

    public void SetRandomStat2(int cap)
    {
        int[] stat = new int[4];
        int firstDis = Mathf.Max(1, Mathf.CeilToInt(cap / 12));
        cap -= firstDis * 4;
        for (int i = 0; i < 4; i++)
        {
            stat[i] += firstDis;
        }

        for (; cap > 0; cap--)
        {
            stat[Random.Range(0, 4)]++;
        }

        Atk = new int[] { stat[0] };
        Def = new int[] { stat[1] };
        Hp = new int[] { stat[2] };
        Spd = new int[] { stat[3] };
        SetRank();
        SetElement();
    }

    // Set properties according to encoded chromosome
    public void SetChromosome(List<List<int>> encodedHolder)
    {
        if(encodedHolder != null)
        {
            List<int> encoded = encodedHolder[0];
            this.Head = encoded[0];
            for (int i = 0; i < 3; i++) this.Body[i] = encoded[1 + i];
            this.Acc = encoded[4];
            for (int i = 0; i < 3; i++) this.Atk[i] = encoded[5 + i];
            for (int i = 0; i < 3; i++) this.Def[i] = encoded[8 + i];
            for (int i = 0; i < 3; i++) this.Hp[i] = encoded[11 + i];
            for (int i = 0; i < 3; i++) this.Spd[i] = encoded[14 + i];
        }

        SetRank();
        SetElement();
    }

    public MechChromo SetChromosomeFromPreset(MechPresetSO m)
    {
        Head = m.Head;
        Body = m.Body.ToArray();
        Acc = m.Acc;
        Atk = m.Atk.ToArray();
        Def = m.Def.ToArray();
        Hp = m.Hp.ToArray();
        Spd = m.Spd.ToArray();

        SetRank();
        SetElement();

        return this;
    }

    // Encode properties into chromosome
    public List<List<int>> GetChromosome()
    {
        List<int> c = new List<int>();
        List<List<int>> holder = new List<List<int>>();

        c.Add(Head);
        foreach (int item in Body)
        {
            c.Add(item);
        }
        c.Add(Acc);

        foreach (int item in Atk)
        {
            c.Add(item);
        }
        foreach (int item in Def)
        {
            c.Add(item);
        }
        foreach (int item in Hp)
        {
            c.Add(item);
        }
        foreach (int item in Spd)
        {
            c.Add(item);
        }
        holder.Add(c);
        return holder;
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
            c.Add(PlayerManager.MechCap);
        }

        return c;
    }

    public void SetRank()
    {
        int sum = Atk.Sum() + Def.Sum() + Hp.Sum() + Spd.Sum();
        // int d = Mathf.Max(PlayerManager.MechCap, 4);
        int index = Mathf.RoundToInt(sum / (PlayerManager.MechCap * 4f));
        Rank = (Ranks)index;
    }

    public float CompareMechQuest(MechChromo m)
    {
        float fitness = 0f;
        if (m != null)
        {
            fitness += Head == m.Head ? 300f : 0f;
            fitness += Acc == m.Acc ? 200f : 0f;
            Color c = new Color(Body[0] / 255f, Body[1] / 255f, Body[2] / 255f);
            Color d = new Color(m.Body[0] / 255f, m.Body[1] / 255f, m.Body[2] / 255f);
            fitness += 500f - CompareColor(c, d) * 5f;
        }

        return fitness / 10f;
    }

    public float GetFitness(List<System.Tuple<Properties, int>> fv)
    {
        float fitness = 0;
        foreach (var item in fv)
        {
            switch (item.Item1)
            {
                case Properties.Head:
                    fitness += (item.Item2 == Head) ? 100 : 0;
                    break;
                case Properties.Body:
                    Color c = new Color(Body[0] / 255f, Body[1] / 255f, Body[2] / 255f);
                    switch (item.Item2)
                    {
                        // Red
                        case 0:
                            fitness += 100f - CompareColor(c, Color.red);
                            break;
                        // Green
                        case 1:
                            fitness += 100f - CompareColor(c, Color.green);
                            break;
                        // Blue
                        case 2:
                            fitness += 100f - CompareColor(c, Color.blue);
                            break;
                        // White
                        case 3:
                            fitness += 100f - CompareColor(c, Color.white);
                            break;
                        // Black
                        case 4:
                            fitness += 100f - CompareColor(c, Color.black);
                            break;
                    }
                    break;
                case Properties.Acc:
                    fitness += (item.Item2 == Acc) ? 100 : 0;
                    break;
                case Properties.Com:
                    int sum = 0;
                    switch (item.Item2)
                    {
                        case 0:
                            sum = Atk.Sum();
                            break;
                        case 1:
                            sum = Def.Sum();
                            break;
                        case 2:
                            sum = Hp.Sum();
                            break;
                        case 3:
                            sum = Spd.Sum();
                            break;
                    }
                    fitness += CalcMe(sum, 0, PlayerManager.MechCap * 3) / 3;
                    break;
            }
        }
        return fitness;
    }

    private float CompareColor(Color A, Color B)
    {
        float r = A.r - B.r;
        float g = A.g - B.g;
        float b = A.b - B.b;

        float dis = Mathf.Sqrt(r*r + g*g + b*b);
        float max = Mathf.Sqrt(3f);

        return 100f * dis / max;
    }

    /*
    public float GetFitness2(List<int> pref)
    {
        float fitness = 0;

        // Head
        fitness += (pref[0] == head) ? 100 : 0;
        // Body
        switch (pref[1])
        {
            case -1:
                break;
            // Red
            case 0:
                fitness += (CalcMe(body[0], 0, 255) + CalcMe(body[1], 255, 0) + CalcMe(body[2], 255, 0)) / 3;
                break;
            // Green
            case 1:
                fitness += (CalcMe(body[0], 255, 0) + CalcMe(body[1], 0, 255) + CalcMe(body[2], 255, 0)) / 3;
                break;
            // Blue
            case 2:
                fitness += (CalcMe(body[0], 255, 0) + CalcMe(body[1], 255, 0) + CalcMe(body[2], 0, 255)) / 3;
                break;
            // White
            case 3:
                fitness += (CalcMe(body[0], 0, 255) + CalcMe(body[1], 0, 255) + CalcMe(body[2], 0, 255)) / 3;
                break;
            // Black
            case 4:
                fitness += (CalcMe(body[0], 255, 0) + CalcMe(body[1], 255, 0) + CalcMe(body[2], 255, 0)) / 3;
                break;
        }
        // Acc
        fitness += (pref[2] == acc) ? 100 : 0;
        // Combat
        int sum = 0;
        switch (pref[3])
        {
            case -1:
                sum = 0;
                break;
            case 0:
                sum = atk.Sum();
                break;
            case 1:
                sum = def.Sum();
                break;
            case 2:
                sum = hp.Sum();
                break;
            case 3:
                sum = spd.Sum();
                break;
        }
        fitness += CalcMe(sum, 0, cap * 3) / 3;

        return fitness;
    }
    */

    // Transform any range into 0-100 format
    private float CalcMe(int me, int min, int max)
    {
        float result = 0;

        result = (me - min) * 100 / (max - min);

        return result;
    }

    /*
    private void ResetMe()
    {
        MechIDCounter = 0;
        MechCap = 0;
    }
    */

    private void SetElement()
    {
        float[] fvTest = new float[5];

        for (int i = 0; i < 5; i++)
        {
            List<System.Tuple<Properties, int>> fv = new List<System.Tuple<Properties, int>>();
            fv.Add(System.Tuple.Create(Properties.Body, i));
            fvTest[i] = GetFitness(fv);
        }

        int max = System.Array.IndexOf(fvTest, fvTest.Max());
        if (fvTest.Distinct().ToArray().SequenceEqual(fvTest))
        {
            if (Mathf.Clamp(max, 3, 4) == max &&
                fvTest[max] > fvTest.Where((item, index) => index != max).Max() * 1.25f)
            {
                Element = (Elements)max;
            }
            else
            {
                Element = (Elements)System.Array.IndexOf
                    (fvTest.Take(3).ToArray(), fvTest.Take(3).ToArray().Max());
            }
        }
        else
        {
            var dupe = fvTest.Select((num, index) => new { num, index }).GroupBy(x => x.num)
                .Where(x => x.Count() > 1).SelectMany(x => x.Select(y => y.index)).ToList();
            if (dupe.Contains(max))
            {
                if (dupe.Count == 2 && Mathf.Clamp(dupe[0], 0, 2) == dupe[0] &&
                    Mathf.Clamp(dupe[1], 3, 4) == dupe[1])
                {
                    Element = (Elements)dupe[0];
                }
                else
                {
                    Element = Elements.None;
                }
            }
            else
            {
                Element = (Elements)max;
            }
        }
    }

    public MechSaver Save()
    {
        MechSaver m = new MechSaver();

        m.ID = ID;
        m.Head = Head;
        m.Body = Body.ToArray();
        m.Acc = Acc;
        m.Atk = Atk.ToArray();
        m.Def = Def.ToArray();
        m.Hp = Hp.ToArray();
        m.Spd = Spd.ToArray();

        return m;
    }
}
