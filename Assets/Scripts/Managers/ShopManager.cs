using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [SerializeField] private BuildingData itemData;
    private Inventory inventory;
    private PlayerWallet wallet;

    [Header("UI Weapon Buttons")]
    [SerializeField] private List<ShopButton> weaponButtons = new List<ShopButton>();

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

        RefreshShopButtons();
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

    public void StartingTowersPurchase(TowerData data)
    {
        if (wallet.Money >= data.Price)
        {
            //BuildManager.Instance.SelectBuildingToPlace(data);
            UIManager.Instance.CloseShopUI();
        }
    }

    void OnBuildingConfirmed(int cost)
    {
        if (wallet.Money >= cost) wallet.SpendMoney(cost);
    }

    public void RefreshShopButtons()
    {
        if (inventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null) inventory = player.GetComponent<Inventory>();
        }

        List<string> ownedWeaponNames = inventory.GetWeaponNamesInIventory();

        foreach (ShopButton button in weaponButtons)
        {
            if (button != null && button.WeaponPrefab != null)
            {
                Weapon buttonWeapon = button.WeaponPrefab.GetComponent<Weapon>();

                if (buttonWeapon != null)
                {
                    if (ownedWeaponNames.Contains(buttonWeapon.WeaponName))
                    {
                        button.SetAsPurchased();
                    }
                }
            }
        }
    }
    void OnDestroy()
    {
        if (BuildManager.Instance != null) BuildManager.Instance.OnBuildingPlaced -= OnBuildingConfirmed;
    }
}
