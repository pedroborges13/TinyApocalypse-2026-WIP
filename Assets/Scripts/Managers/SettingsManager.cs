using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    //Readonly arrays to store fixed resolution values
    private readonly int[] widths = { 1920, 2560 };
    private readonly int[] heights = { 1080, 1080 };

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
        LoadSettings();
    }

    public void SetResolution(int index)
    {
        //PlayerPrefs doesn't support bool directly. It only accepts int, float, or string.
        bool isFullScreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        //Applies the resolution using values from the arrays based on the selected index
        Screen.SetResolution(widths[index], heights[index], isFullScreen);

        //Saves the chosen resolution index to disk
        PlayerPrefs.SetInt("ResIndex", index);

        Debug.Log($"Changing Resolution to: {widths[index]}x{heights[index]}");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        int valueToSave;

        //Converts boolean to int (1 or 0). PlayerPrefs doesn't support bools
        if (isFullScreen) valueToSave = 1;
        else valueToSave = 0;

        //Saves the fullscreen preference
        PlayerPrefs.SetInt("FullScreen", valueToSave);

        Debug.Log($"Setting Fullscreen to: {isFullScreen}");
    }

    void LoadSettings()
    {
        //Fetches saved data or uses default values (0 for 1080p, 1 for Fullscreen)
        int resIndex = PlayerPrefs.GetInt("ResIndex", 0); //1920x1080
        bool isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1; //Fullscreen

        SetResolution(resIndex);
        SetFullScreen(isFullScreen);
    }
}
