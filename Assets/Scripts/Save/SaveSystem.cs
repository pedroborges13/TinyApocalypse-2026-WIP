using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");

    public static bool IsLoadingSave {  get; set; }

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved in: {savePath}");
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No Save Found!");
            return null;
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static bool DoesSaveExist()
    {
        return File.Exists(savePath);   
    }
}
