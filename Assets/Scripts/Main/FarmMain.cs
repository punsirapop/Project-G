using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmMain : FarmMngFunc
{
    // Game object of sprite group of each state
    [SerializeField] private GameObject _Locked;
    [SerializeField] private GameObject _Unlockable;
    [SerializeField] private GameObject _Unlocked;

    // Sprite of this factory in main game page
    [SerializeField] private Sprite _MainLocked;
    private Sprite _MainNormal;
    private Sprite _MainBroken;
    private Sprite _Locker;

    // Actual UI element object
    [SerializeField] private GameObject _MainBackground;
    [SerializeField] private GameObject _MainLightBoard;
    [SerializeField] private GameObject _FixButton;
    [SerializeField] private GameObject _UnlockableLockerUI;
    [SerializeField] private GameObject _LockedLockerUI;

    // Farm information
    private int _FarmIndex;
    public FarmSO FarmDatabase => PlayerManager.FarmDatabase[_FarmIndex];

    // Sound Effect When Change scene
    public string _SoundEffectName = "Button"; // Name of the sound effect to play

    void Start()
    {
        if (PlayerManager.FarmDatabase[_FarmIndex].Condition > 0)
        {
            _MainBackground.GetComponent<Image>().sprite = _MainNormal;
        }
        else
        {
            _MainBackground.GetComponent<Image>().sprite = _MainBroken;
        }
        _UnlockableLockerUI.GetComponent<Image>().sprite = _Locker;
        _LockedLockerUI.GetComponent<Image>().sprite = _Locker;
        //_MainBackground.GetComponent<Button>().onClick.AddListener(() => EnterFarm());
        // _MainLightBoard.GetComponent<Button>().onClick.AddListener(() => EnterKnapsackPuzzle());
    }

    private void Update()
    {
        // for debug purposes
        int lights = 0;
        bool fixable = false;
        foreach (var item in _MainLightBoard.GetComponentsInChildren<Image>())
        {
            if (lights >= PlayerManager.FarmDatabase[_FarmIndex].Condition)
            {
                fixable = true;
            }
            item.color = (lights < PlayerManager.FarmDatabase[_FarmIndex].Condition) ? Color.green : Color.red;
            lights++;
        }
        _FixButton.SetActive(fixable);
    }

    public void SetFarm(int newFarmIndex, FarmSO newFarmSO)
    {
        _FarmIndex = newFarmIndex;
        _MainNormal = newFarmSO.MainNormal;
        _MainBroken = newFarmSO.MainBroken;
        _Locker = newFarmSO.Locker;
        RenderSprites();
        /*
        int lights = 0;
        foreach (var item in _MainLightBoard.GetComponentsInChildren<Image>())
        {
            item.color = (lights < newFarmSO.Condition) ? Color.green : Color.red;
            lights++;
        }
        */
    }

    // Render the sprites according to LockStatus
    public void RenderSprites()
    {
        _Locked.SetActive(PlayerManager.FarmDatabase[_FarmIndex].LockStatus == LockableStatus.Lock);
        _Unlockable.SetActive(PlayerManager.FarmDatabase[_FarmIndex].LockStatus == LockableStatus.Unlockable);
        _Unlocked.SetActive(PlayerManager.FarmDatabase[_FarmIndex].LockStatus == LockableStatus.Unlock);
    }

    public void EnterFarm()
    {
        SoundEffectManager.Instance.PlaySoundEffect(_SoundEffectName);
        PlayerManager.CurrentFarmIndex = _FarmIndex;
        this.GetComponent<SceneMng>().ChangeScene("Farm");
    }

    #region Lock and Unlocking
    // Display overlay for Locked factory
    public void DisplayLockOverlay()
    {
        MainPageManager.Instance.DisplayUnlockOverlay(this);
    }

    // Unlock the Unlockable FactorySO
    public void UnlockFarm()
    {
        PlayerManager.FarmDatabase[_FarmIndex].Unlock();
        GetComponent<Animator>().Play("UnlockFarm");
    }

    // Wrap function for validate unlocking and render UI, triggered at the end of UnlockFactory animation
    public void OnUnlockAnimationEnd()
    {
        GetComponent<Animator>().enabled = false;
        PlayerManager.ValidateUnlocking();
        MainPageManager.Instance.RenderFacilities();
    }
    #endregion

    public void OnFixButtonClick()
    {
        PlayerManager.Instance.SetFacilityToFix(PlayerManager.FacilityType.Farm, _FarmIndex);
        MainPageManager.Instance.DisplayFixChoice(PlayerManager.FarmDatabase[_FarmIndex]);
    }
    // ------- DEBUG -------
    public void AddChromo(int amount)
    {
        AddChromo(PlayerManager.FarmDatabase[_FarmIndex], amount);
    }

    //public void FixFarm()
    //{
    //    PlayerManager.FarmDatabase[_FarmIndex].Fixed();
    //}

    //public void OnFixButtonClick()
    //{
    //    PlayerManager.Instance.SetFacilityToFix(PlayerManager.FacilityType.Farm, _FarmIndex);
    //    MainPageManager.Instance.DisplayFixChoice();
    //}
}
