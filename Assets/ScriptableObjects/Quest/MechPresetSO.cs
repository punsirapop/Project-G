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
}
