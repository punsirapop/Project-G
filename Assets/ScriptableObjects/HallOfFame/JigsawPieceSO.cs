using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObject/JigsawPiece")]
public class JigsawPieceSO : LockableObject
{
    [Header("Jigsaw Information")]
    [SerializeField] private string _Name;
    public string Name => _Name;
    [SerializeField] private JigsawLevel _Level;
    public JigsawLevel Level => _Level;
    [SerializeField] private PuzzleType _HowToObtain;
    public PuzzleType HowToObtain => _HowToObtain;
    [SerializeField] private DialogueSO _TestingDialogue;   // Dialogue to use in case if it's PuzzleType.Dialogue for _HowToObtain
    //private int _SuccessCount;      // The number of success testing through _HowToObtain
    //private int _FailCount;         // The number of fail testing through _HowToObtain

    public override string GetRequirementPrefix()
    {
        return "Obtain";
    }

    public override string GetLockableObjectName()
    {
        return Name + " (" + _Level.ToString() + ")";
    }

    private void OnEnable()
    {
        SaveManager.OnReset += Reset;
    }

    private void OnDestroy()
    {
        SaveManager.OnReset -= Reset;
    }

    public new void Reset()
    {
        base.Reset();
        //_SuccessCount = 0;
        //_FailCount = 0;
    }
}

public enum JigsawLevel
{
    None,       // Place holder used as null for the Level
    Copper,     // Reflect learning level of Understand
    Silver,     // Reflect learning level of Demonstration
    Gold        // Reflect learning level of Solving
}