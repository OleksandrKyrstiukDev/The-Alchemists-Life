using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameSaveData Data;

    public int CurrentDay => Data.day;
    public int Gold => Data.gold;
    public int Reputation => Data.reputation;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGame();
    }

    public void LoadGame()
    {
        Data = SaveSystem.Load();
        ApplyToSystems();
    }

    public void SaveGame()
    {
        SaveSystem.Save(Data);
    }

    public void NextDay()
    {
        Data.day++;
        SaveGame();
    }

    private void ApplyToSystems()
    {
        Debug.Log($"[LOAD] Day: {Data.day}, Gold: {Data.gold}, Rep: {Data.reputation}");

    }
}