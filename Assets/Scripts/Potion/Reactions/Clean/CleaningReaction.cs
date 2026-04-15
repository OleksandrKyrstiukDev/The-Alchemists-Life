using UnityEngine;

public enum CleaningState
{
    Dirty,
    Clean,
    PerfectClean,
    Messy
}

public class CleaningReaction : PotionReaction
{
    public CleaningState CurrentState { get; private set; }

    public event System.Action<CleaningState> OnStateChanged;

    public override void React(BrewResult quality)
    {
        CurrentState = quality switch
        {
            BrewResult.Perfect => CleaningState.PerfectClean,
            BrewResult.Good => CleaningState.Clean,
            BrewResult.Fail => CleaningState.Messy,
            _ => CleaningState.Dirty
        };

        Debug.Log($"[CleaningReaction] State: {CurrentState}");

        OnStateChanged?.Invoke(CurrentState);
    }
}