using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // public static SaveManager Instance;

    // call when reset the game
    public static event Action OnReset;

    // struct to save text data to json
    public struct SaveData
    {
        // public List<int> mechCurrentGen;
        // public int mechCurrentID;
        public int Days;
        public int Money;
    }

    // SOs to save farms info
    [SerializeField] protected FarmSO[] _SavedFarms;

    // json save path
    string path = "";
    string persistentPath = "";
    protected string currentPath;

    // object save path
    string mechAssetsPath = Path.Combine("Assets", "Resources", "Mechs");
    string mechResourcePath = "Mechs";
    string sideQuestAssetsPath = Path.Combine("Assets", "Resources", "SideQuest");
    string sideQuestResourcePath = "SideQuest";


    private void Awake()
    {
        // Debug.Log($"Save Farms: {_SavedFarms.Length}");
    }

    // check if path is ready to save
    protected bool SaveReady()
    {
        return File.Exists(currentPath);
    }
    
    // set path for json
    protected void SetPaths()
    {
        path = Path.Combine(Application.dataPath, "Save.json");
        persistentPath = Path.Combine(Application.persistentDataPath, "Save.json");

        currentPath = path;
    }

    // generate struct to save
    public SaveData SetJson()
    {
        SaveData saveData = new SaveData();
        // saveData.mechCurrentGen = new List<int>(FarmManager.CurrentGen);
        // saveData.mechCurrentID = FarmManager.CurrentID;
        saveData.Days = PlayerManager.CurrentDate.ToDay();
        saveData.Money = PlayerManager.Money;

        return saveData;
    }

    // set data from saved json to game
    public virtual void LoadJson(SaveData s)
    {
        /*
        FarmManager.CurrentGen = new List<int>(s.mechCurrentGen);
        FarmManager.CurrentID = s.mechCurrentID;
        */
        PlayerManager.SetCurrentDate(TimeManager.Date.FromDay(s.Days));
        PlayerManager.SetMoney(s.Money);
        Debug.Log("Data loaded");
    }

    // save file
    public void SaveToFile()
    {
        // ---------- Save json ----------
        SetPaths();
        string savePath = currentPath;

        Debug.Log("Saving at " + savePath);
        SaveData saveData = SetJson();
        string json = JsonUtility.ToJson(saveData);

        using StreamWriter streamWriter = new StreamWriter(savePath);
        streamWriter.Write(json);
        streamWriter.Close();

        // ---------- Save scriptable objects ----------
        for (int i = 0; i < 4; i++)
        {
            _SavedFarms[i].SetMe(PlayerManager.FarmDatabase[i]);
        }
        
        // ---------- Save objects ----------
        // Mechs
        MechChromoSO[] loaded = PlayerManager.FarmDatabase.SelectMany(x => x.MechChromos).ToArray();
        MechChromoSO[] saved = Resources.LoadAll<MechChromoSO>(mechResourcePath);
        Debug.Log("LOADED: " + loaded.Count() + " SAVED: " + saved.Length);
        Debug.Log("UNSAVED: " + loaded.Except(saved).Count() + " UNLOADED: " + saved.Except(loaded).Count());
        // Side quest
        SideQuestSO[] loadedSideQuest = PlayerManager.SideQuestDatabase.GetAllQuest().ToArray();
        SideQuestSO[] savedSideQuest = Resources.LoadAll<SideQuestSO>(sideQuestResourcePath);
        // Save new items
        foreach (var item in loaded.Except(saved))
        {
            AssetDatabase.CreateAsset(item, Path.Combine(mechAssetsPath, item.ID.ToString() + ".asset"));
        }
        foreach (var currentQuest in loadedSideQuest.Except(savedSideQuest))
        {
            AssetDatabase.CreateAsset(currentQuest, Path.Combine(sideQuestAssetsPath, currentQuest.ID.ToString() + ".asset"));
        }
        // Delete unused items
        foreach (var item in saved.Except(loaded))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
        }
        foreach (var oldQuest in savedSideQuest.Except(loadedSideQuest))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldQuest));
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log(json);
    }

    // load file
    public virtual void LoadFromFile()
    {
        // ---------- Load json ----------
        SetPaths();
        string loadPath = currentPath;

        using StreamReader streamReader = new StreamReader(loadPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();

        SaveData loadData = JsonUtility.FromJson<SaveData>(json);
        LoadJson(loadData);

        // ---------- Load scriptable objects ----------
        for (int i = 0; i < _SavedFarms.Length; i++)
        {
            PlayerManager.FarmDatabase[i].SetMe(_SavedFarms[i]);
        }
        
    }

    // trigger reset event
    public void ResetGame()
    {
        Debug.Log("PLS RESET");
        // delete mech chromos
        MechChromoSO[] deleteMe = Resources.LoadAll<MechChromoSO>("Mechs");
        foreach (MechChromoSO mechChromoSO in deleteMe)
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mechChromoSO));
        // Delete side quests
        SideQuestSO[] oldSideQuests = Resources.LoadAll<SideQuestSO>(sideQuestResourcePath);
        foreach (SideQuestSO quest in oldSideQuests)
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(quest));
        OnReset?.Invoke();
    }
}