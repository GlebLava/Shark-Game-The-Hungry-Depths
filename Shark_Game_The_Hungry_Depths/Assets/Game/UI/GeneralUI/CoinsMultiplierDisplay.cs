using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsMultiplierDisplay : MonoBehaviour
{
    public TMP_Text multiplierPreText;
    public TMP_Text multiplierValue;

    public Color colorNormal;
    public Color colorFrenzy;

    private void Awake()
    {
        SignalBus.OnFrenzyStart += OnFrenzyStart;   
        SignalBus.OnFrenzyEnd += OnFrenzyEnd;

        multiplierPreText.color = colorNormal;
        multiplierValue.color = colorNormal;
    }

    private void OnDestroy()
    {
        SignalBus.OnFrenzyStart -= OnFrenzyStart;
        SignalBus.OnFrenzyEnd -= OnFrenzyEnd;
    }

    void OnFrenzyStart()
    {
        multiplierPreText.fontStyle = FontStyles.Bold;
        multiplierValue.fontStyle = FontStyles.Bold;

        multiplierPreText.color = colorFrenzy;
        multiplierValue.color = colorFrenzy;

        multiplierPreText.text += "!!!";
    }

    void OnFrenzyEnd()
    {
        multiplierPreText.fontStyle = FontStyles.Normal;
        multiplierValue.fontStyle = FontStyles.Normal;

        multiplierPreText.color = colorNormal;
        multiplierValue.color = colorNormal;

        multiplierPreText.text = multiplierPreText.text.Trim('!');
    }


    // Update is called once per frame
    void Update()
    {
        if (LevelManager.instance == null) return;

        float multiplier = LevelManager.instance.coinsMultiplier;
        if (LevelManager.instance.playerController != null && LevelManager.instance.playerController.InFrenzy())
        {
            multiplier *= 5f;
        }

        multiplierValue.text = multiplier.ToString("0.#");

    }
}
