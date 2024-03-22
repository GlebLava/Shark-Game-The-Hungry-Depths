using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AfterGameScreen : MonoBehaviour
{
    public TMP_Text coinsCollected;
    public TMP_Text coinsTotal;

    public void Setup(int coinsCollectedThisRun)
    {
        coinsCollected.text = coinsCollectedThisRun.ToString();
        coinsTotal.text = (coinsCollectedThisRun + GameManager.instance.gameData.coinsOwned).ToString();
    }

    public void ClickReturnToMainMenu()
    {
        GameManager.instance.ReturnToEntryScene(0);
    }
}
