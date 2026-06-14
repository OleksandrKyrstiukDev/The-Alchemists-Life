using TMPro;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI reputationText;

    public int Gold { get; private set; }
    public int Reputation { get; private set; }

    private void Awake()
    {
        RefreshUI();
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        RefreshUI();
    }

    public void RemoveGold(int amount)
    {
        Gold -= amount;

        if (Gold < 0)
            Gold = 0;

        RefreshUI();
    }

    public void AddReputation(int amount)
    {
        Reputation += amount;
        RefreshUI();
    }

    public void RemoveReputation(int amount)
    {
        Reputation -= amount;

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (goldText != null)
            goldText.text = Gold.ToString();

        if (reputationText != null)
            reputationText.text = Reputation.ToString();
    }

    public ReputationTier CurrentTier
    {
        get
        {
            if (Reputation >= 321)
                return ReputationTier.Legend;

            if (Reputation >= 221)
                return ReputationTier.Master;

            if (Reputation >= 131)
                return ReputationTier.Respected;

            if (Reputation >= 61)
                return ReputationTier.Practitioner;

            if (Reputation >= 0)
                return ReputationTier.Unknown;

            return ReputationTier.Exile;
        }
    }
}