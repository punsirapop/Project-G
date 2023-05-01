using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TimeManager;

/*
 * Store various miscellanous functions
 * Still don't know how to categorize them
 * - AddChromo
 * - DelChromo
 */
public class PlayerManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static PlayerManager Instance;

    public static Date CurrentDate;
    public static int MechStatCap;

    // Resources
    public static int Money { get; private set; }

    // ChapterSO for validation purpose
    public static ContentChapterSO[] ContentChapterDatabase;
    [SerializeField] private ContentChapterSO[] ContentChapterDatabaseHelper;

    // Facilities
    public static int CurrentFactoryIndex = 0;
    public static int CurrentFarmIndex = 1;
    public static FactorySO[] FactoryDatabase;
    public static FarmSO[] FarmDatabase;
    public static FactorySO CurrentFactoryDatabase => FactoryDatabase[CurrentFactoryIndex];
    public static FarmSO CurrentFarmDatabase => FarmDatabase[CurrentFarmIndex];
    [SerializeField] private FactorySO[] FactoryDatabaseHelper;
    [SerializeField] private FarmSO[] FarmDatabaseHelper;

    // Facility fixing
    public static bool FixingFacility = false;
    public static FacilityType FacilityToFix;
    public static int FacilityToFixIndex;

    // Puzzle/JigsawPiece for HallOfFame
    public static JigsawPieceSO[] JigsawPieceDatabase;
    [SerializeField] private JigsawPieceSO[] JigsawPieceDatabaseHelper;
    public static JigsawPieceSO CurrentJigsawPiece;
    public static PuzzleType PuzzleToGenerate => CurrentJigsawPiece.HowToObtain;
    public static bool IsSolveFixFactory;       // Special boolean for indicate whether factory is fixed in solve mode or not

    public enum FacilityType
    {
        Factory,
        Farm
    }

    // Assign factories data from serialized field on editor to the static variable
    public void OnAfterDeserialize()
    {
        ContentChapterDatabase = ContentChapterDatabaseHelper;
        FactoryDatabase = FactoryDatabaseHelper;
        FarmDatabase = FarmDatabaseHelper;
        JigsawPieceDatabase = JigsawPieceDatabaseHelper;
    }

    // Reflect the value back into editor
    public void OnBeforeSerialize()
    {
        ContentChapterDatabaseHelper = ContentChapterDatabase;
        FactoryDatabaseHelper = FactoryDatabase;
        FarmDatabaseHelper = FarmDatabase;
        JigsawPieceDatabaseHelper = JigsawPieceDatabase;
    }

    private void Awake()
    {
        TimeManager.OnChangeDate += OnChangeDate;
        SaveManager.OnReset += ResetMoney;
        SaveManager.OnReset += ValidateUnlocking;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("FOUND DUPE");
            Destroy(Instance.gameObject);
        }

        FixingFacility = false;
    }

    private void OnDestroy()
    {
        TimeManager.OnChangeDate -= OnChangeDate;
    }

    public static void OnChangeDate(Date d)
    {
        int day = d.CompareDate(CurrentDate);
        Debug.Log("ASK TO BREED FOR " + day + " FROM PM");
        for (int i = 0; i < day; i++)
        {
            foreach (var item in FarmDatabase)
            {
                if (item.Status == Status.BREEDING) item.FillBreedGuage();
            }
            foreach (var factory in FactoryDatabase)
            {
                if (factory.Status == Status.BREEDING)
                {
                    factory.FillBreedGuage();
                }
            }
        }

        CurrentDate = d.DupeDate();
    }

    #region Money
    public void ResetMoney()
    {
        Money = 3000;   // Hard-code initial amount of Money
    }

    // Deduct Money and return true if Money is enough. Otherwise, do nothing and return false
    public static bool SpendMoneyIfEnought(int deductAmount)
    {
        if (deductAmount <= Money)
        {
            Money -= deductAmount;
            return true;
        }
        else
        {
            return false;
        }
    }

    // Gain money and return true if success, Otherwise, do nothing and return false
    public static bool GainMoneyIfValid(int gainAmount)
    {
        if (gainAmount >= 0)
        {
            Money += gainAmount;
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Facility Navigation and fixing
    // Change current factory
    public void SetCurrentFactoryIndex(int newFactoryIndex)
    {
        CurrentFactoryIndex = newFactoryIndex;
    }

    public void SetCurrentFarmIndex(int index)
    {
        CurrentFarmIndex = index;
    }

    // Set facility to be fixed after the puzzle is done
    public void SetFacilityToFix(FacilityType facilityType, int facilityIndex)
    {
        FixingFacility = true;
        FacilityToFix = facilityType;
        FacilityToFixIndex = facilityIndex;
    }

    // Get the facility to fix name
    public static string GetFacilityToFixName()
    {
        if (FacilityToFix == FacilityType.Factory)
        {
            return FactoryDatabase[FacilityToFixIndex].Name;
        }
        else if (FacilityToFix == FacilityType.Farm)
        {
            return FarmDatabase[FacilityToFixIndex].Name;
        }
        return "Invalid Facility";
    }

    // Fix the facility and return the name of fixed facility
    public static void FixFacility()
    {
        if (FacilityToFix == FacilityType.Factory)
        {
            FactoryDatabase[FacilityToFixIndex].Fixed();
        }
        else if (FacilityToFix == FacilityType.Farm)
        {
            FarmDatabase[FacilityToFixIndex].Fixed();
        }
    }
    #endregion

    // Validate locking status of all SO
    public static void ValidateUnlocking()
    {
        foreach (ContentChapterSO chapter in ContentChapterDatabase)
        {
            chapter.ValidateUnlockRequirement();
        }
        foreach (FactorySO factory in FactoryDatabase)
        {
            factory.ValidateUnlockRequirement();
        }
        foreach (FarmSO farm in FarmDatabase)
        {
            farm.ValidateUnlockRequirement();
        }
        foreach (JigsawPieceSO piece in JigsawPieceDatabase)
        {
            piece.ValidateUnlockRequirement();
        }
    }

    // TEMP function for start the game with the least restriction
    public static void GMStart()
    {
        Money = 1000000;
        foreach (ContentChapterSO chapter in ContentChapterDatabase)
        {
            chapter.ForceUnlock();
        }
        foreach (FactorySO factory in FactoryDatabase)
        {
            factory.ForceUnlock();
        }
        foreach (FarmSO farm in FarmDatabase)
        {
            farm.ForceUnlock();
        }
        foreach (JigsawPieceSO piece in JigsawPieceDatabase)
        {
            piece.ForceUnlock();
        }
        ValidateUnlocking();
    }

    #region Puzzle
    // Return a description string of the given PuzzleType
    public static string DescribePuzzleType(PuzzleType puzzleType)
    {
        switch (puzzleType)
        {
            default:
                return "Unknown puzzle type";
            case PuzzleType.Dialogue:
                return "Answer the questions";
            // Crossover
            case PuzzleType.CrossoverOnePointDemon:
                return "Demonstrate Single-point Crossover";
            case PuzzleType.CrossoverOnePointSolve:
                return "Solve Crossover problem";
            case PuzzleType.CrossoverTwoPointsDemon:
                return "Demonstrate Two-point Crossover";
            case PuzzleType.CrossoverTwoPointsSolve:
                return "Solve Crossover problem";
            // Selection
            case PuzzleType.SelectionTournamentDemon:
                return "Demonstrate Tournament-based Selection";
            case PuzzleType.SelectionTournamentSolve:
                return "Solve Selection problem";
            case PuzzleType.SelectionRouletteDemon:
                return "Demonstrate Roulette Wheel Selection";
            case PuzzleType.SelectionRouletteSolve:
                return "Solve Selection problem";
            case PuzzleType.SelectionRankDemon:
                return "Demonstrate Rank-based Selection";
            case PuzzleType.SelectionRankSolve:
                return "Solve Selection problem";
            // Knapsack
            case PuzzleType.KnapsackStandardDemon:
                return "Demonstrate Standard Knapsack encoding/decoding";
            case PuzzleType.KnapsackStandardSolve:
                return "Solve Knapsack encoding/decoding problem";
            case PuzzleType.KnapsackMultiDimenDemon:
                return "Demonstrate Multidimensional Knapsack encoding/decoding";
            case PuzzleType.KnapsackMultiDimenSolve:
                return "Solve Knapsack encoding/decoding problem";
            case PuzzleType.KnapsackMultipleDemon:
                return "Demonstrate Multiple Knapsack encoding/decoding";
            case PuzzleType.KnapsackMultipleSolve:
                return "Solve Knapsack encoding/decoding problem";
        }
    }

    public static void SetCurrentJigsawPiece(JigsawPieceSO jigsawPiece)
    {
        CurrentJigsawPiece = jigsawPiece;
    }

    public static List<string> RecordPuzzleResult(bool isSuccess)
    {
        // Record progress in JigsawPieceSO
        int[] amountAndMoney = CurrentJigsawPiece.AddProgressCount(isSuccess, 1);
        // Generate feedback string from JigsawPieceSO.AddProgressCount()
        List<string> feedbackTexts = new List<string>();
        int amount = amountAndMoney[0];
        int money = amountAndMoney[1];
        string obtainSuffix = " x " + CurrentJigsawPiece.GetLockableObjectName();
        // Feedback on piece count
        if (amount > 0)
        {
            feedbackTexts.Add("- Obtain: " + amount.ToString() + obtainSuffix);
        }
        else if (amount < 0)
        {
            feedbackTexts.Add("- Fail to obtain: " + (-amount).ToString() + obtainSuffix);
        }
        // Feedback on money
        if (money > 0)
        {
            feedbackTexts.Add("- Gain money: " + money.ToString());
        }
        else if (money < 0)
        {
            feedbackTexts.Add("- Spend money: " + (-money).ToString());
        }
        // Fix facility if it's in fixing mode
        if (!FixingFacility)
        {
            return feedbackTexts;
        }
        FixingFacility = false;
        string fixingFeedback = "";
        if (isSuccess)
        {
            FixFacility();
            fixingFeedback += "- Successfully fix: ";
        }
        else
        {
            fixingFeedback += "- Fail to fix: ";
        }
        fixingFeedback += GetFacilityToFixName();
        feedbackTexts.Add(fixingFeedback);
        return feedbackTexts;
    }
    #endregion
}

public enum PuzzleType
{
    Dialogue,
    CrossoverOnePointDemon,
    CrossoverOnePointSolve,
    CrossoverTwoPointsDemon,
    CrossoverTwoPointsSolve,
    SelectionTournamentDemon,
    SelectionTournamentSolve,
    SelectionRouletteDemon,
    SelectionRouletteSolve,
    SelectionRankDemon,
    SelectionRankSolve,
    KnapsackStandardDemon,
    KnapsackStandardSolve,
    KnapsackMultiDimenDemon,
    KnapsackMultiDimenSolve,
    KnapsackMultipleDemon,
    KnapsackMultipleSolve
}