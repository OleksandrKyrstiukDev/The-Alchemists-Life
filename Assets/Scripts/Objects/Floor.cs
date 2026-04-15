using UnityEngine;

public class Floor : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private CleaningReaction cleaningReaction;
    [SerializeField] private ParticleSystem dirtParticles;

    private float baseEmission;

    void Awake()
    {
        if (cleaningReaction == null)
            cleaningReaction = GetComponent<CleaningReaction>();

        if (dirtParticles != null)
        {
            var emission = dirtParticles.emission;
            baseEmission = emission.rateOverTime.constant;
        }
    }

    void OnEnable()
    {
        if (cleaningReaction != null)
            cleaningReaction.OnStateChanged += OnCleaningChanged;
    }

    void OnDisable()
    {
        if (cleaningReaction != null)
            cleaningReaction.OnStateChanged -= OnCleaningChanged;
    }

    void OnCleaningChanged(CleaningState state)
    {
        Debug.Log($"[Floor] State: {state}");

        if (dirtParticles == null) return;

        var emission = dirtParticles.emission;

        switch (state)
        {
            case CleaningState.PerfectClean:
                dirtParticles.Stop();
                break;

            case CleaningState.Clean:
                emission.rateOverTime = baseEmission * 0.5f;
                break;

            case CleaningState.Messy:
                emission.rateOverTime = baseEmission * 2f;
                if (!dirtParticles.isPlaying)
                    dirtParticles.Play();
                break;
        }
    }
}
