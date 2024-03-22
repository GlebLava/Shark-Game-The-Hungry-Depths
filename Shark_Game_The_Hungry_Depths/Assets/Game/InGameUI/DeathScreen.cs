using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    public void ClickContinue()
    {
        SignalBus.OnPlayerDeathContinueInvoke();
    }

    public void ClickRevive()
    {
        SignalBus.OnPlayerDeathReviveInvoke();
        gameObject.SetActive(false);
    }
}
