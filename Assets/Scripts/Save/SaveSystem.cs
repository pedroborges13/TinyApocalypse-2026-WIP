using UnityEngine;
using System.IO;
using UnityEditorInternal;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");

    // Global flag to determine if the game should load state configuration during scene initialization.
    public static bool IsLoadingSave {  get; set; }

    public static void Save(GameSaveData data)
    {
        //Converts the C# object into a readable JSON format string
        string json = JsonUtility.ToJson(data, true);

        //Creates or overwrites the file at the target path with the JSON string content
        File.WriteAllText(savePath, json);

        Debug.Log($"Game saved in: {savePath}");
    }

    public static GameSaveData Load()
    {
        //Safety check to prevent crashing or throwing exceptions if the player has no previous save
        if (!File.Exists(savePath))
        {
            Debug.Log("No Save Found!");
            return null;
        }

        //Read the entire text content from the file into memory
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
    }

    //True if the file exists on the designated path, otherwise false.
    public static bool DoesSaveExist()
    {
        return File.Exists(savePath);   
    }
}
