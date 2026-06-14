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
            // === Головні типи зіль (Ефекти) ===
            IngredientTag.Poison => "зілля отруєння",
            IngredientTag.Healing => "зілля зцілення",
            IngredientTag.Heating => "зілля зігрівання",
            IngredientTag.Clean => "зілля очищення",
            IngredientTag.WallRepair => "зілля для ремонту стін",
            IngredientTag.ToolRepair => "зілля для інструментів",
            IngredientTag.WeatherControl => "зілля керування погодою",
            IngredientTag.Tracking => "зілля слідопита",
            IngredientTag.Gardening => "садове зілля",
            IngredientTag.Purification => "зілля фільтрації",
            IngredientTag.Recovery => "зілля відновлення сил",
            IngredientTag.Endurance => "зілля витривалості",
            IngredientTag.Social => "зілля одкровення",
            IngredientTag.Strength => "зілля фізичної сили",
            IngredientTag.Demolition => "вибухове варево",
            IngredientTag.Mutation => "мутагенне зілля",
            IngredientTag.Disguise => "зілля маскування",

            // === Фізичні стани та властивості (якщо вони домінують) ===
            IngredientTag.Base => "базове варево",
            IngredientTag.Liquid => "рідке варево",
            IngredientTag.Stable => "стабільне варево",
            IngredientTag.Unstable => "нестабільне варево",
            IngredientTag.Hot => "гаряче варево",
            IngredientTag.Sticky => "липке місиво",
            IngredientTag.Viscous => "в'язке варево",
            IngredientTag.Reactive => "реактивне хімічне зілля",
            IngredientTag.Explosive => "вибухонебезпечне зілля",
            IngredientTag.Powder => "порошкове варево",
            IngredientTag.Toxic => "токсичне варево",

            // === Елементи, джерела та матеріали ===
            IngredientTag.Water => "очищення води",
            IngredientTag.Stone => "кам'яне зілля",
            IngredientTag.Mineral => "мінеральне варево",
            IngredientTag.Stabilizer => "зілля-стабілізатор",
            IngredientTag.Catalyst => "зілля-каталізатор",
            IngredientTag.Web => "павутинне варево",
            IngredientTag.Insect => "комашине зілля",
            IngredientTag.Fire => "вогняне зілля",
            IngredientTag.Ash => "попелясте варево",
            IngredientTag.Magic => "магічне зілля",
            IngredientTag.Pollen => "пилкове зілля",
            IngredientTag.Herb => "трав'яне зілля",
            IngredientTag.Bitter => "гірке зілля",
            IngredientTag.Berry => "ягідне варево",
            IngredientTag.Time => "зілля з ефектом часу",
            IngredientTag.Root => "кореневе варево",
            IngredientTag.Dark => "темне зілля",
            IngredientTag.Spore => "спорове варево",
            IngredientTag.Mushroom => "грибне зілля",
            IngredientTag.Green => "зелене зілля",
            IngredientTag.Shadow => "тіньове зілля",
            IngredientTag.Morning => "ранкове зілля",

            // === Fallback ===
            IngredientTag.None => "невідоме зілля",
            _ => "невідоме зілля"
        };
    }
}