using UnityEngine;

public class HealingReaction : PotionReaction
{
    [Header("Growth Settings")]
    [SerializeField] private float perfectScaleMultiplier = 2f;
    [SerializeField] private float goodScaleMultiplier = 1.3f;

    [SerializeField] private bool destroyOnFail = true;

    public override void React(BrewResult quality)
    {
        Debug.Log($"[HealingReaction] React: {quality}");

        switch (quality)
        {
            case BrewResult.Perfect:
                Grow(perfectScaleMultiplier);
                break;

            case BrewResult.Good:
                Grow(goodScaleMultiplier);
                break;

            case BrewResult.Fail:
                Fail();
                break;
        }
    }

    void Grow(float multiplier)
    {
        transform.localScale *= multiplier;

        Debug.Log($"[HealingReaction] Plant grown x{multiplier}");
    }

    void Fail()
    {
        Debug.Log("[HealingReaction] Plant destroyed");

        if (destroyOnFail)
            Destroy(gameObject);
    }
}