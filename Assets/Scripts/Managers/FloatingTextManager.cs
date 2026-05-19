using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Pool;
using System.Collections;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private FloatingText textPrefab;
    [SerializeField] private Transform uiTrasform;

    [Header("Dynamic Size Settings")]
    [SerializeField] private float baseScale;
    [SerializeField] private float maxScale;
    [SerializeField] private int baseRewardValue;

    //Pool
    IObjectPool<FloatingText> textPool;

    //Money Count
    int accumulatedCoins = 0;
    Coroutine accumulationCoroutine;
    [SerializeField] private float accumulationWindow = 0.05f; //Janela de frames para somar

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        textPool = new ObjectPool<FloatingText>(CreateText, GetFromPool, BackToPool, OnDestroyText, true, 5, 15);
    }

    private FloatingText CreateText()
    {
        FloatingText newText = Instantiate(textPrefab, uiTrasform);
        newText.SetPool(textPool);
        return newText;
    }

    void GetFromPool(FloatingText text) => text.gameObject.SetActive(true);

    void BackToPool(FloatingText text) => text.gameObject.SetActive(false);

    void OnDestroyText(FloatingText text) => Destroy(text.gameObject);

    public void ShowCoinText(int amount) //Called in PlayerWallet
    {
        if (textPrefab == null || uiTrasform == null) return;   

        accumulatedCoins += amount;

        if (accumulationCoroutine == null)
        {
            accumulationCoroutine = StartCoroutine(SpawnAccumulatedTextRoutine());
        }
    }

    IEnumerator SpawnAccumulatedTextRoutine()
    {
        //Wait a very short time to capture deaths in the same frame
        yield return new WaitForSeconds(accumulationWindow);

        float rawModifier = baseScale + ((float)accumulatedCoins / baseRewardValue - 1) * 0.25f;

        float finalScale = Mathf.Clamp(rawModifier, baseScale, maxScale);

        //Gets from pool and show the total value
        FloatingText textInstance = textPool.Get();
        textInstance.Setup(accumulatedCoins, finalScale);

        //Reset
        accumulatedCoins = 0;
        accumulationCoroutine = null;
    }
}
