using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameCoinsDisplay : MonoBehaviour
{
    public TMP_Text textCoinsCount;

    private int coinsCount = 0;
    
    private void Awake()
    {
        SignalBus.OnInGameCoinsCollected += OnInGameCoinsCollected;
    }

    private void OnDestroy()
    {
        SignalBus.OnInGameCoinsCollected -= OnInGameCoinsCollected;
    }

    private void Start()
    {
        coinsCount = LevelManager.instance.coinsCollectedThisRun;
        textCoinsCount.text = coinsCount.ToString();
    }
    void OnInGameCoinsCollected(int collected)
    {
        coinsCount += collected;
        textCoinsCount.text = coinsCount.ToString();
    }



}
