using UnityEngine;

public enum PotionQuality
    {
        Fail,
        Good,
        Perfect
    }

public enum PotionPurpose
{
    Heating,
    Repair,
    Poison,
    Healing,
    Clean,
    None
}

public interface IPotionReactive
{
    void ApplyPotion(BrewedPotionData potion);
}

public struct BrewedPotion
{
    public BrewedPotionData data;
    public GameObject prefab;
}

public struct BrewedPotionData
{
    public string name;
    public BrewResult result;
    public float stability;
    public float toxicity;
    public PotionPurpose purpose;
}

public enum HeatingState { Off, Unstable, Stable, Perfect }