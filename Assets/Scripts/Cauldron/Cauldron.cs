using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CauldronPhase { Idle, Prep, Brew }

public struct PrepResult
{
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

    [Header("Visual Effects & Objects")]
    [SerializeField] private GameObject[] objectsToActivateOnBrew;
    [SerializeField] private PotionPopupUI potionPopup;
    [SerializeField] private CameraFocusController cameraFocus;
    public CauldronPhase CurrentPhase { get; private set; } = CauldronPhase.Idle;

    [Header("World Popup")]
    [SerializeField] private IngredientPopupWorld popupPrefab;
    [SerializeField] private Transform popupPoint;

    [SerializeField]
    private ParticleSystem ingredientParticles;
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

        ToggleBrewingObjects(true);
    }

    public void OnBrewingFinished()
    {
        CurrentPhase = CauldronPhase.Idle;

        // Вимикаємо всі об'єкти з масиву
        ToggleBrewingObjects(false);
    }

    // Допоміжний метод для безпечного перемикання стану об'єктів
    private void ToggleBrewingObjects(bool activate)
    {
        if (objectsToActivateOnBrew == null) return;

        foreach (GameObject obj in objectsToActivateOnBrew)
        {
            if (obj != null)
            {
                obj.SetActive(activate);
            }
        }
    }

    public CauldronUI UI { get; private set; }

    public void RegisterUI(CauldronUI ui)
    {
        UI = ui;
    }

    public bool CanReceive(IngredientData data)
    {
        return data != null && !hasBrewed;
    }

    public void Receive(IngredientData data)
    {
        if (data == null || hasBrewed)
            return;

        ingredients.Add(data);

        Debug.Log($"[CAULDRON] Received: {data.displayName}");
    }
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
        Debug.Log($"[CAULDRON] Спроба викликати Brew! hasBrewed: {hasBrewed}, Кількість інгредієнтів: {ingredients.Count}");

        if (hasBrewed || ingredients.Count == 0)
        {
            Debug.LogWarning("[CAULDRON] Brew скасовано! Або казан уже зварив, або інгредієнтів 0.");
            return;
        }

        hasBrewed = true;

        currentMatch = recipeManager.MatchRecipe(ingredients);

        bool isExperimental = currentMatch.isExperimental;
        RecipeObject recipe = currentMatch.recipe;

        BrewingPhase prepPhase = null;
        BrewingPhase brewPhase = null;

        if (!isExperimental && recipe != null)
        {
            prepPhase = recipe.phases[0];
            brewPhase = recipe.phases[1];
        }

        PrepResult prep = new PrepResult
        {
            prepTime = UI.PrepTime,
            avgPrepTemperature = UI.AveragePrepTemperature
        };

        List<BrewFeedback> feedback = new List<BrewFeedback>();
        float extraPenalty = 0f;

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
            extraPenalty += ingredients.Count * 0.1f;
        }
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


        BrewResultData resultData = new BrewResultData
        {
            result = result,
            feedback = feedback
        };

        UI.ShowFeedback(resultData);


        // ===============================
        // RECIPE KNOWLEDGE
        // ===============================

        if (currentMatch.recipe != null)
        {
            foreach (var mistake in feedback)
            {
                RecipeKnowledgeManager.Instance.RegisterMistake(
                    currentMatch.recipe,
                    mistake.type
                );
            }
        }


        // ===============================
        // NAME GENERATION
        // ===============================

        bool underheated =
            feedback.Any(f => f.type == BrewMistakeType.Underheated);

        bool overheated =
            feedback.Any(f => f.type == BrewMistakeType.Overheated);


        var context = new BrewNameContext
        {
            result = result,

            avgStability =
                SafeAverage(ingredients, i => i.stability, 1f),

            avgToxicity =
                SafeAverage(ingredients, i => i.toxicity, 0f),

            avgTemperature = finalTemperature,

            underheated = underheated,
            overheated = overheated,

            dominantTag = GetDominantTag(ingredients),

            recipeName =
                currentMatch.recipe != null
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


        if (potionPopup != null)
        {
            potionPopup.Show(potionName);
        }
        else
        {
            Debug.LogWarning("[CAULDRON] PotionPopupUI not assigned");
        }


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

        if (currentMatch.recipe != null && currentMatch.recipe.phases.Count > 0)
        {
            var prep = currentMatch.recipe.phases[0];
            prepMax = prep.optimalTemperature + prep.temperatureTolerance;
        }

        CurrentPhase = temperature < prepMax ? CauldronPhase.Prep : CauldronPhase.Brew;
    }

    private void SpawnPotion(BrewResult result, string potionName)
    {
        Debug.Log("[SpawnPotion] START");

        PotionPurpose purpose = DeterminePurpose();

        BrewedPotionData runtimeData = new BrewedPotionData
        {
            name = potionName,
            result = result,
            purpose = purpose
        };

        BrewedPotion brewedPotion = new BrewedPotion
        {
            data = runtimeData,
            prefab = potionPrefab 
        };
        CheckPotionQuest(runtimeData);
        playerUsePotion.GivePotion(brewedPotion);

        Debug.Log("[SpawnPotion] Sent to PlayerUsePotion");
    }

    PotionPurpose DeterminePurpose()
    {
        var tag = GetDominantTag(ingredients);

        return tag switch
        {
            // === ЦІЛЬОВІ ТЕГИ ЗІЛЛЯ -> МЕТА ЗІЛЛЯ ===
            IngredientTag.Heating => PotionPurpose.Heating,        // Теплий шепіт
            IngredientTag.WallRepair => PotionPurpose.Repair,         // Тріщинозбір
            IngredientTag.ToolRepair => PotionPurpose.Maintenance,    // Тиха правка (інструменти)
            IngredientTag.Poison => PotionPurpose.Poison,         // Чорний ковток
            IngredientTag.Healing => PotionPurpose.Healing,        // Лікувальні ефекти
            IngredientTag.Clean => PotionPurpose.Clean,          // Пилозгін
            IngredientTag.WeatherControl => PotionPurpose.Weather,        // Хмарний вузол
            IngredientTag.Tracking => PotionPurpose.Foraging,       // Слідолист (пошук трав)
            IngredientTag.Gardening => PotionPurpose.PlantCare,      // Садовий настрій
            IngredientTag.Purification => PotionPurpose.Purification,   // Чиста вода
            IngredientTag.Recovery => PotionPurpose.Energy,         // Збір сил (відновлення енергії)
            IngredientTag.Endurance => PotionPurpose.Stamina,        // Стійкість (витривалість)
            IngredientTag.Social => PotionPurpose.Charisma,       // Відкрите серце (харизма/соціальне)
            IngredientTag.Strength => PotionPurpose.Buff,           // Кріпкість (фізичне підсилення NPC)
            IngredientTag.Demolition => PotionPurpose.Explosion,      // Бомбарда (вибух)
            IngredientTag.Mutation => PotionPurpose.Transformation, // Чужа риса (трансформація)
            IngredientTag.Disguise => PotionPurpose.Illusion,       // Маска (ілюзія/маскування)

            // Усі інші фізичні, матеріальні теги чи None не є домінантними для мети зілля
            _ => PotionPurpose.None
        };
    }

    public bool TryAddFromInventory(PlayerInventory inventory, IngredientData ingredient)
    {
        if (inventory == null || ingredient == null)
            return false;

        if (!CanReceive(ingredient))
            return false;

        if (!inventory.Has(ingredient, 1))
            return false;

        inventory.Remove(ingredient, 1);
        Receive(ingredient);
        ShowIngredientPopup(ingredient,1);
        PlayIngredientEffect(ingredient);
        Debug.Log($"[CAULDRON] Added from inventory: {ingredient.displayName}");

        return true;
    }

    private Color GetPotionColor(BrewResult result)
    {
        switch (result)
        {
            case BrewResult.Perfect:
                return new Color(0.2f, 1f, 0.3f); // яскраво-зелений

            case BrewResult.Good:
                return new Color(0.4f, 0.8f, 1f); // блакитний

            case BrewResult.Fail:
                return new Color(0.8f, 0.4f, 0.1f); // помаранчевий

            default:
                return Color.magenta;
        }
    }

    private void CheckPotionQuest(BrewedPotionData potion)
    {
        QuestInteractable[] quests =
            FindObjectsByType<QuestInteractable>(
                FindObjectsSortMode.None
            );


        foreach (var quest in quests)
        {
            if (quest.CheckPotion(potion))
            {
                quest.Interact();

                Debug.Log(
                    $"[QUEST] Potion {potion.purpose} completed"
                );
            }
        }
    }

    private void PlayIngredientEffect(
    IngredientData ingredient)
    {
        var main = ingredientParticles.main;

        main.startColor =
            ingredient.particleColor;

        ingredientParticles.Play();

    }

    private void ShowIngredientPopup(
    IngredientData ingredient,
    int amount)
    {

        if (popupPrefab == null)
            return;


        IngredientPopupWorld popup =
            Instantiate(
                popupPrefab,
                popupPoint.position,
                Quaternion.identity
            );


        popup.Setup(
            ingredient,
            amount
        );

    }
}
