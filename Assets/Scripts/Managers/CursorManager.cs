using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance {  get; private set; }

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D gameplayCrosshair;

    [Header("Settings")]
    [SerializeField] private Vector2 crosshairHotspot = new Vector2(16, 16);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void SetGameplayCursor()
    {
        Cursor.SetCursor(gameplayCrosshair, crosshairHotspot, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
