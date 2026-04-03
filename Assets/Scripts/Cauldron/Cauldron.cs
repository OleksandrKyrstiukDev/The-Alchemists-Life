using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CauldronPhase { Idle, Prep, Brew }

public struct PrepResult { 
    public float riskMultiplier; 
    public float temperatureBias; 
    public int stirBias; 
    public float stabilityBonus; 
    public float prepTime; 
    public float avgPrepTemperature; 
    public static PrepResult Neutral => new PrepResult { riskMultiplier = 1f, temperatureBias = 0f, stirBias = 0, stabilityBonus = 0f };
}

public class Cauldron : MonoBehaviour, IIngredientReceiver, IBrewFinishReceiver
{
    [Header("Recipes")]
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private TitleGeneration titleGeneration;
    [SerializeField] private Feedback feedbackSystem;
    private RecipeMatchResult currentMatch;

    private readonly List<IngredientData> ingredients = new();
    private float extraPenalty;
    private bool hasBrewed;
    [SerializeField] private float defaultPrepMaxTemp = 40f;
    [SerializeField] private GameObject potionPrefab;
    [SerializeField] private Transform potionSpawnPoint;
    [SerializeField] private PlayerUsePotion playerUsePotion;

    public CauldronPhase CurrentPhase { get; private set; } = CauldronPhase.Idle;

    public static IngredientTag GetDominantTag(List<IngredientData> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
            return IngredientTag.None;

        var tags = ingredients.Where(i => i != null && i.tags != null).SelectMany(i => i.tags).ToList();

        if (tags.Count == 0)
            return IngredientTag.None;

        return tags.GroupBy(tag => tag).OrderByDescending(g => g.Count()).First().Key;
    }

    public float GetPrepMaxTemperature()
    {
        if (currentMatch.recipe == null)
            return 50f;

        var prep = currentMatch.recipe.phases[0];
        return prep.optimalTemperature + prep.temperatureTolerance;
    }

    public void OnBrewingStarted()
    {
        CurrentPhase = CauldronPhase.Prep;
    }

    public void OnBrewingFinished()
    {
        CurrentPhase = CauldronPhase.Idle;
    }


    public CauldronUI UI { get; private set; }

    public void RegisterUI(CauldronUI ui)
    {
        UI = ui;
    }

    // -----------------------------
    // Ingredient input
    // -----------------------------
    public bool CanReceive(IngredientData data) => data != null;

    public void Receive(IngredientData data)
    {
        if (data == null || hasBrewed)
            return;

        ingredients.Add(data);
    }

    // -----------------------------
    // Brew entry point
    // -----------------------------
    public void FinishBrew()
    {
        if (UI == null) return;
        UI.Finish();
    }

    public void Brew(
     float finalTemperature,
     int stirCount,
     float greenTime,
     float yellowTime,
     float redTime
 )
    {
        if (hasBrewed || ingredients.Count == 0)
            return;

        hasBrewed = true;

        // 1️⃣ Знаходимо рецепт
        currentMatch = recipeManager.MatchRecipe(ingredients);

        bool isExperimental = currentMatch.isExperimental;
        RecipeObject recipe = currentMatch.recipe;

        // 2️⃣ Готуємо фази
        BrewingPhase prepPhase = null;
        BrewingPhase brewPhase = null;

        if (!isExperimental && recipe != null)
        {
            prepPhase = recipe.phases[0];
            brewPhase = recipe.phases[1];
        }

        // 3️⃣ PrepResult з UI
        PrepResult prep = new PrepResult
        {
            prepTime = UI.PrepTime,
            avgPrepTemperature = UI.AveragePrepTemperature
        };

        // 4️⃣ Feedback + штрафи
        List<BrewFeedback> feedback = new List<BrewFeedback>();
        float extraPenalty = 0f;

        // ----------------------------
        // 🔴 ШТРАФ ЗА ЗАЙВІ ІНГРЕДІЄНТИ
        // ----------------------------
        if (!isExperimental && recipe != null)
        {
            int expected = recipe.requiredIngredients.Count;
            int actual = ingredients.Count;

            int overflow = Mathf.Max(0, actual - expected);

            if (overflow > 0)
            {
                float overflowSeverity = Mathf.Clamp01(overflow / (float)expected);

                extraPenalty += overflow * 0.15f;
                feedback.Add(new BrewFeedback
                {
                    type = BrewMistakeType.UnstableIngredients,
                    severity = overflowSeverity
                });
            }
        }
        else
        {
            // експеримент → легкий хаос
            extraPenalty += ingredients.Count * 0.1f;
        }

        // ----------------------------
        // 🔴 ОСНОВНА ОЦІНКА
        // ----------------------------
        BrewResult result = CauldronProcess.Evaluate(
            ingredients,
            feedback,
            prepPhase,
            brewPhase,
            finalTemperature,
            stirCount,
            greenTime,
            yellowTime,
            redTime,
            prep,
            extraPenalty,
            currentMatch.confidence
        );

        // ----------------------------
        // 📣 ПОКАЗ ФІДБЕКУ
        // ----------------------------
        BrewResultData resultData = new BrewResultData
        {
            result = result,
            feedback = feedback
        };

        UI.ShowFeedback(resultData);

        // ----------------------------
        // 🧪 КОНТЕКСТ ДЛЯ ІМЕНІ
        // ----------------------------
        bool underheated = feedback.Any(f => f.type == BrewMistakeType.Underheated);
        bool overheated = feedback.Any(f => f.type == BrewMistakeType.Overheated);

        var context = new BrewNameContext
        {
            result = result,
            avgStability = SafeAverage(ingredients, i => i.stability, 1f),
            avgToxicity = SafeAverage(ingredients, i => i.toxicity, 0f),
            avgTemperature = finalTemperature,

            underheated = underheated,
            overheated = overheated,

            dominantTag = GetDominantTag(ingredients),

            recipeName = currentMatch.recipe != null
        ? currentMatch.recipe.displayName
        : null,

            isExperimental = currentMatch.isExperimental
        };


        string potionName =
            titleGeneration != null
                ? titleGeneration.Generate(context)
                : "Невідоме зілля";

        Debug.Log($"[PLAYER POTION NAME] {potionName}");
        LogFinalReport(result, prep);

        SpawnPotion(result, potionName);

        // ----------------------------
        // 🧹 RESET
        // ----------------------------
        ResetCauldron();
    }


    public static float SafeAverage<T>(
    IEnumerable<T> source,
    System.Func<T, float> selector,
    float fallback = 0f
)
    {
        if (source == null) return fallback;

        var list = source as ICollection<T> ?? source.ToList();
        return list.Count == 0 ? fallback : list.Average(selector);
    }

    private void LogFinalReport(BrewResult result, PrepResult prep)
    {
        Debug.Log("===== BREW REPORT =====");
        Debug.Log($"Result: {result}");
        Debug.Log($"Recipe: {currentMatch.recipe.displayName}");
        Debug.Log($"Confidence: {currentMatch.confidence:0.00}");
        Debug.Log($"PrepTime: {prep.prepTime:0.0}s");
        Debug.Log($"AvgPrepTemp: {prep.avgPrepTemperature:0.0}°C");
        Debug.Log($"ExtraPenalty: {extraPenalty:0.00}");

        foreach (var ing in ingredients)
            Debug.Log($"{ing.displayName} | S:{ing.stability} T:{ing.toxicity}");
    }

    private void ResetCauldron()
    {
        ingredients.Clear();
        extraPenalty = 0f;
        hasBrewed = false;
    }

    public void UpdatePhase(float temperature)
    {
        if (CurrentPhase == CauldronPhase.Idle)
            return;

        float prepMax = defaultPrepMaxTemp;

        // якщо рецепт вже відомий — використовуємо його
        if (currentMatch.recipe != null && currentMatch.recipe.phases.Count > 0)
        {
            var prep = currentMatch.recipe.phases[0];
            prepMax = prep.optimalTemperature + prep.temperatureTolerance;
        }

        CurrentPhase = temperature < prepMax ? CauldronPhase.Prep : CauldronPhase.Brew;
    }

    private void SpawnPotion(BrewResult result, string potionName)
    {
        if (playerUsePotion == null)
        {
            Debug.LogError("Cauldron: PlayerUsePotion not assigned!");
            return;
        }

        PotionPurpose purpose = DeterminePurpose();

        Debug.Log($"[Cauldron] Potion purpose: {purpose}");

        BrewedPotion brewedPotion = new BrewedPotion
        {
            data = new BrewedPotionData
            {
                name = potionName,
                result = result,
                purpose = purpose
            },
            prefab = potionPrefab
        };

        playerUsePotion.GivePotion(brewedPotion);
    }

    PotionPurpose DeterminePurpose()
    {
        var tag = GetDominantTag(ingredients);

        return tag switch
        {
            IngredientTag.Fire => PotionPurpose.Heating,
            IngredientTag.Healing => PotionPurpose.Healing,
            _ => PotionPurpose.None
        };
    }
}