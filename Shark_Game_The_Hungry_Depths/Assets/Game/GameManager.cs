using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<SharkSO> sharkScriptableObjects;

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
    }

    private void Start()
    {
        SignalBus.OnSharkPlayerChosenInvoke(gameData.currentShark);
        SignalBus.OnCoinsAmountChangedInvoke(gameData.coinsOwned);
    }

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
    }

    [MenuItem("GameManager/Print Game Data Path")]
    static void EditorPrintGameDataPath()
    {
        Debug.Log(Application.persistentDataPath);
    }
}


[System.Serializable]
public class GameData
{
    public string currentShark = "DefaultShark";
    public int coinsOwned = 100;

    [SerializeField]
    public List<string> sharksOwned = new List<string>();

    public GameData()
    {
        // Put all stuff here that a new player owns from the start
        sharksOwned.Add("DefaultShark"); 
    }

}