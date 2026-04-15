using System.Collections.Generic;

public enum RecipeAddResult
{
    Allowed,
    ExtraIngredient,    
    DuplicateIngredient, 
    ForbiddenIngredient  
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

        RecipeIngredient req = null;

        foreach (var r in recipe.requiredIngredients)
        {
            if (r.ingredient == incoming)
            {
                req = r;
                break;
            }
        }

        if (req == null)
            return RecipeAddResult.ForbiddenIngredient;

        int currentAmount = 0;
        foreach (var ing in current)
            if (ing == incoming)
                currentAmount++;

        if (currentAmount >= req.amount)
            return RecipeAddResult.DuplicateIngredient;

        return RecipeAddResult.Allowed;
    }

    public static RecipeCheckResult ValidateFinal(
    List<IngredientData> input,
    RecipeObject recipe
)
    {

        foreach (var req in recipe.requiredIngredients)
        {
            int count = 0;
            foreach (var ing in input)
                if (ing == req.ingredient)
                    count++;

            if (count < req.amount)
                return RecipeCheckResult.MissingIngredient;
        }

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
