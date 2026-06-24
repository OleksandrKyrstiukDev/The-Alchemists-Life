using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(
    fileName = "LocalizationData",
    menuName = "Localization/Data"
)]
public class LocalizationData : ScriptableObject
{
    public List<LocalizationEntry> entries;
}


[Serializable]
public class LocalizationEntry
{
    public string key;


    [TextArea(2, 5)]
    public string ukrainian;


    [TextArea(2, 5)]
    public string english;
}