using System.Collections.Generic;

public enum RecipeAddResult
{
    Allowed,
    ExtraIngredient,     // зайвий, але дозволений
    DuplicateIngredient, // забагато одного
    ForbiddenIngredient  // взагалі не з рецепту
}

public enum RecipeCheckResult
{
    Ok,
    MissingIngredient,
    ExtraIngredient
}


public static class RecipeValidator
{
    public static RecipeAddResult CanAddIngredient(
        List<IngredientData> current,
        RecipeObject recipe,
        IngredientData incoming
    )
    {
        if (recipe == null || incoming == null)
            return RecipeAddResult.ForbiddenIngredient;

        // 1. Перевіряємо чи інгредієнт взагалі є в рецепті
        RecipeIngredient req = null;

        foreach (var r in recipe.requiredIngredients)
        {
            if (r.ingredient == incoming)
            {
                req = r;
                break;
            }
        }

        // ❌ Взагалі не з рецепту
        if (req == null)
            return RecipeAddResult.ForbiddenIngredient;

        // 2. Скільки вже додано такого інгредієнта
        int currentAmount = 0;
        foreach (var ing in current)
            if (ing == incoming)
                currentAmount++;

        // ⚠️ Забагато цього інгредієнта
        if (currentAmount >= req.amount)
            return RecipeAddResult.DuplicateIngredient;

        // ✅ Все ок
        return RecipeAddResult.Allowed;
    }

    public static RecipeCheckResult ValidateFinal(
    List<IngredientData> input,
    RecipeObject recipe
)
    {
        // 1. Перевірка: чи вистачає кожного інгредієнта
        foreach (var req in recipe.requiredIngredients)
        {
            int count = 0;
            foreach (var ing in input)
                if (ing == req.ingredient)
                    count++;

            if (count < req.amount)
                return RecipeCheckResult.MissingIngredient;
        }

        // 2. Перевірка: чи є зайві
        foreach (var ing in input)
        {
            bool allowed = false;
            foreach (var req in recipe.requiredIngredients)
            {
                if (req.ingredient == ing)
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed)
                return RecipeCheckResult.ExtraIngredient;
        }

        return RecipeCheckResult.Ok;
    }

}
