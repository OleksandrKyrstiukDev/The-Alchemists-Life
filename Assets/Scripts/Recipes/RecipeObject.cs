using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Game/Recipe")]
public class RecipeObject : ScriptableObject
{
    [Header("ID")]
    public string id;

    [Header("Info")]
    public string displayName;
    [TextArea] public string description;
    [TextArea] public string instructions;

    [Header("Required Ingredients")]
    public List<RecipeIngredient> requiredIngredients;

    [Header("Brewing Phases")]
    public List<BrewingPhase> phases;

    [Header("Side Effects")]
    public List<SideEffect> sideEffects;

    public Sprite icon;

    [Header("Progression")]
    public ReputationTier requiredTier;
}

[System.Serializable]
public class BrewingPhase
{
    public string phaseName; 
    public float optimalTemperature;
    public float temperatureTolerance;
    public float optimalTime;
    public float timeTolerance;
    public int optimalStirCount;
    public float stirTolerance;
}

[System.Serializable]
public class SideEffect
{
    public string effectName;
    [TextArea] public string description;
    [Range(0f,1f)] public float chance;
}
