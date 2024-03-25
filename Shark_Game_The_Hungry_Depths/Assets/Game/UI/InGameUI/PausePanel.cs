using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PausePanel : MonoBehaviour
{
    public Canvas parentCanvas;

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
}
