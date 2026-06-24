using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifeTime;
    //private float sizeModifier = 1f;

    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private IObjectPool<FloatingText> pool;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetPool(IObjectPool<FloatingText> pool)
    {
        this.pool = pool;
    }

    public void Setup(int amount, float sizeModifier)
    {
        if (textMesh != null)
        {
            textMesh.text = $"+${amount}";
        }

        transform.localScale = Vector3.one * sizeModifier;

        //Offset
        float randomX = Random.Range(-25f, 25f);
        float randomY = Random.Range(-40f, 40f);

        rectTransform.anchoredPosition = new Vector2(randomX,randomY);

        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
    }

    private void ReturnToPool()
    {
        if (pool != null) pool.Release(this);
        else Destroy(gameObject);
    }
}
