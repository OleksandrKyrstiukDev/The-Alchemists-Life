using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeUI : MonoBehaviour
{
    [Header("Right Side")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    [Header("Left Side")]
    public Transform ingredientContainer;
    public IngredientUI ingredientPrefab;
    public TextMeshProUGUI instructionText;

    [Header("Icon")]
    public Image recipeIcon;

    [Header("Recipes")]
    public RecipeObject[] recipes;

   
    private int currentIndex = 0;
    private List<IngredientUI> spawnedIngredients = new List<IngredientUI>();

    private void Start()
    {
        ShowRecipe(currentIndex);
    }

    public void ShowRecipe(int index)
    {
        if (recipes.Length == 0) return;

        currentIndex = Mathf.Clamp(index, 0, recipes.Length - 1);
        var recipe = recipes[currentIndex];

        var stats = FindFirstObjectByType<PlayerStats>();

        bool unlocked =
            stats != null &&
            stats.CurrentTier >= recipe.requiredTier;

        // очистка старих інгредієнтів
        foreach (var item in spawnedIngredients)
            Destroy(item.gameObject);

        spawnedIngredients.Clear();

        if (!unlocked)
        {
            // ===== LOCKED STATE =====

            nameText.text = "⚗ Невідома формула";
            descriptionText.text =
                "Частина сторінки затерта алхімічною печаткою.";

            instructionText.text =
                $"Потрібна репутація: {recipe.requiredTier}";

            // затемнення іконки
            if (recipeIcon != null)
            {
                recipeIcon.sprite = recipe.icon;
                recipeIcon.color = new Color(0f, 0f, 0f, 0.8f);
            }

            return;
        }

        // ===== UNLOCKED STATE =====

        nameText.text = recipe.displayName;
        descriptionText.text = recipe.description;
        instructionText.text = recipe.instructions;

        if (recipeIcon != null)
        {
            recipeIcon.sprite = recipe.icon;
            recipeIcon.color = Color.white;
        }

        foreach (var ing in recipe.requiredIngredients)
        {
            var ui = Instantiate(ingredientPrefab, ingredientContainer);
            ui.Setup(ing);
            spawnedIngredients.Add(ui);
        }
    }

    public void NextRecipe()
    {
        if (currentIndex < recipes.Length - 1)
            ShowRecipe(currentIndex + 1);
    }

    public void PrevRecipe()
    {
        if (currentIndex > 0)
            ShowRecipe(currentIndex - 1);
    }
}