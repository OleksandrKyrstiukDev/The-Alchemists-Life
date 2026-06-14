using UnityEngine;

[CreateAssetMenu(fileName = "Order", menuName = "Game/Order")]
public class OrderObject : ScriptableObject
{
    [Header("Client")]
    public string clientName;
    public Sprite portrait;

    [Header("Description")]
    [TextArea]
    public string shortDescription;

    [TextArea]
    public string fullDescription;

    [Header("Requirements")]
    public PotionPurpose requiredPurpose;

    public PotionQuality minimumQuality;

    [TextArea]
    public string restrictions;

    [Header("Rewards")]
    public int goldReward;
    public int reputationReward;

    [Header("Penalty")]
    public int declinePenalty = 1;

    [Header("Progression")]
    public ReputationTier requiredTier;
}