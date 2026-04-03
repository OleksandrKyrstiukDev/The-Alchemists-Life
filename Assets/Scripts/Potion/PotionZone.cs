using UnityEngine;
public class PotionZone : MonoBehaviour
{
    public PotionPurpose allowedPurpose;
    public PotionReaction reaction;

    private PotionObject potionInZone;

    void OnTriggerEnter(Collider other)
    {
        var potion = other.GetComponent<PotionObject>();
        if (potion == null) return;

        if (potion.Data.name == null)
        {
            Debug.LogWarning("[PotionZone] Potion without Data entered zone");
            return;
        }

        if (potion.Data.purpose != allowedPurpose)
        {
            Debug.Log(
                $"[PotionZone] ❌ Wrong purpose!\n" +
                $"Potion: {potion.Data.name}\n" +
                $"Has: {potion.Data.purpose}\n" +
                $"Required: {allowedPurpose}"
            );
            return;
        }

        potionInZone = potion;
        Debug.Log("[PotionZone] ✅ Potion in zone");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PotionObject>() == potionInZone)
            potionInZone = null;
    }

    public bool CanApply(PotionObject potion)
    {
        return potionInZone == potion;
    }

    public void Apply(PotionObject potion)
    {
        if (potion == null)
        {
            Debug.LogError("[PotionZone] Potion is NULL");
            return;
        }

        if (potion.Data.name == null)
        {
            Debug.LogError("[PotionZone] Potion Data is NULL");
            return;
        }

        if (reaction == null)
        {
            Debug.LogError("[PotionZone] Reaction is NULL (не призначений в інспекторі)");
            return;
        }

        Debug.Log($"[PotionZone] Applying {potion.Data.name}");

        reaction.React(potion.Data.result);
    }
}
