using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PausePanel : MonoBehaviour
{
    public Canvas parentCanvas;

    public TMP_Text coinsCollectedInLevel;
    public TMP_Text coinsCollectedTotal;

    public void ClickReturnToMainMenu()
    {
        string text = LocalizationManager.instance.GetStringForId("Popup.ReturnMainMenuText");

        PopupManager.instance.DisplayPopupOkCancel(parentCanvas, text, ReturnToMainMenu, null);
    }

    private void ReturnToMainMenu()
    {
        SignalBus.OnUnpauseGameInvoke();
        GameManager.instance.ReturnToEntryScene(0);
    }

    private void OnEnable()
    {
        coinsCollectedInLevel.text = LevelManager.instance.coinsCollectedThisRun.ToString();
        int coinsTotal = LevelManager.instance.coinsCollectedThisRun + GameManager.instance.gameData.coinsOwned;
        coinsCollectedTotal.text = coinsTotal.ToString();
    }

}
