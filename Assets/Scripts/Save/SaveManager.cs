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

    // json save path
    string path = "";
    string persistentPath = "";
    protected string currentPath;

    private void Awake()
    {
        path = Path.Combine(Application.dataPath, "Save", name);
        persistentPath = Path.Combine(Application.persistentDataPath, "Save", name);

        currentPath = persistentPath;

        if (!Directory.Exists(currentPath))
        {
            Directory.CreateDirectory(currentPath);
        }
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

        currentPath = persistentPath;
    }

    // generate struct to save
    public Saver SetJson()
    {
        Saver saveData = new Saver();

        saveData.Days = PlayerManager.CurrentDate.ToDay();
        saveData.Money = PlayerManager.Money;
        saveData.MechIDCounter = PlayerManager.MechIDCounter;
        saveData.MechCap = PlayerManager.MechCap;
        saveData.BattleRecord = PlayerManager.BattleRecord.ToList();

        // ContentChapSO
        saveData.CCLockStatus = PlayerManager.ContentChapterDatabase.Select(x => x.LockStatus).ToArray();

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
        PlayerManager.BattleRecord = s.BattleRecord.ToList();

        // ContentChapSO
        for (int i = 0; i < PlayerManager.ContentChapterDatabase.Length; i++)
        {
            PlayerManager.ContentChapterDatabase[i].Load(s.CCLockStatus[i]);
        }

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
        if (PlayerManager.SideQuestDatabase.SideQuests.Count > 0)
        {
            q.SideQuestSavers = PlayerManager.SideQuestDatabase.SideQuests.Select(x => x.Save()).ToArray();
        }
        else
        {
            q.SideQuestSavers = null;
        }

        json = JsonUtility.ToJson(q);
        streamWriter.Write(json);
        streamWriter.Close();
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
        PlayerManager.SideQuestDatabase.Load(q.NextSideQuestID, q.DayLeftBeforeNewQuest, q.SideQuestSavers);
    }

    // trigger reset event
    public void ResetGame()
    {
        Debug.Log("PLS RESET");
        OnReset?.Invoke();
    }

    // Delete all save files if possible
    public void Delete()
    {
        SetPaths("Save.json");
        string savePath = currentPath;
        if (File.Exists(savePath)) File.Delete(savePath);
        SetPaths("Farms.json");
        savePath = currentPath;
        if (File.Exists(savePath)) File.Delete(savePath);
        SetPaths("Factories.json");
        savePath = currentPath;
        if (File.Exists(savePath)) File.Delete(savePath);
        SetPaths("Quests.json");
        savePath = currentPath;
        if (File.Exists(savePath)) File.Delete(savePath);
    }
}