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
        int c = MechChromoSO.Cap == 0 ? 4 : MechChromoSO.Cap;
        SetRandom(c);
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
        for (int i = 0; i < Atk.Length; i++) Atk[i] = Random.Range(1, c);
        for (int i = 0; i < Def.Length; i++) Def[i] = Random.Range(1, c);
        for (int i = 0; i < Hp.Length; i++) Hp[i] = Random.Range(1, c);
        for (int i = 0; i < Spd.Length; i++) Spd[i] = Random.Range(1, c);
    }
}
