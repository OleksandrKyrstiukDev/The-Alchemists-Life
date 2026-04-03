using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct BrewNameContext
{
    public BrewResult result;

    public float avgStability;
    public float avgToxicity;
    public float avgTemperature;

    public bool overheated;
    public bool underheated;

    public IngredientTag dominantTag;

    // 🔥 НОВЕ
    public string recipeName;     // displayName рецепта
    public bool isExperimental;   // чи це не рецепт
}



public class TitleGeneration : MonoBehaviour
{
    // ===============================
    // PUBLIC API
    // ===============================
    public static IngredientTag GetDominantTag(List<IngredientData> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
            return IngredientTag.None; // fallback для порожніх інгредієнтів

        var allTags = ingredients.SelectMany(i => i.tags).ToList();

        if (allTags.Count == 0)
            return IngredientTag.None;

        return allTags
            .GroupBy(tag => tag)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }


    public string Generate(BrewNameContext ctx)
    {
        string quality = GetQualityPrefix(ctx);
        string condition = GetConditionPrefix(ctx);

        string baseName =
            !ctx.isExperimental && !string.IsNullOrEmpty(ctx.recipeName)
                ? ctx.recipeName
                : GetBaseName(ctx.dominantTag);

        return $"{quality} {condition} {baseName}"
            .Trim()
            .Replace("  ", " ");
    }


    // ===============================
    // QUALITY (Perfect / Good / Fail)
    // ===============================

    private string GetQualityPrefix(BrewNameContext ctx)
    {
        return ctx.result switch
        {
            BrewResult.Perfect => "Чисте",
            BrewResult.Good => "Слабке",
            BrewResult.Fail => "Каламутне",
            _ => ""
        };
    }

    // ===============================
    // CONDITIONS (toxicity / stability / temp)
    // ===============================

    private string GetConditionPrefix(BrewNameContext ctx)
    {
        List<string> parts = new();

        if (ctx.avgStability < 1.0f)
            parts.Add("Нестабільне");

        if (ctx.avgToxicity > 0.8f)
            parts.Add("Отруйне");
        else if (ctx.avgToxicity > 0.5f)
            parts.Add("Токсичне");

        if (ctx.overheated)
            parts.Add("Перегріте");
        else if (ctx.underheated)
            parts.Add("Сире");

        return string.Join(" ", parts);
    }

    // ===============================
    // BASE NAME (by dominant tag)
    // ===============================

    private string GetBaseName(IngredientTag tag)
    {
        return tag switch
        {
            IngredientTag.Water => "очищення води",
            IngredientTag.Healing => "зілля зцілення",
            IngredientTag.Poison => "отрута",
            IngredientTag.Fire => "вогняний настій",
            IngredientTag.Ice => "крижаний екстракт",
            IngredientTag.Energy => "енергетичний настій",
            IngredientTag.Chaos => "хаотичний еліксир",
            IngredientTag.Nature => "природний відвар",
            IngredientTag.Shadow => "темний настій",
            IngredientTag.None => "невідоме зілля",
            _ => "невідоме зілля"
        };
    }

}
