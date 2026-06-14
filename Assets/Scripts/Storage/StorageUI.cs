using System.Collections.Generic;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    [SerializeField] private StorageInventory storage;
    [SerializeField] private Transform container;
    [SerializeField] private IngredientSlotUI prefab;
    [SerializeField] private ShopSystem shop;
    [SerializeField] private PlayerInventory playerInventory;

    private readonly List<IngredientSlotUI> uiSlots = new();

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (var ui in uiSlots)
            Destroy(ui.gameObject);

        uiSlots.Clear();

        foreach (var slot in storage.Items)
        {
            var ui = Instantiate(prefab, container);
            ui.Setup(slot, this);
            uiSlots.Add(ui);
        }
    }

    public void TakeIngredient(InventorySlot slot)
    {
        bool success = storage.TakeIngredient(slot.ingredient, 1);

        if (!success)
        {
            Debug.Log("[StorageUI] Not enough items");
            return;
        }

        playerInventory.Add(slot.ingredient, 1);

        Debug.Log(
            $"[StorageUI] Moved {slot.ingredient.displayName} to player inventory"
        );

        Refresh();
    }

    public void Buy(
    IngredientData ingredient,
    int amount
)
    {
        if (shop.BuyIngredient(ingredient, amount))
        {
            Refresh();
        }
    }
}