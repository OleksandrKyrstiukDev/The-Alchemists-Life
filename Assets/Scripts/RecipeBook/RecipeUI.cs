using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeUI : MonoBehaviour
{
    [Header("Right Side")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Left Side")]
    [SerializeField] private Transform ingredientContainer;
    [SerializeField] private IngredientUI ingredientPrefab;
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Notes")]
    [SerializeField] private TextMeshProUGUI notesText;

    [Header("Icon")]
    [SerializeField] private Image recipeIcon;

    [Header("Recipes")]
    [SerializeField] private RecipeObject[] recipes;

    private int currentIndex;
    private readonly List<IngredientUI> spawnedIngredients = new();

    private void Start()
    {
        if (recipes == null || recipes.Length == 0)
        {
            Debug.LogWarning("[RecipeUI] No recipes assigned");
            return;
        }

        ShowRecipe(0);
    }

    public void ShowRecipe(int index)
    {
        if (recipes == null || recipes.Length == 0)
            return;

        currentIndex = Mathf.Clamp(index, 0, recipes.Length - 1);

        RecipeObject recipe = recipes[currentIndex];

        if (notesText != null)
            notesText.text = "";

        ClearIngredients();

        PlayerStats stats = FindFirstObjectByType<PlayerStats>();

        bool unlocked =
            stats != null &&
            stats.CurrentTier >= recipe.requiredTier;

        if (!unlocked)
        {
            ShowLockedRecipe(recipe);
            return;
        }

        ShowUnlockedRecipe(recipe);
    }

    private void ShowLockedRecipe(RecipeObject recipe)
    {
        nameText.text = "⚗ Невідома формула";

        descriptionText.text =
            "Частина сторінки затерта алхімічною печаткою.";

        instructionText.text =
            $"Потрібна репутація: {recipe.requiredTier}";

        if (notesText != null)
        {
            notesText.text =
                "━━━━━━━━━━━━━━\n" +
                "Особисті нотатки\n" +
                "━━━━━━━━━━━━━━\n\n" +
                "Поки що недоступно.";
        }

        if (recipeIcon != null)
        {
            recipeIcon.sprite = recipe.icon;
            recipeIcon.color = new Color(0f, 0f, 0f, 0.8f);
        }
    }

    private void ShowUnlockedRecipe(RecipeObject recipe)
    {
        nameText.text = recipe.displayName;
        descriptionText.text = recipe.description;
        instructionText.text = recipe.instructions;

        if (recipeIcon != null)
        {
            recipeIcon.sprite = recipe.icon;
            recipeIcon.color = Color.white;
        }

        foreach (var ingredient in recipe.requiredIngredients)
        {
            IngredientUI ui =
                Instantiate(
                    ingredientPrefab,
                    ingredientContainer
                );

            ui.Setup(ingredient);

            spawnedIngredients.Add(ui);
        }

        UpdateNotes(recipe);
    }

    private void UpdateNotes(RecipeObject recipe)
    {
        if (notesText == null)
            return;

        notesText.text =
            "━━━━━━━━━━━━━━\n" +
            "Особисті нотатки\n" +
            "━━━━━━━━━━━━━━";

        if (RecipeKnowledgeManager.Instance == null)
        {
            notesText.text +=
                "\n\nСистема нотаток недоступна.";
            return;
        }

        List<string> unlockedNotes =
            RecipeKnowledgeManager.Instance
                .GetUnlockedNotes(recipe);

        if (unlockedNotes == null ||
            unlockedNotes.Count == 0)
        {
            notesText.text +=
                "\n\nПоки що записів немає.";
            return;
        }

        foreach (string note in unlockedNotes)
        {
            notesText.text +=
                "\n\n✎ " + note;
        }
    }

    private void ClearIngredients()
    {
        foreach (var item in spawnedIngredients)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        spawnedIngredients.Clear();
    }

    public void NextRecipe()
    {
        if (currentIndex >= recipes.Length - 1)
            return;

        ShowRecipe(currentIndex + 1);
    }

    public void PrevRecipe()
    {
        if (currentIndex <= 0)
            return;

        ShowRecipe(currentIndex - 1);
    }

    public void RefreshCurrentRecipe()
    {
        ShowRecipe(currentIndex);
    }
}