using TMPro;
using UnityEngine;

public class IngredientUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;

    public void Setup(RecipeIngredient ingredient)
    {
        nameText.text = ingredient.ingredient.displayName;
        amountText.text = ingredient.amount.ToString();
    }
}