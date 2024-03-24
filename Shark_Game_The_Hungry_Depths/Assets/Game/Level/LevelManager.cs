﻿using System.Collections;
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
    public List<Transform> boidsSpawnPoints;
    public SwarmBoidsManager swarmBoidsManager;


    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public PlayerController playerController;
    [HideInInspector]
    public int coinsCollectedThisRun = 0;



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
        // This needs to happen in start.
        // It cannot happen in awake because
        // awake still counts towards loading of the scene
        // when using LoadSceneAsync()

        // Ignore the message above since the coroutine will 
        // be to slow to happen in awake anyways
        StartCoroutine(LoadTheLevelCo());

        SignalBus.OnPauseGame += PauseGame;
        SignalBus.OnUnpauseGame += UnpauseGame;
        SignalBus.OnPlayerDeathContinue += OnPlayerDeathContinue;
    }

    private void OnDestroy()
    {
        SignalBus.OnPauseGame -= PauseGame;
        SignalBus.OnUnpauseGame -= UnpauseGame;
        SignalBus.OnPlayerDeathContinue -= OnPlayerDeathContinue;
    }

    private IEnumerator LoadTheLevelCo()
    {
        SpawnPlayer();
        SignalBus.OnLevelFinishedLoadingInvoke();
        swarmBoidsManager.Setup(1000, boidsSpawnPoints, aquariumBuilder);
        yield break;
    }

    private void SpawnPlayer()
    {
        player = new GameObject("Player");
        player.transform.SetParent(transform); // So it gets added to the right scene

        player.transform.position = spawnPoint.transform.position;
        playerController = player.AddComponent<PlayerController>();
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

    /// <summary>
    /// This is here because I want it to make the LevelManager's responsibility
    /// to pass all relevant data to the GameManager once the level is over (which 
    /// happens when the player decides to press continue after dying)
    /// </summary>
    private void OnPlayerDeathContinue()
    {
        GameManager.instance.SwitchToAfterDeathScreen(coinsCollectedThisRun);
    }
}
