using System;
using System.Collections.Generic;

[Serializable]
public class RecipeKnowledgeData
{
    public string recipeId;

    public List<MistakeCounterData> mistakeCounters =
        new();

    public List<string> unlockedNotes =
        new();
}

[Serializable]
public class MistakeCounterData
{
    public BrewMistakeType mistakeType;
    public int count;
}