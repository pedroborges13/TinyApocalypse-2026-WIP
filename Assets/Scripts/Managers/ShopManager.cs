using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private BuildingData itemData;
    private Inventory inventory;
    private PlayerWallet wallet;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        wallet = player.GetComponent<PlayerWallet>();

        if (BuildManager.Instance != null) BuildManager.Instance.OnBuildingPlaced += OnBuildingConfirmed;
    }

    public void BuyWeapon(GameObject weaponPrefab, ShopButton button)
    {
        //Gets weapon cost value
        Weapon weaponScript = weaponPrefab.GetComponent<Weapon>();
        int cost = weaponScript.GetPrice();

        if (wallet.Money >= cost)
        {
            wallet.SpendMoney(cost);
            inventory.AddWeapon(weaponPrefab);

            //Notifies the UI that this specific button is now "purchased"
            button.SetAsPurchased();
        }
        else
        {
            Debug.Log("Not enough money");
            //Red button?
        }
    }

    public void StartingBuildingPurchase(BuildingData data)
    {
        if (wallet.Money >= data.Price)
        {
            BuildManager.Instance.SelectBuildingToPlace(data);
            UIManager.Instance.CloseShopUI();
        }
    }

    void OnBuildingConfirmed(int cost)
    {
        if (wallet.Money >= cost) wallet.SpendMoney(cost);
    }

    void OnDestroy()
    {
        if (BuildManager.Instance != null) BuildManager.Instance.OnBuildingPlaced -= OnBuildingConfirmed;
    }
}
