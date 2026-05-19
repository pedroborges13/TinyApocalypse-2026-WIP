using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifeTime;
    private float sizeModifier = 1f;

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

    public void Setup(int amount)
    {
        if (textMesh != null)
        {
            textMesh.text = $"+${amount}";
        }

        transform.localScale = Vector3.one * sizeModifier;

        rectTransform.anchoredPosition = Vector2.zero;

        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    // Update is called once per frame
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
