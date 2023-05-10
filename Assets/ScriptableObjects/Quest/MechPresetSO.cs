using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/StatPreset")]
public class MechPresetSO : ScriptableObject
{
    public int Head;
    public int[] Body;
    public int Acc;
    public int[] Atk;
    public int[] Def;
    public int[] Hp;
    public int[] Spd;

    private void Awake()
    {
        //SetRandom(PlayerManager.MechCap);
    }

    public void SetRandom(int c)
    {
        Head = Random.Range(0, 20);
        Body = new int[3];
        for (int i = 0; i < 3; i++) Body[i] = Random.Range(0, 256);
        Acc = Random.Range(0, 10);

        Atk = new int[3];
        Def = new int[3];
        Hp = new int[3];
        Spd = new int[3];
        for (int i = 0; i < Atk.Length; i++) Atk[i] = Random.Range(1, c+1);
        for (int i = 0; i < Def.Length; i++) Def[i] = Random.Range(1, c+1);
        for (int i = 0; i < Hp.Length; i++) Hp[i] = Random.Range(1, c+1);
        for (int i = 0; i < Spd.Length; i++) Spd[i] = Random.Range(1, c+1);
    }

    public MechSaver Save()
    {
        MechSaver m = new MechSaver();

        m.Head = Head;
        m.Body = Body.ToArray();
        m.Acc = Acc;
        m.Atk = Atk.ToArray();
        m.Def = Def.ToArray();
        m.Hp = Hp.ToArray();
        m.Spd = Spd.ToArray();

        return m;
    }

    public void Load(MechSaver m)
    {
        Head = m.Head;
        Body = m.Body.ToArray();
        Acc = m.Acc;
        Atk = m.Atk.ToArray();
        Def = m.Def.ToArray();
        Hp = m.Hp.ToArray();
        Spd = m.Spd.ToArray();
    }
}
