using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public GameObject panelPause;
    public GameObject panelOptions;
    public GameObject panelDeath;

    private void Awake()
    {
        panelPause.SetActive(false);
        panelOptions.SetActive(false);
        panelDeath.SetActive(false);

        SignalBus.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnDestroy()
    {
        SignalBus.OnPlayerDeath -= OnPlayerDeath;
    }

    public void ClickOptions()
    {
        panelPause.SetActive(false);
        panelOptions.SetActive(!panelOptions.activeSelf);

        if (panelOptions.activeSelf)
        {
            SignalBus.OnPauseGameInvoke();
        }
        else
        {
            SignalBus.OnUnpauseGameInvoke();
        }

    }

    public void ClickPause()
    {
        panelOptions.SetActive(false);
        panelPause.SetActive(!panelPause.activeSelf);

        if (panelPause.activeSelf)
        {
            SignalBus.OnPauseGameInvoke();
        }
        else
        {
            SignalBus.OnUnpauseGameInvoke();
        }
    }

    private void OnPlayerDeath()
    {
        panelDeath.SetActive(true);
    }

}
