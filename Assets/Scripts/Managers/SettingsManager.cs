using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private readonly int[] widths = { 1920, 1600, 1366, 1280 };
    private readonly int[] heights = { 1080, 900, 768, 720 };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //LoadSettings();
    }

    public void SetResolution(int index)
    {
        bool isFullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        Screen.SetResolution(widths[index], heights[index], isFullScreen);

        //if (isFullScreen) 
        //PlayerPrefs.SetInt("FullScreen", isFu)
    }
}
