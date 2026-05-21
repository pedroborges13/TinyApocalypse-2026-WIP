using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Save Prefabs Mapping")]
    [SerializeField] private GameObject barrierPrefab;
    [SerializeField] private GameObject explosiveBarrelPrefab;
    [SerializeField] private GameObject landminePrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();

        // --- PLAYER DATA ---
        if (GameManager.Instance != null) saveData.currentWave = GameManager.Instance.CurrentWave;

        PlayerWallet wallet = FindFirstObjectByType<PlayerWallet>();
        if (wallet != null) saveData.currentMoney = wallet.Money;

        // BUILDINGS DATA ---
        PlaceableObject[] sceneObjects = FindObjectsByType<PlaceableObject>(FindObjectsSortMode.None);
        foreach (var obj in sceneObjects)
        {
            saveData.placedObjects.Add(new ObjectSaveData { objectType = obj.ObjectType, position = obj.transform.position, rotation = obj.transform.rotation });
        }

        SaveSystem.Save(saveData);
        Debug.Log("Game saved");
    }

    public void LoadGame()
    {
        GameSaveData saveData = SaveSystem.Load();

        if (saveData == null) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestoreWave(saveData.currentWave);
        }

        PlaceableObject[] oldObjects = FindObjectsByType<PlaceableObject>(FindObjectsSortMode.None);
        foreach (var oldObj in oldObjects) Destroy(oldObj.gameObject); 

        foreach (var objData in saveData.placedObjects)
        {
            GameObject prefab = GetPrefabByType(objData.objectType);

            if (prefab != null)
            {
                Instantiate(prefab, objData.position, objData.rotation );
            }
        }
    }

    GameObject GetPrefabByType(PlacedObjectType type)
    {
        return type switch
        {
            PlacedObjectType.Barrier => barrierPrefab,
            PlacedObjectType.ExplosiveBarrel => explosiveBarrelPrefab,
            PlacedObjectType.Landmine => landminePrefab,
            _ => null
        };
    }
}
