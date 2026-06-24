using System.Collections.Generic;
using UnityEngine;


public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;


    [SerializeField]
    private LocalizationData data;

    public System.Action OnLanguageChanged;

    public Language CurrentLanguage { get; private set; }


    private Dictionary<string, LocalizationEntry> table;



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        DontDestroyOnLoad(gameObject);


        CurrentLanguage =
            (Language)
            PlayerPrefs.GetInt(
                "Language",
                1
            );


        CreateTable();
    }



    private void CreateTable()
    {
        table =
            new Dictionary<string, LocalizationEntry>();


        foreach (var item in data.entries)
        {
            table.Add(
                item.key,
                item
            );
        }
    }



    public string Get(string key)
    {
        if (!table.ContainsKey(key))
        {
            Debug.LogWarning(
                "Missing key: " + key
            );

            return key;
        }


        var entry = table[key];


        if (CurrentLanguage ==
            Language.Ukrainian)
        {
            return entry.ukrainian;
        }


        return entry.english;
    }



    public void ChangeLanguage(Language language)
    {
        CurrentLanguage = language;

        PlayerPrefs.SetInt(
            "Language",
            (int)language
        );

        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke();
    }
}