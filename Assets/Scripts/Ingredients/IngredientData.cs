using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "IngredientData", menuName = "Game/Ingredient")]
public class IngredientData : ScriptableObject
{
    [Header("Identity")]
    public string id;               
    public string displayName;   
    [TextArea] public string description;

    [Header("Alchemy Properties")]
    [Range(1f, 5f)] public float stability;  
    [Range(0f, 3f)] public float toxicity;  
    [Range(-2f, 2f)] public float potency;  

    [Header("Sensitivity (modifiers for phases)")]
    public float tempModifier = 0f;   
    public float timeModifier = 0f;  
    public float stirModifier = 0f;  

    [Header("Tags")]
    public List<IngredientTag> tags;

    [Header("Phase Effects")]
    public bool affectsStartPhase = true;  
    public bool affectsBrewPhase = true; 
    public bool affectsFinishPhase = true;  

    [Header("Optional")]
    public Sprite icon;

    [Header("Price")]
    public int buyPrice = 5;

    public Color particleColor = Color.green;
}
