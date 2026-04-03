using UnityEngine;

public abstract class PotionReaction : MonoBehaviour
{
    public PotionPurpose purpose;
    public abstract void React(BrewResult quality);
}

