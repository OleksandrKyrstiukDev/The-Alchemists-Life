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
    [Range(1f, 5f)] public float stability;  // вплив на стабільність зілля
    [Range(0f, 3f)] public float toxicity;   // токсичність
    [Range(-2f, 2f)] public float potency;   // сила ефекту

    [Header("Sensitivity (modifiers for phases)")]
    public float tempModifier = 0f;   // змінює допустиму температуру
    public float timeModifier = 0f;   // змінює допустимий час варіння
    public float stirModifier = 0f;   // вплив на допустиму кількість помішувань

    [Header("Tags")]
    public List<IngredientTag> tags;

    [Header("Phase Effects")]
    public bool affectsStartPhase = true;   // вплив на фазу початку
    public bool affectsBrewPhase = true;    // вплив на фазу варіння
    public bool affectsFinishPhase = true;  // вплив на фазу завершення

    [Header("Optional")]
    public Sprite icon; // для UI, якщо треба
}
