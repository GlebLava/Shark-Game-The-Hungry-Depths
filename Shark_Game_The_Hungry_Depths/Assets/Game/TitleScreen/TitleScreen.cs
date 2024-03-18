using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject optionsScreen;
    public GameObject sharkShopScreen;

    public GameObject returnToTitleScreenButton;

    private void Awake()
    {
        returnToTitleScreenButton.SetActive(false);
        AllDisable();
        titleScreen.SetActive(true);
    }

    public void ClickOptionsButton()
    {
        AllDisable();

        titleScreen.SetActive(optionsScreen.activeSelf); // Because its a toggle
        optionsScreen.SetActive(!optionsScreen.activeSelf); // Because its a toggle
        returnToTitleScreenButton.SetActive(true);
    }

    public void ClickReturnToTitleScreen()
    {
        AllDisable();
        titleScreen.SetActive(true);
        returnToTitleScreenButton.SetActive(false);
    }

    public void ClickSharkShop()
    {
        AllDisable();
        sharkShopScreen.SetActive(true);
        returnToTitleScreenButton.SetActive(true);
    }

    private void AllDisable()
    {
        titleScreen.SetActive(false);
        optionsScreen.SetActive(false);
        sharkShopScreen.SetActive(false);
    }


}
