using System;
using UnityEngine;
[Serializable]
public class GameSaveData
{
    public int day;

    public int gold;
    public int reputation;

    public int progress;

    public HouseStateData houseState;
}

[Serializable]
public class HouseStateData
{
    public float warmth;
    public float cleanliness;

    public float decayRate;

    public float maxWarmth = 1f;
    public float maxCleanliness = 1f;
}