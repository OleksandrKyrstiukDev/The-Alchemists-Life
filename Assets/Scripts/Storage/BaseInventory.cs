using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInventory : MonoBehaviour
{
    [SerializeField] protected List<InventorySlot> items = new();

    public IReadOnlyList<InventorySlot> Items => items;

    public int GetAmount(IngredientData ingredient)
    {
        var slot = items.Find(x => x.ingredient == ingredient);
        return slot != null ? slot.amount : 0;
    }

    public void Add(IngredientData ingredient, int amount = 1)
    {
        var slot = items.Find(x => x.ingredient == ingredient);

        if (slot != null)
            slot.amount += amount;
        else
            items.Add(new InventorySlot(ingredient, amount));
    }

    public bool Remove(IngredientData ingredient, int amount = 1)
    {
        var slot = items.Find(x => x.ingredient == ingredient);

        if (slot == null || slot.amount < amount)
            return false;

        slot.amount -= amount;

        slot.amount = Mathf.Max(0, slot.amount);

        return true;
    }

    public bool Has(IngredientData ingredient, int amount = 1)
    {
        var slot = items.Find(x => x.ingredient == ingredient);
        return slot != null && slot.amount >= amount;
    }
}