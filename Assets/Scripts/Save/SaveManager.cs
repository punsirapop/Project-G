using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static SaveManager;

public class SaveManager : MonoBehaviour
{
    // public static SaveManager Instance;

    // call when reset the game
    public static event Action OnReset;

    // json save path
    string path = "";
    string persistentPath = "";
    protected string currentPath;

    // object save path
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
    protected void SetPaths(string name)
    {
        path = Path.Combine(Application.dataPath, "Save", name);
        persistentPath = Path.Combine(Application.persistentDataPath, "Save", name);

        currentPath = path;
    }

    // generate struct to save
    public Saver SetJson()
    {
        Saver saveData = new Saver();

        saveData.Days = PlayerManager.CurrentDate.ToDay();
        saveData.Money = PlayerManager.Money;
        saveData.MechIDCounter = PlayerManager.MechIDCounter;
        saveData.MechCap = PlayerManager.MechCap;

        // JigsawSO
        saveData.JigsawLockStatus = PlayerManager.JigsawPieceDatabase.Select(x => x.LockStatus).ToArray();
        saveData.SuccessCounts = PlayerManager.JigsawPieceDatabase.Select(x => x.SuccessCount).ToArray();
        saveData.FailCounts = PlayerManager.JigsawPieceDatabase.Select(x => x.FailCount).ToArray();

        // InfoSO
        saveData.IsNeverShow = PlayerManager.InformationDatabase.Select(x => x.IsNeverShow).ToArray();

        // ShopSO
        saveData.ShopItems = PlayerManager.Shop.ShopItems.Select(x => x.Save()).ToArray();
        saveData.InStock = PlayerManager.Shop.InStock.ToArray();
        saveData.DayLeftBeforeRestock = PlayerManager.Shop.DayLeftBeforeRestock;

        // CappySO
        saveData.CumulativeSpawnChance = PlayerManager.CapybaraDatabase.CumulativeSpawnChance;
        saveData.IsFirstSpawn = PlayerManager.CapybaraDatabase.IsFirstSpawn;
        saveData.CappyLockStatus = PlayerManager.CapybaraDatabase.Capybaras.Select(x => x.LockStatus).ToArray();
        saveData.FoundCount = PlayerManager.CapybaraDatabase.Capybaras.Select(x => x.FoundCount).ToArray();

        return saveData;
    }

    // set data from saved json to game
    public void LoadJson(Saver s)
    {
        PlayerManager.SetCurrentDate(TimeManager.Date.FromDay(s.Days));
        PlayerManager.SetMoney(s.Money);
        PlayerManager.MechIDCounter = s.MechIDCounter;
        PlayerManager.MechCap = s.MechCap;

        // JigsawSO
        for (int i = 0; i < PlayerManager.JigsawPieceDatabase.Length; i++)
        {
            PlayerManager.JigsawPieceDatabase[i].Load(s.JigsawLockStatus[i],
                s.SuccessCounts[i], s.FailCounts[i]);
        }

        // InfoSO
        for (int i = 0; i < PlayerManager.InformationDatabase.Length; i++)
        {
            PlayerManager.InformationDatabase[i].Load(s.IsNeverShow[i]);
        }

        // ShopSO
        for (int i = 0; i < PlayerManager.Shop.ShopItems.Length; i++)
        {
            PlayerManager.Shop.ShopItems[i].Load(s.ShopItems[i]);
            PlayerManager.Shop.InStock[i] = s.InStock[i];
        }
        PlayerManager.Shop.DayLeftBeforeRestock = s.DayLeftBeforeRestock;

        // CappySO
        PlayerManager.CapybaraDatabase.Load(s.CumulativeSpawnChance, s.IsFirstSpawn,
            s.CappyLockStatus, s.FoundCount);

        // Debug.Log("Data loaded");
    }

    // save file
    public void SaveToFile()
    {
        // ---------- Save json ----------
        SetPaths("Save.json");
        string savePath = currentPath;

        // Debug.Log("Saving at " + savePath);
        Saver saveData = SetJson();
        string json = JsonUtility.ToJson(saveData);

        StreamWriter streamWriter = new StreamWriter(savePath);
        streamWriter.Write(json);
        streamWriter.Close();

        // Save Farm
        SetPaths("Farms.json");
        savePath = currentPath;
        streamWriter = new StreamWriter(savePath);

        List<FarmSaver> f = new List<FarmSaver>();
        foreach (var item in PlayerManager.FarmDatabase)
        {
            f.Add(item.Save());
        }
        FarmXaver fx = new FarmXaver(f);
        json = JsonUtility.ToJson(fx);
        streamWriter.Write(json);
        streamWriter.Close();

        // Save Factory
        SetPaths("Factories.json");
        savePath = currentPath;
        streamWriter = new StreamWriter(savePath);

        List<FactorySaver> fc = new List<FactorySaver>();
        foreach (var item in PlayerManager.FactoryDatabase)
        {
            fc.Add(item.Save());
        }
        FactoryXaver fcx = new FactoryXaver(fc);
        json = JsonUtility.ToJson(fcx);
        streamWriter.Write(json);
        streamWriter.Close();

        // Save Quest
        SetPaths("Quests.json");
        savePath = currentPath;
        streamWriter = new StreamWriter(savePath);
        QuestSaver q = new QuestSaver();

        q.CurrentQuestIndex = PlayerManager.MainQuestDatabase.CurrentQuestIndex;
        q.IsDayPassed = MainQuestDatabaseSO.IsDayPassed;
        q.QuestStatus = PlayerManager.MainQuestDatabase.MainQuests.Select(x => x.QuestStatus).ToArray();

        q.NextSideQuestID = PlayerManager.SideQuestDatabase.NextSideQuestID;
        q.DayLeftBeforeNewQuest = PlayerManager.SideQuestDatabase.DayLeftBeforeNewQuest;
        q.SideQuestSavers = PlayerManager.SideQuestDatabase.SideQuests.Select(x => x.Save()).ToArray();

        json = JsonUtility.ToJson(q);
        streamWriter.Write(json);
        streamWriter.Close();

        // ---------- Save scriptable objects ----------
        /*
        for (int i = 0; i < 4; i++)
        {
            _SavedFarms[i].SetMe(PlayerManager.FarmDatabase[i]);
        }
        */

        // ---------- Save objects ----------
        // Mechs
        /*
        MechChromo[] loaded = PlayerManager.FarmDatabase.SelectMany(x => x.MechChromos).ToArray();
        MechChromo[] saved = Resources.LoadAll<MechChromo>(mechResourcePath);
        Debug.Log("LOADED: " + loaded.Count() + " SAVED: " + saved.Length);
        Debug.Log("UNSAVED: " + loaded.Except(saved).Count() + " UNLOADED: " + saved.Except(loaded).Count());
        */
        // Side quest
        /*
        SideQuestSO[] loadedSideQuest = PlayerManager.SideQuestDatabase.GetAllQuest().ToArray();
        SideQuestSO[] savedSideQuest = Resources.LoadAll<SideQuestSO>(sideQuestResourcePath);
        */
        // Save new items
        /*
        foreach (var item in loaded.Except(saved))
        {
            AssetDatabase.CreateAsset(item, Path.Combine(mechAssetsPath, item.ID.ToString() + ".asset"));
        }
        */
        /*
        foreach (var currentQuest in loadedSideQuest.Except(savedSideQuest))
        {
            AssetDatabase.CreateAsset(currentQuest, Path.Combine(sideQuestAssetsPath, currentQuest.ID.ToString() + ".asset"));
        }
        */
        // Delete unused items
        /*
        foreach (var item in saved.Except(loaded))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
        }
        */
        /*
        foreach (var oldQuest in savedSideQuest.Except(loadedSideQuest))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldQuest));
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        */
        // Debug.Log(json);
    }

    // load file
    public virtual void LoadFromFile()
    {
        // ---------- Load json ----------
        SetPaths("Save.json");
        string loadPath = currentPath;

        StreamReader streamReader = new StreamReader(loadPath);
        string json = streamReader.ReadToEnd();
        streamReader.Close();

        Saver loadData = JsonUtility.FromJson<Saver>(json);
        LoadJson(loadData);

        // Load Farm
        SetPaths("Farms.json");
        loadPath = currentPath;

        streamReader = new StreamReader(loadPath);
        json = streamReader.ReadToEnd();
        streamReader.Close();

        FarmXaver fx = JsonUtility.FromJson<FarmXaver>(json);

        for (int i = 0; i < PlayerManager.FarmDatabase.Length; i++)
        {
            PlayerManager.FarmDatabase[i].Load(fx.FarmSavers[i]);
        }

        // Load Factory
        SetPaths("Factories.json");
        loadPath = currentPath;

        streamReader = new StreamReader(loadPath);
        json = streamReader.ReadToEnd();
        streamReader.Close();

        FactoryXaver fcx = JsonUtility.FromJson<FactoryXaver>(json);

        for (int i = 0; i < PlayerManager.FactoryDatabase.Length; i++)
        {
            PlayerManager.FactoryDatabase[i].Load(fcx.FacSavers[i]);
        }

        // Load Quest
        SetPaths("Quests.json");
        loadPath = currentPath;

        streamReader = new StreamReader(loadPath);
        json = streamReader.ReadToEnd();
        streamReader.Close();

        QuestSaver q = JsonUtility.FromJson<QuestSaver>(json);

        PlayerManager.MainQuestDatabase.Load(q.CurrentQuestIndex, q.IsDayPassed, q.QuestStatus);

        // ---------- Load scriptable objects ----------
        /*
        for (int i = 0; i < _SavedFarms.Length; i++)
        {
            PlayerManager.FarmDatabase[i].SetMe(_SavedFarms[i]);
        }
        */

    }

    // trigger reset event
    public void ResetGame()
    {
        Debug.Log("PLS RESET");
        // delete mech chromos
        /*
        MechChromo[] deleteMe = Resources.LoadAll<MechChromo>("Mechs");
        foreach (MechChromo mechChromoSO in deleteMe)
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mechChromoSO));
        */
        // Delete side quests
        /*
        SideQuestSO[] oldSideQuests = Resources.LoadAll<SideQuestSO>(sideQuestResourcePath);
        foreach (SideQuestSO quest in oldSideQuests)
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(quest));
        */
        OnReset?.Invoke();
    }
}