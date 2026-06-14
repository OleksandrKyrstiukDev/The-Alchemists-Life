using System;
using System.Collections.Generic;
using UnityEngine;

public class StorageInventory : BaseInventory
{
    public bool TakeIngredient(IngredientData ingredient, int amount = 1)
    {
        bool success = Remove(ingredient, amount);

        if (success)
            Debug.Log($"[Storage] Took {ingredient.displayName} x{amount}");

        return success;
    }

    public void AddIngredient(IngredientData ingredient, int amount = 1)
    {
        Add(ingredient, amount);
    }
}


[Serializable]
public class InventorySlot
{
    public IngredientData ingredient;
    public int amount;

    public InventorySlot(IngredientData ingredient, int amount)
    {
        this.ingredient = ingredient;
        this.amount = amount;
    }
}