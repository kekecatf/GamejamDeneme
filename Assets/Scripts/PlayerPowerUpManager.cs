using System.Collections;
using UnityEngine;

public class PlayerPowerUpManager : MonoBehaviour
{
    [Header("Speed Boost (Alyuvar)")]
    public float speedBoostDuration = 3f;     // Hız artışı süresi (saniye)
    public float speedBoostMultiplier = 1.5f; // Hız artışı çarpanı (1.5 = %50 artış)
    private bool isSpeedBoosted = false;
    private float speedBoostEndTime = 0f;     // Hız artışının biteceği zaman
    private float originalPlayerSpeed;         // Orijinal hareket hızı
    
    [Header("Extra Time (Glikoz)")]
    public float extraTimeAmount = 2f;        // Eklenecek ek süre (saniye)
    
    [Header("Ghost Mode (Nörotransmitter)")]
    public float ghostModeDuration = 4f;      // Engellerin içinden geçebilme süresi (saniye)
    public Color ghostModeColor = new Color(0.5f, 0.5f, 1f, 0.7f); // Yarı şeffaf mavi renk
    private bool isGhostMode = false;         // Şu anda içinden geçilebilir mi?
    private SpriteRenderer spriteRenderer;    // Sprite renderer
    private float ghostModeEndTime = 0f;      // Ghost mode'un biteceği zaman
    
    [Header("Pickup Effects")]
    public GameObject pickupEffectPrefab;     // Pickup efekti prefabı
    public float effectDuration = 1f;         // Efekt süresi (saniye)
    public Color alyuvarEffectColor = new Color(1f, 0f, 0f, 0.7f);  // Alyuvar efekt rengi
    public Color glikozEffectColor = new Color(0f, 1f, 0f, 0.7f);   // Glikoz efekt rengi
    public Color norotransmitterEffectColor = new Color(0f, 0f, 1f, 0.7f); // Nörotransmitter efekt rengi
    
    [Header("Movement")]
    public float speed = 5f;
    public Joystick joystick;
    public bool useRigidbody = true;         // Rigidbody2D kullanmak için
    
    private GameManager gameManager;
    private SkillManager skillManager;
    private Rigidbody2D rb;                  // Hareket için Rigidbody2D
    
    // Engellerden geçebilme özelliği için
    private string originalTag;               // Orijinal tag
    private Collider2D playerCollider;        // Oyuncu collider
    private Color originalColor = Color.white; // Başlangıç orijinal renk (güvenlik için)
    
    // Görünmezlik sorununu tespit için
    private bool rendererWasNull = false;

    void Update()
    {
        // Düzenli kontroller Update'de kalsın, hareket FixedUpdate'e taşındı
        
        // Görünmezlik kontrolü - her frame'de, sprite renderer'ın düzgün olduğundan emin olalım
        CheckRendererVisibility();
        
        // Ghost mode süresi kontrol - zamanlayıcı ile ek güvenlik
        if (isGhostMode && Time.time > ghostModeEndTime)
        {
            // Süre dolmuş ama hala ghost modundaysak, zorla sıfırla
            Debug.LogWarning("Ghost mode süresi doldu ama hala aktif! Zorla kapatılıyor...");
            StopCoroutine("GhostModeCoroutine");
            EndGhostMode();
        }
        
        // Speed boost süresi kontrol - zamanlayıcı ile ek güvenlik
        if (isSpeedBoosted && Time.time > speedBoostEndTime)
        {
            // Süre dolmuş ama hala hız artışı aktifse, zorla sıfırla
            Debug.LogWarning("Speed boost süresi doldu ama hala aktif! Zorla kapatılıyor...");
            StopCoroutine("SpeedBoostCoroutine");
            EndSpeedBoost();
        }
    }
    
    void FixedUpdate()
    {
        // Fizik temelli hareket için FixedUpdate kullanımı
        if (joystick == null) return;
        
        // Joystick girdisini al
        Vector2 movement = new Vector2(joystick.Horizontal, 0);
        
        if (useRigidbody && rb != null)
        {
            // Rigidbody2D ile hareket et
            rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
        }
        else
        {
            // Transform ile hareket et (eski yöntem)
            transform.Translate(movement * speed * Time.fixedDeltaTime);
        }
    }
    
    // Her frame'de sprite renderer ve görünürlük kontrolü
    private void CheckRendererVisibility()
    {
        if (spriteRenderer == null)
        {
            // Sprite renderer yok, tekrar bulmaya çalış
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer == null)
            {
                // Bulunamadı, daha önce de bulunamadıysa tekrar log yazmaya gerek yok
                if (!rendererWasNull)
                {
                    Debug.LogError("SpriteRenderer bulunamadı! Oyuncu görünmüyor olabilir!");
                    rendererWasNull = true;
                }
                return;
            }
            else
            {
                // Sprite renderer bulundu, orijinal rengi ata
                Debug.Log("SpriteRenderer yeniden bulundu!");
                rendererWasNull = false;
            }
        }
        
        // Sprite renderer var ama enabled değil mi?
        if (!spriteRenderer.enabled)
        {
            Debug.LogWarning("SpriteRenderer devre dışı! Etkinleştiriliyor...");
            spriteRenderer.enabled = true;
        }
        
        // Alfa değeri sıfıra yakın mı (görünmez mi)?
        if (spriteRenderer.color.a < 0.1f && !isGhostMode)
        {
            Debug.LogWarning("Oyuncu görünmez! Ghost mode aktif değil, renk sıfırlanıyor...");
            spriteRenderer.color = originalColor;
        }
    }
    
    private void Start()
    {
        // Rigidbody2D bileşenini al
        rb = GetComponent<Rigidbody2D>();
        if (rb == null && useRigidbody)
        {
            Debug.LogWarning("Rigidbody2D bulunamadı! Transform ile hareket edilecek.");
            useRigidbody = false;
        }
        
        // Referansları al
        gameManager = FindFirstObjectByType<GameManager>();
        skillManager = GetComponent<SkillManager>();
        
        // Sprite renderer ve collider referansını al
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        
        // Orijinal değerleri kaydet
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            Debug.Log("Orijinal oyuncu rengi kaydedildi: " + originalColor);
        }
        else
        {
            Debug.LogError("SpriteRenderer bulunamadı! Ghost Mode görsel efekti çalışmayabilir.");
        }
        
        // Joystick kontrolü
        if (joystick == null)
        {
            joystick = FindObjectOfType<Joystick>();
            if (joystick == null)
            {
                Debug.LogError("Joystick bulunamadı! Hareket etmek mümkün olmayacak.");
            }
        }
        
        originalTag = gameObject.tag;
        originalPlayerSpeed = speed;
    }
    
    // Görünürlük sorununu çözmek için OnDisable ve OnEnable metotları ekleyelim
    private void OnDisable()
    {
        // Eğer oyuncu devre dışı kalırsa, görünürlüğü ve collider ayarlarını sıfırla
        ResetVisualEffects();
    }
    
    private void OnEnable()
    {
        // Eğer oyuncu aktif olursa, görünürlüğü ve collider ayarlarını sıfırla
        ResetVisualEffects();
    }
    
    // Oyun duraklatıldığında/devam ettiğinde kontrol et
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Oyun devam ediyor
        {
            // Oyun devam ettiğinde görünürlüğü kontrol et
            ResetVisualEffects();
        }
    }
    
    // Renk ve görünürlük ayarlarını sıfırlama
    private void ResetVisualEffects()
    {
        // Coroutine'leri durdur
        StopAllCoroutines();
        
        // Sprite renderer'ı kontrol et
        if (spriteRenderer != null)
        {
            // Renderer'ı etkinleştir
            spriteRenderer.enabled = true;
            
            // Rengi orijinale sıfırla
            spriteRenderer.color = originalColor;
            Debug.Log("Oyuncu rengi sıfırlandı: " + originalColor);
        }
        else
        {
            // SpriteRenderer yok, tekrar bulmaya çalış
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.color = originalColor;
                Debug.Log("SpriteRenderer yeniden bulundu ve sıfırlandı!");
            }
        }
        
        // Collider'ı kontrol et
        if (playerCollider != null)
        {
            playerCollider.isTrigger = false;
        }
        else
        {
            // Collider yok, tekrar bulmaya çalış
            playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.isTrigger = false;
                Debug.Log("Collider yeniden bulundu ve sıfırlandı!");
            }
        }
        
        // Speed boost'u kapat
        if (isSpeedBoosted)
        {
            speed = originalPlayerSpeed;
            isSpeedBoosted = false;
        }
        
        // Ghost mode'u kapat
        isGhostMode = false;
    }
    
    // Hız artışı uygula
    public void ApplySpeedBoost()
    {
        // Toplama efekti göster
        PlayPickupEffect(alyuvarEffectColor);
        
        // Zaten hız artışı aktifse
        if (isSpeedBoosted)
        {
            // Sadece süreyi uzat, hızı tekrar artırma
            Debug.Log("Alyuvar toplandı! Mevcut hız artışının süresi uzatıldı.");
            
            // Coroutine hala çalışıyorsa durdur
            StopCoroutine("SpeedBoostCoroutine");
            
            // Bitiş zamanını güncelle (süreyi uzat)
            speedBoostEndTime = Time.time + speedBoostDuration;
            
            // Yeni bir coroutine başlat
            StartCoroutine(SpeedBoostCoroutine(false)); // false = hızı tekrar artırma
        }
        else
        {
            // İlk kez hız artışı alıyor
            Debug.Log("Alyuvar toplandı! Hız arttı!");
            
            // Bitiş zamanını ayarla
            speedBoostEndTime = Time.time + speedBoostDuration;
            
            // Coroutine'i başlat
            StartCoroutine(SpeedBoostCoroutine(true)); // true = hızı artır
        }
    }
    
    // Ek süre ekle
    public void AddExtraTime()
    {
        // Toplama efekti göster
        PlayPickupEffect(glikozEffectColor);
        
        // GameManager varsa süre ekle
        if (gameManager != null)
        {
            gameManager.AddTime(extraTimeAmount);
        }
        
        Debug.Log("Glikoz toplandı! " + extraTimeAmount + " saniye eklendi!");
    }
    
    // Engellerden geçebilme özelliği ekle
    public void ActivateGhostMode()
    {
        // Toplama efekti göster
        PlayPickupEffect(norotransmitterEffectColor);
        
        // Zaten ghost mode aktifse
        if (isGhostMode)
        {
            // Sadece süreyi uzat, efekti tekrar uygulamayalım
            Debug.Log("Nörotransmitter Kristali toplandı! Mevcut ghost mode süresi uzatıldı.");
            
            // Coroutine hala çalışıyorsa durdur
            StopCoroutine("GhostModeCoroutine");
            
            // Bitiş zamanını güncelle (süreyi uzat)
            ghostModeEndTime = Time.time + ghostModeDuration;
            
            // Yeni bir coroutine başlat
            StartCoroutine(GhostModeCoroutine(false)); // false = efekti tekrar uygulama
        }
        else
        {
            // İlk kez ghost mode aktivasyonu
            Debug.Log("Nörotransmitter Kristali toplandı! " + ghostModeDuration + " saniye boyunca engellerden geçebilirsiniz!");
            
            // Bitiş zamanını ayarla
            ghostModeEndTime = Time.time + ghostModeDuration;
            
            // Coroutine'i başlat
            StartCoroutine(GhostModeCoroutine(true)); // true = görsel ve fiziksel efektleri uygula
        }
    }
    
    // Toplama efekti oynat
    private void PlayPickupEffect(Color effectColor)
    {
        // Eğer efekt prefabı ayarlanmışsa
        if (pickupEffectPrefab != null)
        {
            // Efekt objesini oluştur
            GameObject effectObj = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            
            // Efekt rengini ayarla
            SpriteRenderer effectRenderer = effectObj.GetComponent<SpriteRenderer>();
            if (effectRenderer != null)
            {
                effectRenderer.color = effectColor;
            }
            else
            {
                // Particle sistem kullanılıyorsa
                ParticleSystem particleSystem = effectObj.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    ParticleSystem.MainModule main = particleSystem.main;
                    main.startColor = effectColor;
                }
            }
            
            // Belirli süre sonra yok et
            Destroy(effectObj, effectDuration);
        }
        else
        {
            Debug.LogWarning("Pickup efekt prefabı ayarlanmamış!");
        }
    }
    
    // Hız artışı coroutine
    private IEnumerator SpeedBoostCoroutine(bool shouldIncreaseSpeed)
    {
        // Hız artışı başlat
        isSpeedBoosted = true;
        
        // Eğer yeni bir hız artışı ise (stacklenme değil)
        if (shouldIncreaseSpeed)
        {
            // Orijinal hızı kaydet ve hızı artır
            speed = originalPlayerSpeed * speedBoostMultiplier;
        }
        
        // Süre kadar bekle (bu sabit bir süre değil, güncellenen bitiş zamanına göre)
        float remainingTime = speedBoostEndTime - Time.time;
        yield return new WaitForSeconds(remainingTime);
        
        // Hızı normale döndür
        EndSpeedBoost();
    }
    
    // Hız artışını sonlandır
    private void EndSpeedBoost()
    {
        // Hızı normale döndür
        speed = originalPlayerSpeed;
        isSpeedBoosted = false;
        Debug.Log("Alyuvar hız artışı etkisi sona erdi.");
    }
    
    // Engellerden geçebilme modu coroutine
    private IEnumerator GhostModeCoroutine(bool shouldApplyEffects)
    {
        // Engellerden geçiş modunu başlat
        isGhostMode = true;
        
        if (shouldApplyEffects)
        {
            // SpriteRenderer var mı kontrol et
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    // Yeni bulduk, orijinal rengi kaydet
                    originalColor = spriteRenderer.color;
                }
            }
            
            // Görsel efekt - sprite rengini değiştir
            if (spriteRenderer != null && spriteRenderer.enabled)
            {
                Debug.Log("Ghost mode rengi uygulanıyor...");
                spriteRenderer.color = ghostModeColor;
            }
            else
            {
                Debug.LogWarning("SpriteRenderer bulunamadı veya devre dışı! Görsel efekt çalışmayacak.");
                if (spriteRenderer != null && !spriteRenderer.enabled)
                {
                    spriteRenderer.enabled = true;
                    spriteRenderer.color = ghostModeColor;
                }
            }
            
            // Fiziksel değişiklikler
            // Engeller ile çarpışmayı devre dışı bırak
            if (playerCollider != null)
            {
                // Engellere karşı çarpışmayı kapat
                playerCollider.isTrigger = true;
            }
            else
            {
                playerCollider = GetComponent<Collider2D>();
                if (playerCollider != null)
                {
                    playerCollider.isTrigger = true;
                }
                else
                {
                    Debug.LogWarning("Collider bulunamadı! Fiziksel efekt çalışmayacak.");
                }
            }
            
            // Ghost mode başladı log
            Debug.Log("Ghost mode aktifleştirildi, " + ghostModeDuration + " saniye aktif.");
        }
        
        // Süre beklemesi
        float remainingTime = ghostModeEndTime - Time.time;
        yield return new WaitForSeconds(remainingTime);
        
        // Ghost mode'u kapat
        EndGhostMode();
    }
    
    // Ghost mode'u güvenli bir şekilde sonlandır
    private void EndGhostMode()
    {
        // Normal moda geri dön
        isGhostMode = false;
        
        // Görsel efekti kapat
        if (spriteRenderer != null)
        {
            Debug.Log("Orijinal renge geri dönülüyor: " + originalColor);
            spriteRenderer.color = originalColor;
            
            // Renderer'ın etkin olduğundan emin ol
            if (!spriteRenderer.enabled)
            {
                spriteRenderer.enabled = true;
            }
        }
        else
        {
            // SpriteRenderer kaybolmuş, tekrar bulmaya çalış
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.color = originalColor;
            }
        }
        
        // Çarpışmaları tekrar aktifleştir
        if (playerCollider != null)
        {
            playerCollider.isTrigger = false;
        }
        else
        {
            // Collider kaybolmuş, tekrar bulmaya çalış
            playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.isTrigger = false;
            }
        }
        
        Debug.Log("Nörotransmitter etkisi sona erdi. Artık engellere çarpabilirsiniz!");
    }
    
    // Engellerden geçebilme modunda mıyız?
    public bool IsInGhostMode()
    {
        return isGhostMode;
    }
} 