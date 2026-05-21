using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;

    void Update()
    {
        EscButton();
    }

    public void StartButton()
    {
        SaveSystem.IsLoadingSave = false;
        SceneManager.LoadScene("Gameplay");
    }

    public void ContinueButton()
    {
        if (SaveSystem.DoesSaveExist())
        {
            SaveSystem.IsLoadingSave = true;
            SceneManager.LoadScene("Gameplay");
        }
        else Debug.LogWarning("No Save Found!");
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
        Debug.Log("Close button");
    }

    public void EscButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuScreen.SetActive(true);
            optionsScreen.SetActive(false);
            creditsScreen.SetActive(false);
            Debug.Log("Esc button");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
