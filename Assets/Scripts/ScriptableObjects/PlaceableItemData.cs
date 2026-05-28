using System.Runtime.CompilerServices;
using UnityEngine;

//No creation menu as this serve only as a template
public abstract class PlaceableItemData : ScriptableObject
{
    [Header("Base Purchase Configuration")]
    [SerializeField] private string itemName;
    [SerializeField] private int price;
    [SerializeField] private GameObject prefab; //Real Object
    [SerializeField] private GameObject previewPrefab; //A version without scripts, purely visual (transparent material)
    [SerializeField] private Vector2Int size = new Vector2Int(1, 1);

    public string ItemName => itemName;
    public int Price => price;
    public GameObject Prefab => prefab;
    public GameObject PreviewPrefab => previewPrefab;
    public Vector2Int Size => size;
}
