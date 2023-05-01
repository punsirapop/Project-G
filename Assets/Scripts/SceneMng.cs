using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMng : MonoBehaviour
{
    public static SceneMng Instance;

    [SerializeField] GameObject[] overlays;
    public static string SceneToReturn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    // Overloading ChangeScene for string parameter
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleOverlay(int index)
    {
        switch (overlays[index].activeSelf)
        {
            case true:
                overlays[index].SetActive(false);
                break;
            case false:
                foreach (var item in overlays)
                {
                    item.SetActive(false);
                }
                overlays[index].SetActive(true);
                break;
        }
    }

    // Set and change to the proper puzzle scene
    public static void SetAndChangePuzzleScene(JigsawPieceSO piece)
    {
        PuzzleType puzzleType = piece.HowToObtain;
        List<PuzzleType> crossoverTypes = new List<PuzzleType> 
        { 
            PuzzleType.CrossoverOnePointDemon,
            PuzzleType.CrossoverOnePointSolve,
            PuzzleType.CrossoverTwoPointsDemon,
            PuzzleType.CrossoverTwoPointsSolve
        };
        List<PuzzleType> selectionTypes = new List<PuzzleType>
        {
            PuzzleType.SelectionTournamentDemon,
            PuzzleType.SelectionTournamentSolve,
            PuzzleType.SelectionRouletteDemon,
            PuzzleType.SelectionRouletteSolve,
            PuzzleType.SelectionRankDemon,
            PuzzleType.SelectionRankSolve
        };
        List<PuzzleType> knapackTypes = new List<PuzzleType>
        {
            PuzzleType.KnapsackStandardDemon,
            PuzzleType.KnapsackStandardSolve,
            PuzzleType.KnapsackMultiDimenDemon,
            PuzzleType.KnapsackMultiDimenSolve,
            PuzzleType.KnapsackMultipleDemon,
            PuzzleType.KnapsackMultipleSolve
        };
        PlayerManager.SetCurrentJigsawPiece(piece); 
        if (puzzleType == PuzzleType.Dialogue)
        {
            piece.AddProgressCount(true, 1); // tmp get success count
            Debug.Log("It's some dialogue scene");
        }
        else if (crossoverTypes.Contains(puzzleType))
        {
            SceneToReturn = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("CrossoverPuzzle");
        }
        else if (selectionTypes.Contains(puzzleType))
        {
            SceneToReturn = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("SelectionPuzzle");
        }
        else if (knapackTypes.Contains(puzzleType))
        {
            SceneToReturn = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("KnapsackPuzzle");
        }
    }

    // Return to previous scene, especially after the puzzle scene end
    public static void ReturnToPreviousScene()
    {
        SceneManager.LoadScene(SceneToReturn);
    }
}
