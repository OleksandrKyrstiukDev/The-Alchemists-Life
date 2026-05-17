public static class OrderEvaluator
{
    public static OrderResult Evaluate(
        OrderObject order,
        PotionData potion
    )
    {
        if (potion == null)
            return OrderResult.Fail;

        // “ип з≥лл€
        if (potion.purpose != order.requiredPurpose)
            return OrderResult.Fail;

        // як≥сть
        if (potion.quality < order.minimumQuality)
            return OrderResult.Medium;

        // “оксичн≥сть (приклад)
        if (potion.toxicity > 0.7f)
            return OrderResult.Fail;

        return OrderResult.Perfect;
    }
}

public enum OrderResult
{
    Perfect,
    Medium,
    Fail
}