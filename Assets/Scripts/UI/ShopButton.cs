using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject weaponPrefab;
    //[SerializeField] private Color purchasedColor;

    [Header("References")]
    [SerializeField] private Button myButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI priceText;

    void Start()
    {
        if (myButton == null) myButton = GetComponent<Button>();
    }

    public void BuyItemButton()
    {
        ShopManager.Instance.BuyWeapon(weaponPrefab, this);
    }

    public void SetAsPurchased()
    {
        myButton.interactable = false;

        buttonImage.color = new Color (175, 175, 175, 1f);
        priceText.text = "SOLD";
    }
}
