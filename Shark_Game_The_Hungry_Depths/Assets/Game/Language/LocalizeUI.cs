using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocalizeUI : MonoBehaviour
{
    public string localizeID;

    
    void Start()
    {
        DoTheThing(LocalizationManager.Language.English);
        LocalizationManager.OnLanguageChangedEvent += DoTheThing;
    }

    void DoTheThing(LocalizationManager.Language language)
    {
        string text = LocalizationManager.instance.GetStringForId(localizeID);

        var tmp_text = GetComponent<TMP_Text>();
        if (tmp_text != null) tmp_text.text = text;

        var normal_text = GetComponent<Text>();
        if (normal_text != null) normal_text.text = text;
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChangedEvent += DoTheThing;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChangedEvent -= DoTheThing;
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChangedEvent -= DoTheThing;
    }

}
