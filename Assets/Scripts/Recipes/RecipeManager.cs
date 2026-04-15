using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct RecipeMatchResult
{
    public RecipeObject recipe;
    public float confidence;
    public bool isExperimental;

    public List<IngredientData> matchedIngredients;
    public List<IngredientData> extraIngredients;
}



public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<RecipeObject> allRecipes;

    private HashSet<RecipeObject> unlocked = new();
    private RecipeObject currentRecipe;

    private void Awake()
    {
        foreach (var r in allRecipes)
            unlocked.Add(r); 
    }

    public RecipeObject GetCurrentRecipe()
    {
        return currentRecipe;
    }

    public RecipeMatchResult MatchRecipe(List<IngredientData> ingredients)
    {
        RecipeMatchResult best = default;
        float bestScore = 0f;

        foreach (var recipe in unlocked)
        {
            var result = EvaluateMatch(recipe, ingredients);

            if (result.confidence > bestScore)
            {
                bestScore = result.confidence;
                best = result;
            }
        }

        if (best.recipe == null || best.confidence < 0.35f)
        {
            return new RecipeMatchResult
            {
                recipe = null,
                confidence = bestScore,
                isExperimental = true,
                matchedIngredients = new(),
                extraIngredients = new List<IngredientData>(ingredients)
            };
        }

        currentRecipe = best.recipe;
        return best;
    }



    private RecipeMatchResult EvaluateMatch(
     RecipeObject recipe,
     List<IngredientData> ingredients
 )
    {
        float score = 0f;
        float maxScore = 0f;

        List<IngredientData> matched = new();
        List<IngredientData> extras = new(ingredients);

        foreach (var req in recipe.requiredIngredients)
        {
            int required = req.amount;

            var matches = ingredients.Where(i =>
                i == req.ingredient ||
                (req.alternatives != null && req.alternatives.Contains(i))
            ).ToList();

            maxScore += required;

            int used = Mathf.Min(matches.Count, required);
            score += used;

            for (int i = 0; i < used; i++)
            {
                matched.Add(matches[i]);
                extras.Remove(matches[i]);
            }
        }

        int extraCount = extras.Count;
        float penalty = extraCount * 0.5f;

        float confidence = Mathf.Clamp01((score - penalty) / maxScore);

        return new RecipeMatchResult
        {
            recipe = recipe,
            confidence = confidence,
            matchedIngredients = matched,
            extraIngredients = extras,
            isExperimental = false
        };
    }
}
