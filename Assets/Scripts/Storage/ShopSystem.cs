using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private StorageInventory storage;

    public bool BuyIngredient(
        IngredientData ingredient,
        int amount
    )
    {
        int totalPrice = ingredient.buyPrice * amount;

        if (playerStats.Gold < totalPrice)
        {
            Debug.Log("[SHOP] Not enough gold");
            return false;
        }

        playerStats.RemoveGold(totalPrice);

        storage.AddIngredient(
            ingredient,
            amount
        );

        Debug.Log(
            $"[SHOP] Bought {ingredient.displayName} x{amount}"
        );

        return true;
    }
}