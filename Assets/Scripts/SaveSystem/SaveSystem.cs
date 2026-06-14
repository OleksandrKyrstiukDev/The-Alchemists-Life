using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/save.json";

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path, json);

        Debug.Log($"[SAVE] Game saved to {Path}");
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(Path))
        {
            Debug.Log("[SAVE] No save found → creating new");
            return new GameSaveData
            {
                day = 1,
                gold = 0,
                reputation = 0,
                progress = 0,
                houseState = new HouseStateData()
            };
        }

        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(Path))
            File.Delete(Path);
    }
}
