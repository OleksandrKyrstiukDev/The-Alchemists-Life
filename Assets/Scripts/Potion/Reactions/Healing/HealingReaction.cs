using UnityEngine;

public enum PlantState
{
    Normal,
    Grown,
    Overgrown,
    Dead
}

public class HealingReaction : PotionReaction
{
    public PlantState CurrentState { get; private set; }

    public event System.Action<PlantState> OnStateChanged;

    public override void React(BrewResult quality)
    {
        CurrentState = quality switch
        {
            BrewResult.Perfect => PlantState.Overgrown,
            BrewResult.Good => PlantState.Grown,
            BrewResult.Fail => PlantState.Dead,
            _ => PlantState.Normal
        };

        Debug.Log($"[HealingReaction] State: {CurrentState}");

        OnStateChanged?.Invoke(CurrentState);
    }
}