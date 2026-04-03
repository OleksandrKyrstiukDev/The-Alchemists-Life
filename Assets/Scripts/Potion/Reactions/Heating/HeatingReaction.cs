using UnityEngine;

public class HeatingReaction : PotionReaction
{
    public HeatingState CurrentState { get; private set; }
    public event System.Action<HeatingState> OnStateChanged;

    public override void React(BrewResult quality)
    {
        Debug.Log($"[HeatingReaction] React called: {quality}");
        CurrentState = quality switch
        {
            BrewResult.Perfect => HeatingState.Perfect,
            BrewResult.Good => HeatingState.Stable,
            BrewResult.Fail => HeatingState.Unstable,
            _ => HeatingState.Off
        };

        OnStateChanged?.Invoke(CurrentState);
    }
}

