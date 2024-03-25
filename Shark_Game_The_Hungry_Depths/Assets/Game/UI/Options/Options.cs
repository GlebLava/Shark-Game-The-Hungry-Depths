using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;
    public void OnLanguageSelected(int index)
    {
        string language = languageDropdown.options[index].text;
        LocalizationManager.Language enumLanguage = (LocalizationManager.Language)Enum.Parse(typeof(LocalizationManager.Language), language);
        LocalizationManager.instance.SetLanguage(enumLanguage);
    }

    private void Start()
    {
        for (int i = 0; i < languageDropdown.options.Count; i++)
        {
            TMP_Dropdown.OptionData option = languageDropdown.options[i];
            if (option.text == LocalizationManager.currentLanguage.ToString())
            {
                languageDropdown.value = i;
            }
        }
    }
}
