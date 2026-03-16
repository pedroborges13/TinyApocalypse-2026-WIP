using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject preparationUI;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject shopButton;
    [SerializeField] private GameObject startWaveButton;

    [Header("Weapon HUD")]
    [SerializeField] private GameObject weaponHUD;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private GameObject reloadHUD;
    [SerializeField] private Image reloadFillImage;
    private Coroutine currentReloadCoroutine; //Stores reference to the active coroutine

    [Header("Weapon Icons")]
    [SerializeField] private GameObject pistolIcon;
    [SerializeField] private GameObject submachineIcon;
    [SerializeField] private GameObject shotgunIcon;
    [SerializeField] private GameObject sniperIcon;

    [Header("GameOver")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject menuButton;

    //References
    [SerializeField] private EntityStats stats;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        PlayerWallet.OnMoneyChanged += UpdateMoneyText; //Must subscribe in OnEnable to get PlayerWallet Start() value

        Weapon.OnWeaponEquipped += UpdateWeaponName;
        Weapon.OnAmmoChanged += UpdateAmmoText;
        Weapon.OnReloadStart += StartReloadVisual;

        if (GameManager.Instance != null) SubscribeToGameManager();

        if (stats != null) stats.OnPlayerDeath += StartGameOverScreen;
    }

    void Start()
    {
        if (GameManager.Instance != null) SubscribeToGameManager(); //GameManager events trigger in Start(), causing subscription conflict in OnEnable
    }

    void Update()
    {
        if (reloadHUD != null && reloadHUD.activeSelf) FollowMouseCursor();
    }

    void SubscribeToGameManager()
    {
        //Prevents duplication if OnEnable() subscription works
        GameManager.Instance.OnWaveChanged -= UpdateWaveText;
        GameManager.Instance.OnGamePhaseChanged -= UpdateGamePhaseUI;
        GameManager.Instance.OnGameStatusChanged -= UpdateGameStatusUI;

        GameManager.Instance.OnWaveChanged += UpdateWaveText;
        GameManager.Instance.OnGamePhaseChanged += UpdateGamePhaseUI;
        GameManager.Instance.OnGameStatusChanged += UpdateGameStatusUI;

        UpdateGamePhaseUI(GameManager.Instance.CurrentPhase);
    }

    void UpdateWaveText(int currentWave)
    {
        waveText.text = "Wave " + currentWave.ToString();
    }

    void UpdateMoneyText(int currentMoney)
    {
        moneyText.text = currentMoney.ToString();
    }

    void UpdateGamePhaseUI(GamePhase newPhase)
    {
        if (newPhase == GamePhase.Preparation)
        {
            preparationUI.SetActive(true);
        }
        else if (newPhase == GamePhase.Combat)
        {
            preparationUI.SetActive(false);
        }
    }

    void UpdateGameStatusUI(GameStatus newStatus)
    {
        if (newStatus == GameStatus.Paused)
        {
            pauseScreen.SetActive(true);
            gameOverScreen.SetActive(false);
        }
        else if (newStatus == GameStatus.GameOver)
        {
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(true);
        }
        else
        {
            pauseScreen.SetActive(false);
            gameOverScreen.SetActive(false);
        }
    }

    public void OptionsButton()
    {
        pauseScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void CloseButton()
    {
        pauseScreen.SetActive(true);
        optionsScreen.SetActive(false);
    }

    public void OpenShopUI()
    {
        shopScreen.SetActive(true);
        shopButton.SetActive(false);
        startWaveButton.SetActive(false);
    }

    public void CloseShopUI()
    {
        shopScreen.SetActive(false);
        shopButton.SetActive(true);
        startWaveButton.SetActive(true);
    }

    void StartGameOverScreen()
    {
        StartCoroutine(GameOverAnimation());
        Debug.Log("Start GameOver animation");
    }

    IEnumerator GameOverAnimation()
    {
        gameOverScreen.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        restartButton.SetActive(false);
        menuButton.SetActive(false);

        preparationUI.SetActive(false);
        waveText.text = "";
        weaponHUD.SetActive(false); 

        Color tempColor = background.color;
        tempColor.a = 0;
        background.color = tempColor;

        float duration = 2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            //Calculates time percentage (normalised from 0 to 1)
            float progress = timer/duration;

            tempColor.a = Mathf.Lerp(0f, 1f, progress);
            background.color = tempColor;

            yield return null;  
        }

        //Ensures it ends up 100% opaque
        tempColor.a = 1f;
        background.color = tempColor;

        gameOverText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        restartButton.SetActive(true);
        menuButton.SetActive(true);

        //Small animation for the button
        restartButton.transform.localScale = Vector3.zero;
        menuButton.transform.localScale = Vector3.zero;

        float buttonTimer = 0;
        while (buttonTimer < 0.4)
        {
            buttonTimer += Time.deltaTime;
            restartButton.transform.localScale = Vector3.one * (buttonTimer / 0.4f);
            menuButton.transform.localScale = Vector3.one * (buttonTimer / 0.4f);
            yield return null;
        }
        restartButton.transform.localScale = Vector3.one;
        menuButton.transform.localScale = Vector3.one;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    // ----- WEAPON HUD -----
    void UpdateWeaponName(string name)
    {
        weaponNameText.text = name;

        //Stops the old coroutine when switching weapons
        if (currentReloadCoroutine != null)
        {
            StopCoroutine(currentReloadCoroutine);
            currentReloadCoroutine = null;
        }

        //Reset reload visuals when switching weapons
        if (reloadHUD != null) reloadHUD.SetActive(false);
        reloadFillImage.fillAmount = 0;

        UpdateWeaponIcon(name);
    }

    void UpdateWeaponIcon(string name)
    {
        if (name == "Pistol")
        {
            pistolIcon.SetActive(true);
            submachineIcon.SetActive(false);
            shotgunIcon.SetActive(false);
            sniperIcon.SetActive(false);
        }
        else if (name == "SMG")
        {
            pistolIcon.SetActive(false);
            submachineIcon.SetActive(true);
            shotgunIcon.SetActive(false);
            sniperIcon.SetActive(false);
        }
        else if (name == "Shotgun")
        {
            pistolIcon.SetActive(false);
            submachineIcon.SetActive(false);
            shotgunIcon.SetActive(true);
            sniperIcon.SetActive(false);
        }
        else if (name == "Sniper")
        {
            pistolIcon.SetActive(false);
            submachineIcon.SetActive(false);
            shotgunIcon.SetActive(false);
            sniperIcon.SetActive(true);
        }
    }

    void UpdateAmmoText(int current, int max)
    {
        ammoText.text = $"{current}/{max}";

        if (current == 0) ammoText.color = Color.red; //Visual feedback: change colour when bullets are running out 
        else ammoText.color = Color.white;
    }

    void StartReloadVisual(float reloadTime)
    {
        if (currentReloadCoroutine != null) StopCoroutine(currentReloadCoroutine); //Stops the previous coroutine before starting new one

        //Reset reload visuals
        reloadFillImage.fillAmount = 0;
        if (reloadHUD != null) reloadHUD.SetActive(true);

        currentReloadCoroutine = StartCoroutine(ReloadAnimationRoutine(reloadTime)); //Starts new coroutine and saves the reference
    }

    IEnumerator ReloadAnimationRoutine(float reloadTime)
    {
        if (reloadHUD != null) reloadHUD.SetActive(true);
        reloadFillImage.fillAmount = 0;
        Cursor.visible = false;

        float timer = 0;

        while (timer < reloadTime)
        {
            timer += Time.deltaTime;

            //Fills from 0 to 1 based on time percentage
            reloadFillImage.fillAmount = timer / reloadTime;
            yield return null;
        }

        reloadFillImage.fillAmount = 1;

        yield return new WaitForSeconds(0.2f);
        if (reloadHUD != null) reloadHUD.SetActive(false);
        Cursor.visible = true;

        currentReloadCoroutine = null; //Clears the reference
    }

    void FollowMouseCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        reloadHUD.transform.position = mousePos;
    }

    //--------------------------

    void OnDisable()
    {
        Weapon.OnWeaponEquipped -= UpdateWeaponName;
        Weapon.OnAmmoChanged -= UpdateAmmoText;
        Weapon.OnReloadStart -= StartReloadVisual;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged -= UpdateWaveText;
            GameManager.Instance.OnGamePhaseChanged -= UpdateGamePhaseUI;
            GameManager.Instance.OnGameStatusChanged -= UpdateGameStatusUI;
        }

        PlayerWallet.OnMoneyChanged -= UpdateMoneyText;

        if (stats != null) stats.OnPlayerDeath -= StartGameOverScreen;
    }
}
 