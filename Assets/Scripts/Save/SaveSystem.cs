using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved in: {savePath}");
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(savePath)) return null;

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
    }
}
