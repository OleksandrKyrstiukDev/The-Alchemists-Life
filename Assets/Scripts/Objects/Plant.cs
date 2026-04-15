using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private HealingReaction healingReaction;

    [Header("Scale Settings")]
    [SerializeField] private float perfectScaleMultiplier = 2f;
    [SerializeField] private float goodScaleMultiplier = 1.3f;

    void Awake()
    {
        if (healingReaction == null)
            healingReaction = GetComponent<HealingReaction>();
    }

    void OnEnable()
    {
        if (healingReaction != null)
            healingReaction.OnStateChanged += OnPlantStateChanged;
    }

    void OnDisable()
    {
        if (healingReaction != null)
            healingReaction.OnStateChanged -= OnPlantStateChanged;
    }

    void OnPlantStateChanged(PlantState state)
    {
        Debug.Log($"[Plant] State: {state}");

        switch (state)
        {
            case PlantState.Grown:
                Grow(goodScaleMultiplier);
                break;

            case PlantState.Overgrown:
                Grow(perfectScaleMultiplier);
                break;

            case PlantState.Dead:
                DestroyPlant();
                break;
        }
    }

    void Grow(float multiplier)
    {
        transform.localScale *= multiplier;
        Debug.Log($"[Plant] Scaled x{multiplier}");
    }

    void DestroyPlant()
    {
        Debug.Log("[Plant] Destroyed");
        Destroy(gameObject);
    }
}