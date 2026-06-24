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

    void Start()
    {
        if (SaveSystem.IsLoadingSave)
        {
            LoadGame();
            SaveSystem.IsLoadingSave = false;
        }
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();

        // --- PLAYER DATA ---
        if (GameManager.Instance != null) saveData.currentWave = GameManager.Instance.CurrentWave;

        //Finds the player's currency state manager in the scene hierarchy
        PlayerWallet wallet = FindAnyObjectByType<PlayerWallet>();
        if (wallet != null) saveData.currentMoney = wallet.Money;

        // BUILDINGS DATA ---
        //Gets all placed objects (barrier, explosive barrel, etc) 
        PlaceableObject[] sceneObjects = FindObjectsByType<PlaceableObject>();
        foreach (var obj in sceneObjects)
        {
            saveData.placedObjects.Add(new ObjectSaveData { objectType = obj.ObjectType, position = obj.transform.position, rotation = obj.transform.rotation });
        }

        //Weapons
        Inventory inventory = FindAnyObjectByType<Inventory>();
        if (inventory != null)
        {
            saveData.savedWeaponNames = inventory.GetWeaponNamesInIventory();
            saveData.savedWeaponIndex = inventory.GetCurrentWeaponIndex();
        }

        SaveSystem.Save(saveData);
        Debug.Log("Game saved");
    }

    public void LoadGame()
    {
        GameSaveData saveData = SaveSystem.Load();

        if (saveData == null) return;

        Debug.Log($"Carregando save com {saveData.placedObjects.Count} objetos.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestoreWave(saveData.currentWave);
        }

        //Items
        PlaceableObject[] oldObjects = FindObjectsByType<PlaceableObject>();
        foreach (var oldObj in oldObjects) Destroy(oldObj.gameObject); 

        foreach (var objData in saveData.placedObjects)
        {
            GameObject prefab = GetPrefabByType(objData.objectType);

            if (prefab != null)
            {
                Instantiate(prefab, objData.position, objData.rotation );
            }
            else
            {
                Debug.LogWarning($"Prefab não encontrado para o tipo: {objData.objectType}");
            }
        }

        // --- RESTORE MONEY ---
        PlayerWallet wallet = FindAnyObjectByType<PlayerWallet>();
        if (wallet != null) wallet.LoadMoney(saveData.currentMoney);

        // --- RESTORE WEAPONS AND SHOP UI ---
        Inventory inventory = FindAnyObjectByType<Inventory>();
        if (inventory != null)
        {
            inventory.LoadInventoryState(saveData.savedWeaponNames, saveData.savedWeaponIndex);

            if (ShopManager.Instance != null)
            {
                //Refreshes UI visual contexts so purchased weapons show up sold out
                ShopManager.Instance.RefreshShopButtons();
            }
        }
    }

    GameObject GetPrefabByType(PlacedObjectType type)
    {
        switch (type)
        {
            case PlacedObjectType.Barrier:
                return barrierPrefab;
            case PlacedObjectType.ExplosiveBarrel:
                return explosiveBarrelPrefab;
            case PlacedObjectType.Landmine:
                return landminePrefab;
            default:
                return null;
        }
    }
}
