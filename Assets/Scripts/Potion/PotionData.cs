using UnityEngine;

[CreateAssetMenu(menuName = "Potion/Brewed Potion")]
public class PotionData : ScriptableObject
{
    public string potionName;
    public PotionPurpose purpose;

    public PotionQuality quality;

    public float stability;
    public float toxicity;
}
