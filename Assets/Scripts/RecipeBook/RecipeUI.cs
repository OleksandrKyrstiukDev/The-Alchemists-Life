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

        // RIGHT SIDE
        nameText.text = recipe.displayName;
        descriptionText.text = recipe.description;

        // LEFT SIDE
        instructionText.text = recipe.instructions;

        // Clear old
        foreach (var item in spawnedIngredients)
            Destroy(item.gameObject);

        spawnedIngredients.Clear();

        // Ingredients
        foreach (var ing in recipe.requiredIngredients)
        {
            var ui = Instantiate(ingredientPrefab, ingredientContainer);

            // ÂÀÆËÈÂÎ: çàëåæèòü â³ä RecipeIngredient
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