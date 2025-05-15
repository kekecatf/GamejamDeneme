using UnityEngine;

public class PickupEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    public float growSpeed = 5f;          // Büyüme hızı
    public float maxScale = 3f;           // Maksimum ölçek
    public float fadeSpeed = 2f;          // Solma hızı
    
    private SpriteRenderer spriteRenderer;
    private float initialAlpha;
    private Vector3 initialScale;
    
    void Start()
    {
        // SpriteRenderer bileşenini al
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Başlangıç değerlerini kaydet
        if (spriteRenderer != null)
        {
            initialAlpha = spriteRenderer.color.a;
        }
        
        initialScale = transform.localScale;
    }
    
    void Update()
    {
        // Efekti büyüt
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale * maxScale, Time.deltaTime * growSpeed);
        
        // Efekti soldur (alpha değerini azalt)
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * fadeSpeed);
            spriteRenderer.color = color;
            
            // Tamamen şeffaf olduğunda yok et
            if (color.a < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }
} 