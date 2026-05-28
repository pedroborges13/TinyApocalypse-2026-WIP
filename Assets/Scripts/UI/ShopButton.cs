using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public enum ItemType { Weapon, PlaceableItem }

    [Header("Item Type")]
    [SerializeField] private ItemType itemType; 

    [Header("Settings")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private PlaceableItemData itemData;

    [Header("References")]
    [SerializeField] private Button myButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI priceText;

    void Start()
    {
        if (myButton == null) myButton = GetComponent<Button>();

        ButtonPrices();
    }

    public GameObject WeaponPrefab
    {
        get { return weaponPrefab; }
    }

    public void ButtonPrices()
    {
        if (itemData != null)
        {
            if (priceText != null) priceText.text = $"{itemData.Price}";
            return;
        }

        if (weaponPrefab != null)
        {
            Weapon weaponScript = weaponPrefab.GetComponent<Weapon>();

            if (weaponScript != null && priceText != null)
            {
                priceText.text = $"{weaponScript.GetPrice()}";
            }
        }
    }

    public void BuyItemButton()
    {
        ShopManager.Instance.BuyWeapon(weaponPrefab, this);
    }

    public void SetAsPurchased()
    {
        if (itemType != ItemType.Weapon) return;

        myButton.interactable = false;

        if (buttonImage != null) buttonImage.color = new Color (175, 175, 175, 1f);

        priceText.text = "SOLD";
    }
}
