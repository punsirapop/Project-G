using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CapybaraManager : MonoBehaviour
{
    public static CapybaraManager Instance;

    [Header("Spawning and Collision")]
    [SerializeField] private Transform[] _LeftSpawnPoints;
    [SerializeField] private Transform[] _RightSpawnPoints;
    [SerializeField] private GameObject _CapybaraPrefab;
    [Header("Capybara Overlay")]
    [SerializeField] private GameObject _Overlay;
    [SerializeField] private TextMeshProUGUI _CapybaraNameRank;
    [SerializeField] private TextMeshProUGUI _CapybaraName;
    [SerializeField] private Image _CapybaraImage;
    [SerializeField] private TextMeshProUGUI _RewardMoneyText;
    [Header("First Time Spawn Capybara")]
    [SerializeField] private GameObject _FirstTimeOverlay;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        _Overlay.SetActive(false);
        _FirstTimeOverlay.SetActive(false);
        float spawnChance = PlayerManager.CapybaraDatabase.CumulativeSpawnChance;
        // Random whether the capybara is spawned
        if (Random.Range(0f, 1f) < spawnChance)
        {
            _SpawnCapybara();
            PlayerManager.CapybaraDatabase.OnSpawnCapybara();
        }
    }

    private void _SpawnCapybara()
    {
        // Random side of spawn points
        bool isFromRight = (Random.Range(0, 2) == 0);
        Transform[] spawnPoints;
        if (isFromRight)
        {
            spawnPoints = _RightSpawnPoints;
        }
        else
        {
            spawnPoints = _LeftSpawnPoints;
        }
        // Random spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        // Spawn capybara
        CapybaraSO capybaraToSpawn = PlayerManager.CapybaraDatabase.GetRandomCapybara();
        GameObject newCapybara = Instantiate(_CapybaraPrefab, spawnPoint);
        newCapybara.GetComponent<Capybara>().SetCapybara(capybaraToSpawn, isFromRight);
        // If spawn capybara for the first time, show first time overlay
        if (PlayerManager.CapybaraDatabase.IsFirstSpawn)
        {
            _FirstTimeOverlay.SetActive(true);
            PlayerManager.CapybaraDatabase.SetIsFirstSpawn(false);
        }
        SoundEffectManager.Instance.PlaySoundEffect("CappySpawn");
    }

    // Open overlay when successfully tap capybara
    public void OpenOverlay(CapybaraSO capybara)
    {
        _Overlay.SetActive(true);
        _CapybaraNameRank.text = capybara.Name + " (" + capybara.Rank.ToString() + ")";
        _CapybaraName.text = capybara.Name;
        _CapybaraImage.sprite = capybara.Sprites[0];
        _RewardMoneyText.text = capybara.MoneyReward.ToString();
    }
}

