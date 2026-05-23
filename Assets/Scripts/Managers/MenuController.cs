using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject savedGameScreen;

    [Header("Menu Buttons")]
    [SerializeField] private Button continueButton;

    void Start()
    {
        CheckExistingSave();

        if (CursorManager.Instance != null) CursorManager.Instance.SetDefaultCursor();
    }

    void Update()
    {
        EscButton();
    }

    void CheckExistingSave()
    { 
        bool saveExist = SaveSystem.DoesSaveExist();
        continueButton.interactable = saveExist;    
    }

    public void StartButton()
    {
        if (SaveSystem.DoesSaveExist())
        {
            savedGameScreen.SetActive(true);
            menuScreen.SetActive(false);
            optionsScreen.SetActive(false);
            creditsScreen.SetActive(false);
        }
        else
        {
            SaveSystem.IsLoadingSave = false;
            Time.timeScale = 1;
            SceneManager.LoadScene("Gameplay");
        }
    }

    public void NewGameButton()
    {
        SaveSystem.IsLoadingSave = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Gameplay");
    }

    public void ContinueButton()
    {
        if (SaveSystem.DoesSaveExist())
        {
            SaveSystem.IsLoadingSave = true;
            Time.timeScale = 1;
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
            savedGameScreen.SetActive(false);
            Debug.Log("Esc button");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
