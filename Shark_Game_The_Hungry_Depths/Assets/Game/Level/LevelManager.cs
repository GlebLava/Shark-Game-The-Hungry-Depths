using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Sits in every level and manages the whole level. 
/// From loading everything in coroutines and setting everything up
/// to loading the after death screen. It keeps track of how many
/// coins the player got and how many fish they ate and etc.-...
/// </summary>

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Camera mainCamera;
    public AquariumBuilder aquariumBuilder;
    public GameObject cameraRotator;
    public GameObject spawnPoint;

    [HideInInspector]
    public GameObject player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("This should not be able to happen!" +
                " Found two LevelManager instances!" +
                " Only one per level allowed and loading two levels at once is not allowed either!", this);
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadTheLevel();
        // This needs to happen in start.
        // It cannot happen in awake because
        // awake still counts towards loading of the scene
        // when using LoadSceneAsync()
        SignalBus.OnLevelFinishedLoadingInvoke();

        SignalBus.OnPauseGame += PauseGame;
        SignalBus.OnUnpauseGame += UnpauseGame;

    }

    private void OnDestroy()
    {
        SignalBus.OnPauseGame -= PauseGame;
        SignalBus.OnUnpauseGame -= UnpauseGame;
    }

    private void LoadTheLevel()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        player = new GameObject("Player");
        player.transform.position = spawnPoint.transform.position;
        var playerController = player.AddComponent<PlayerController>();
        var rb = player.AddComponent<Rigidbody>();
        player.AddComponent<SphereCollider>().radius = 0.5f;

        rb.useGravity = false;

        playerController.Setup(mainCamera, cameraRotator, aquariumBuilder.transform, rb, GameManager.instance.sharkScriptableObjects.Find((s) => s.name == GameManager.instance.gameData.currentShark));
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1;
    }

}
