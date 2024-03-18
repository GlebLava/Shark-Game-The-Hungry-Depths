using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject optionsScreen;

    public void ClickOptionsButton()
    {
        titleScreen.SetActive(optionsScreen.activeSelf);
        optionsScreen.SetActive(!optionsScreen.activeSelf);
    }


}
