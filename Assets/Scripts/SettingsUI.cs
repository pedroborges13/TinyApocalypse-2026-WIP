using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    void OnEnable()
    {
        resDropdown.value = PlayerPrefs.GetInt("ResIndex", 0);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        //Subscribes to Unity UI events. These trigger when the user interacts with the elements
        resDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
    }

    void OnResolutionDropdownChanged(int index)
    {
        SettingsManager.Instance.SetResolution(index);
    }

    void OnFullscreenToggleChanged(bool val)
    {
        SettingsManager.Instance.SetFullScreen(val);
    }
}
