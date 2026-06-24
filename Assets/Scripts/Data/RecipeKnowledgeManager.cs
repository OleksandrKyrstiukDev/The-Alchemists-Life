using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeKnowledgeManager : MonoBehaviour
{
    public static RecipeKnowledgeManager Instance;

    private Dictionary<string, RecipeKnowledgeData>
        knowledge = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterMistake(
        RecipeObject recipe,
        BrewMistakeType mistake)
    {
        if (!knowledge.TryGetValue(
            recipe.id,
            out var recipeData))
        {
            recipeData = new RecipeKnowledgeData
            {
                recipeId = recipe.id
            };

            knowledge.Add(recipe.id, recipeData);
        }

        var counter = recipeData.mistakeCounters
            .FirstOrDefault(x => x.mistakeType == mistake);

        if (counter == null)
        {
            counter = new MistakeCounterData
            {
                mistakeType = mistake,
                count = 0
            };

            recipeData.mistakeCounters.Add(counter);
        }

        counter.count++;

        CheckNotes(recipe, recipeData, mistake);
    }

    private void CheckNotes(
        RecipeObject recipe,
        RecipeKnowledgeData data,
        BrewMistakeType mistake)
    {
        foreach (var note in recipe.notes)
        {
            if (note.triggerMistake != mistake)
                continue;

            int count = data.mistakeCounters
                .First(x => x.mistakeType == mistake)
                .count;

            if (count < note.requiredOccurrences)
                continue;

            if (data.unlockedNotes.Contains(note.noteText))
                continue;

            data.unlockedNotes.Add(note.noteText);

            Debug.Log(
                $"[NOTE UNLOCKED] {recipe.displayName}\n{note.noteText}"
            );
        }
    }

    public List<string> GetUnlockedNotes(
        RecipeObject recipe)
    {
        if (!knowledge.TryGetValue(
            recipe.id,
            out var data))
        {
            return new List<string>();
        }

        return data.unlockedNotes;
    }
}