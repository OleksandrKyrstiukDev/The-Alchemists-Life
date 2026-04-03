using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    [Header("Main ingredient")]
    public IngredientData ingredient;

    [Header("Quantity")]
    public int amount = 1;

    [Header("Alternative ingredients (optional)")]
    public List<IngredientData> alternatives; // сюди можна додавати замінники
}
