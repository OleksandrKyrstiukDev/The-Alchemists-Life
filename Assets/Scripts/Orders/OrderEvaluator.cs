using UnityEngine;

public static class OrderEvaluator
{
    public static OrderResult Evaluate(
        OrderObject order,
        BrewedPotionData potion
    )
    {
        if (potion.name == null || potion.name == "")
            return OrderResult.Fail;

        Debug.Log($"[OrderEvaluator] Potion: {potion.name} | Purpose: {potion.purpose} | Result: {potion.result}");

        // =========================
        // 1. PURPOSE CHECK
        // =========================
        if (potion.purpose != order.requiredPurpose)
        {
            Debug.Log("[OrderEvaluator] FAIL: Wrong purpose");
            return OrderResult.Fail;
        }

        // =========================
        // 2. TOXICITY CHECK
        // =========================
        bool isToxic = potion.toxicity > 0.7f;

        if (isToxic)
        {
            Debug.Log("[OrderEvaluator] FAIL: Toxic potion");
            return OrderResult.Fail;
        }

        // =========================
        // 3. QUALITY CHECK
        // =========================
        int quality = (int)potion.result;
        int minQuality = (int)order.minimumQuality;

        int diff = quality - minQuality;

        Debug.Log($"[OrderEvaluator] Quality diff: {diff}");

        // =========================
        // 4. RESULT LOGIC
        // =========================

        if (diff >= 1)
            return OrderResult.Perfect;

        if (diff == 0)
            return OrderResult.Perfect;

        if (diff == -1)
            return OrderResult.Medium;

        return OrderResult.Fail;
    }
}

public enum OrderResult
{
    Perfect,
    Medium,
    Fail
}