using UnityEngine;

public class PlayerInventory : BaseInventory
{
    public bool TryUseInCauldron(IngredientData ingredient, Cauldron cauldron)
    {
        if (ingredient == null || cauldron == null)
            return false;

        if (!Has(ingredient, 1))
            return false;

        if (!cauldron.CanReceive(ingredient))
            return false;

        cauldron.Receive(ingredient);
        Remove(ingredient, 1);

        Debug.Log($"[PlayerInventory] Used {ingredient.displayName}");
        return true;
    }
}