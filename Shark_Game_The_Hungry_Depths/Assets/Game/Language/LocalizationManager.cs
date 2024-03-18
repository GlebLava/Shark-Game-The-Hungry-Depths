using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public delegate void LanguageChangedDelegate(Language language);

    public static event LanguageChangedDelegate OnLanguageChangedEvent;

    public enum Language
    {
        English,
        Deutsch
    }

    public static Language currentLanguage = Language.Deutsch;

    public static LocalizationManager instance;

    Dictionary<Language, Dictionary<string, string>> languageToIdToText;




    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        OnLanguageChangedEvent?.Invoke(language);
    }

    public string GetStringForId(string id)
    {
        return languageToIdToText[currentLanguage][id];
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Add languages
        languageToIdToText = new Dictionary<Language, Dictionary<string, string>>();
        languageToIdToText.Add(Language.English, new Dictionary<string, string>());
        languageToIdToText.Add(Language.Deutsch, new Dictionary<string, string>());


        // Add IDs
        // Main Menu
        AddID("PLAY", "PLAY", "START");
        AddID("Level Select", "Level Select", "Level Auswahl");
        AddID("Shark Shop", "Shark Shop", "Shark Shop");
        AddID("Gadget Shop", "Gadget Shop", "Gadget Shop");

        // Options Menu
        AddID("Volume Music", "Volume Music", "Volumen Musik");
        AddID("Volume Effects", "Volume Effects", "Volumen Effekte");
        AddID("Volume Effects", "Volume Effects", "Volumen Effekte");
        AddID("Graphics Quality", "Graphics Quality", "Graphik Qualität");
        AddID("Language", "Language", "Sprache");
    }

    private void AddID(string id, string textEnglish, string textGerman)
    {
        languageToIdToText[Language.English][id] = textEnglish;
        languageToIdToText[Language.Deutsch][id] = textGerman;
    }
}
