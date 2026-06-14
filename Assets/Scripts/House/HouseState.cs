using UnityEngine;

public class HouseState : MonoBehaviour
{
    public float warmth = 0f;
    public float cleanliness = 0f;

    public float decayRate = 0.01f;

    public float maxWarmth = 1f;
    public float maxCleanliness = 1f;

    public bool IsComplete => GetProgress() >= 1f;

    private void Update()
    {
        Decay();
    }

    void Decay()
    {
        warmth -= decayRate * Time.deltaTime;
        cleanliness -= decayRate * Time.deltaTime;

        warmth = Mathf.Clamp(warmth, 0, maxWarmth);
        cleanliness = Mathf.Clamp(cleanliness, 0, maxCleanliness);
    }

    public float GetProgress()
    {
        return (warmth + cleanliness) / (maxWarmth + maxCleanliness);
    }

    // =========================
    // SAVE / LOAD
    // =========================

    public HouseStateData GetSaveData()
    {
        return new HouseStateData
        {
            warmth = warmth,
            cleanliness = cleanliness,
            decayRate = decayRate,
            maxWarmth = maxWarmth,
            maxCleanliness = maxCleanliness
        };
    }

    public void LoadFromData(HouseStateData data)
    {
        if (data == null)
            return;

        warmth = data.warmth;
        cleanliness = data.cleanliness;
        decayRate = data.decayRate;
        maxWarmth = data.maxWarmth;
        maxCleanliness = data.maxCleanliness;
    }
}