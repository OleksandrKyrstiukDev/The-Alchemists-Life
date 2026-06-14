using TMPro;
using UnityEngine;

public class ReputationUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tierText;

    private ReputationTier lastTier;
    private int lastReputation;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (playerStats == null)
            return;

        if (playerStats.Reputation != lastReputation ||
            playerStats.CurrentTier != lastTier)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        lastReputation = playerStats.Reputation;
        lastTier = playerStats.CurrentTier;

        tierText.text =
            $"🧪 {ReputationHelper.GetDisplayName(lastTier)}\n" +
            $"Репутація: {lastReputation}";
    }
}