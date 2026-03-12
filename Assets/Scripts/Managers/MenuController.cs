using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;

    public void StartButton()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void OptionsButton()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(true);
        creditsScreen.SetActive(false);
    }

    public void CreditsButton()
    {
        menuScreen.SetActive(false);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void CloseButton()
    {
        menuScreen.SetActive(true);
        optionsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
