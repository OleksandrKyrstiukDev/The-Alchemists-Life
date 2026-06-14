public enum ReputationTier
{
    Exile,
    Unknown,
    Practitioner,
    Respected,
    Master,
    Legend
}

public static class ReputationHelper
{
    public static string GetDisplayName(ReputationTier tier)
    {
        return tier switch
        {
            ReputationTier.Exile => "Вигнанець лабораторій",
            ReputationTier.Unknown => "Невідомий алхімік",
            ReputationTier.Practitioner => "Практик з околиць",
            ReputationTier.Respected => "Поважний алхімік",
            ReputationTier.Master => "Майстер трансмутації",
            ReputationTier.Legend => "Легенда забороненої науки",
            _ => "Невідомо"
        };
    }
}