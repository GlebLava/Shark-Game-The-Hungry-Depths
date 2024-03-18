using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsDisplay : MonoBehaviour
{
    public TMP_Text text;

    private void Awake()
    {
        SignalBus.OnCoinsAmountChanged += DisplayCoins;
    }

    private void OnDestroy()
    {
        SignalBus.OnCoinsAmountChanged -= DisplayCoins;
    }

    void DisplayCoins(int amount)
    {
        text.text = amount.ToString() + " x";
    }
}
