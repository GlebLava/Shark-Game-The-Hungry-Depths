using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<SharkSO> sharkScriptableObjects;
    public List<GadgetSO> gadgetScriptableObjects;
    public List<LevelSelectItemSO> levelSelectItemScriptableObjects;

    public GameData gameData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // sort the scriptable objects by cost
        sharkScriptableObjects.Sort((a, b) => a.cost.CompareTo(b.cost));

        gameData = SaveSystem.LoadGameData();

        SignalBus.OnSharkPlayerChosen += (s) =>
        {
            gameData.currentShark = s;
            SaveSystem.SaveGameData(gameData);
        };

        SignalBus.OnCoinsAmountChanged += (i) =>
        {
            gameData.coinsOwned = i;
            // We do not save here. Only save coins
            // on other specific occasions
        };

        SignalBus.OnNewSharkBought += (s) =>
        {
            if (!gameData.sharksOwned.Contains(s))
                gameData.sharksOwned.Add(s);
            SaveSystem.SaveGameData(gameData);
        };

        SignalBus.OnNewGadgetBought += (s) =>
        {
            int index = gameData.gadgetsOwned.FindIndex((gadget) => gadget.name == s);
            if (index == -1)
            {
                gameData.gadgetsOwned.Add(new GadgetData() { amountOwned = 1, name = s });
            }
            else
            {
                gameData.gadgetsOwned[index].amountOwned++;
            }
            SaveSystem.SaveGameData(gameData);
        };

        SignalBus.OnLevelSelected += (s) =>
        {
            gameData.selectedLevel = s;
        };

        SignalBus.OnNewLevelBought += (s) =>
        {
            if (!gameData.levelsOwned.Contains(s))
                gameData.levelsOwned.Add(s);
            SaveSystem.SaveGameData(gameData);
        };

    }

    private void Start()
    {
        SignalBus.OnSharkPlayerChosenInvoke(gameData.currentShark);
        SignalBus.OnCoinsAmountChangedInvoke(gameData.coinsOwned);
        SignalBus.OnLevelSelectedInvoke(gameData.selectedLevel);
    }

    public void PlayGamePressed()
    {
        SignalBus.OnLevelFinishedLoading += OnLevelFinishedLoading;
        SceneManager.LoadScene("LoadingScene");
        string sceneName = levelSelectItemScriptableObjects.Find((so) => so.name == gameData.selectedLevel).level.SceneName;
        AsyncOperation loadingLevelOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    private void OnLevelFinishedLoading()
    {
        // We can only unload the loading scene if its not the active scene, so we set the current levels scene as the active scene
        string sceneName = levelSelectItemScriptableObjects.Find((so) => so.name == gameData.selectedLevel).level.SceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        SignalBus.OnLevelFinishedLoading -= OnLevelFinishedLoading;
        SceneManager.UnloadSceneAsync("LoadingScene"); // Idk the serialized UnloadScene is deprecated
    }





#if UNITY_EDITOR

    [MenuItem("GameManager/Save Game")]
    static void EditorSaveGame()
    {
        SaveSystem.SaveGameData(instance.gameData);
    }

    [MenuItem("GameManager/Print Game Data")]
    static void EditorPrintGameData()
    {
        Debug.Log(instance.gameData.currentShark);
        Debug.Log(instance.gameData.coinsOwned);

        foreach (var shark in instance.gameData.sharksOwned)
        {
            Debug.Log(shark);
        }

        foreach (var level in instance.gameData.levelsOwned)
        {
            Debug.Log(level);
        }

        Debug.Log("Selected level: " + instance.gameData.selectedLevel);
    }

    [MenuItem("GameManager/Print Game Data Path")]
    static void EditorPrintGameDataPath()
    {
        Debug.Log(Application.persistentDataPath);
    }

    [MenuItem("GameManager/Check all Level Selects for Errors")]
    static void CheckLevelSelectsForErrors()
    {
        foreach (LevelSelectItemSO item in instance.levelSelectItemScriptableObjects)
        {
            Scene scene = SceneManager.GetSceneByName(item.level.SceneName);
            if (!scene.IsValid())
            {
                Debug.LogError($"Invalid scene in scriptable object: \"{item.name}\" with scene name: \"{scene.name}\". The scene is not added in the build settings!");
            }
        }
    }

#endif

}


[System.Serializable]
public class GameData
{
    public string currentShark = "DefaultShark";
    public string selectedLevel = "DefaultLevel";
    public int coinsOwned = 100;

    [SerializeField]
    public List<string> sharksOwned = new List<string>();
    [SerializeField]
    public List<GadgetData> gadgetsOwned = new List<GadgetData>();
    [SerializeField]
    public List<string> levelsOwned = new List<string>();

    public GameData()
    {
        // Put all stuff here that a new player owns from the start
        sharksOwned.Add("DefaultShark");
        levelsOwned.Add("DefaultLevel");
    }
}

[System.Serializable]
public class GadgetData
{
    // As in name of the scriptable object NOT the title
    public string name = "";
    public int amountOwned = 0;
}