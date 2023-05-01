using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FixChoiceOverlay : MonoBehaviour
{
    // Actual UI element
    [SerializeField] private Button _QuestionButton;
    [SerializeField] private Button _DemonButton;
    [SerializeField] private Button _SolveButton;
    [SerializeField] private Transform _QuestionJigsawsHolder;
    [SerializeField] private Transform _DemonJigsawsHolder;
    [SerializeField] private Transform _SolveJigsawsHolder;
    [SerializeField] private TextMeshProUGUI _QuestionCostText;
    [SerializeField] private TextMeshProUGUI _DemonCostText;
    [SerializeField] private TextMeshProUGUI _SolveCostText;
    [SerializeField] private GameObject _JigsawNamePrefab;

    // Fixing cost
    [SerializeField] private int _QuestionCost;
    [SerializeField] private int _DemonCost;
    [SerializeField] private int _SolveCost;

    // Jigsaw data for generating puzzle
    private List<JigsawPieceSO> _QuestionJigsawPieces;
    private List<JigsawPieceSO> _DemonJigsawPieces;
    private List<JigsawPieceSO> _SolveJigsawPieces;

    private void Start()
    {
        _QuestionCostText.text = PlayerManager.Money.ToString() + "/" + _QuestionCost.ToString();
        _DemonCostText.text = PlayerManager.Money.ToString() + "/" + _DemonCost.ToString();
        _SolveCostText.text = PlayerManager.Money.ToString() + "/" + _SolveCost.ToString();
    }

    public void SetFixChoice(LockableObject clickedFacility)
    {
        // Refresh and clear previous data
        _QuestionJigsawPieces = new List<JigsawPieceSO>();
        _DemonJigsawPieces = new List<JigsawPieceSO>();
        _SolveJigsawPieces = new List<JigsawPieceSO>();
        _ClearHolder(_QuestionJigsawsHolder);
        _ClearHolder(_DemonJigsawsHolder);
        _ClearHolder(_SolveJigsawsHolder);
        // Calculate obtainable jigsaw from such facility
        JigsawPieceGroupSO[] jigsawGroups;
        jigsawGroups = ((FactorySO)clickedFacility).ObtainableJisawGroups;
        //if (PlayerManager.FacilityToFix == PlayerManager.FacilityType.Factory)
        //{
        //    jigsawGroups = ((FactorySO)clickedFacility).ObtainableJisawGroups;
        //}
        //else if (PlayerManager.FacilityToFix == PlayerManager.FacilityType.Farm)
        //{
        //    jigsawGroups = ((Farm)clickedFacility).ObtainableJisawGroups;
        //}
        // Add only jigsaw that doesn't be locked to the generating list
        foreach (JigsawPieceGroupSO jigsawGroup in jigsawGroups)
        {
            foreach (JigsawPieceSO jigsawPiece in jigsawGroup.JigsawPieces)
            {
                if (jigsawPiece.LockStatus == LockableStatus.Lock)
                {
                    continue;
                }
                switch (jigsawPiece.Level)
                {
                    default:
                        break;
                    case JigsawLevel.Copper:
                        _QuestionJigsawPieces.Add(jigsawPiece);
                        _SpawnJigsawName(_QuestionJigsawsHolder, jigsawPiece);
                        break;
                    case JigsawLevel.Silver:
                        _DemonJigsawPieces.Add(jigsawPiece);
                        _SpawnJigsawName(_DemonJigsawsHolder, jigsawPiece);
                        break;
                    case JigsawLevel.Gold:
                        _SolveJigsawPieces.Add(jigsawPiece);
                        _SpawnJigsawName(_SolveJigsawsHolder, jigsawPiece);
                        break;
                }
            }
        }
        // Disable button for the method that dosen't have any obtainable jigsaw
        _QuestionButton.interactable = (_QuestionJigsawPieces.Count > 0);
        _DemonButton.interactable = (_DemonJigsawPieces.Count > 0);
        _SolveButton.interactable = (_SolveJigsawPieces.Count > 0);
        // Assign method for move to obtain jigsaw scene when click on interactable button
        _QuestionButton.onClick.AddListener(() => _OnChoiceButtonClick(_QuestionJigsawPieces, _QuestionCost));
        _DemonButton.onClick.AddListener(() => _OnChoiceButtonClick(_DemonJigsawPieces, _DemonCost));
        _SolveButton.onClick.AddListener(() => _OnChoiceButtonClick(_SolveJigsawPieces, _SolveCost));
    }

    // Destroy all existing object in the given holder
    private void _ClearHolder(Transform holder)
    {
        foreach (Transform child in holder)
        {
            Destroy(child.gameObject);
        }
    }

    // Spawn jigsaw name object in its holder
    private void _SpawnJigsawName(Transform holder, JigsawPieceSO jigsawPiece)
    {
        GameObject newJigsawName = Instantiate(_JigsawNamePrefab, holder);
        newJigsawName.GetComponent<TextMeshProUGUI>().text = jigsawPiece.GetLockableObjectName();
    }

    // Proportionate random picking the jigsaw to obtain
    private void _OnChoiceButtonClick(List<JigsawPieceSO> obtainableJigsaws, int cost)
    {
        // Spend fixing cost and if transaction fail, do nothing
        bool isTransactionSuccess = PlayerManager.SpendMoneyIfEnought(cost);
        if (!isTransactionSuccess)
        {
            return;
        }
        List<float> jigsawChance = new List<float>();
        foreach (JigsawPieceSO piece in obtainableJigsaws)
        {
            // If found piece with 0 SuccessCount, go obtain it
            if (piece.SuccessCount == 0)
            {
                piece.GoToObtain();
                return;
            }
            // If all piece has SuccessCount > 0, calculate the chance
            // The more its SuccessCount, the less the chance
            jigsawChance.Add(1f/((float)piece.SuccessCount));
        }
        // Random and pick piece
        float randomValue = Random.Range(0f, jigsawChance.Sum());
        for (int i = 0; i < obtainableJigsaws.Count; i++)
        {
            if (randomValue < jigsawChance[i])
            {
                obtainableJigsaws[i].GoToObtain();
                return;
            }
            randomValue -= jigsawChance[i];
        }
        // If nothing's wrong, it should not goes down here, but just in case
        obtainableJigsaws[0].GoToObtain();
    }
}
