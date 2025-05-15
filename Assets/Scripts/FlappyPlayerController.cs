using UnityEngine;
using UnityEngine.UI;

public class FlappyPlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;         // Zıplama kuvveti
    public float gravityScale = 2f;      // Yerçekimi çarpanı
    public float maxVelocity = 10f;      // Maksimum düşüş hızı
    
    [Header("Controls")]
    public Button jumpButton;            // Mobil için zıplama butonu
    public bool useKeyboardControls = true; // Klavye kontrollerini kullan
    
    [Header("Audio")]
    public AudioClip jumpSound;          // Zıplama sesi
    public AudioClip hitSound;           // Çarpışma sesi
    
    private Rigidbody2D rb;              // Fizik bileşeni
    private AudioSource audioSource;     // Ses kaynağı
    private bool isDead = false;         // Ölüm durumu
    
    // Referans
    private FlappyGameManager gameManager;
    
    void Start()
    {
        // Bileşenleri al
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<FlappyGameManager>();
        
        // Bileşenler yoksa ekle
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = gravityScale;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        
        if (audioSource == null && (jumpSound != null || hitSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Rigidbody ayarları
        rb.gravityScale = gravityScale;
        
        // Mobil zıplama butonu event listener
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(Jump);
        }
    }
    
    void Update()
    {
        // Ölmüşse işlem yapma
        if (isDead) return;
        
        // Klavye kontrolü - Space tuşu veya Sol fare tıklaması ile zıplama
        if (useKeyboardControls && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            Jump();
        }
        
        // Hız sınırlandırması
        if (rb.linearVelocity.y < -maxVelocity)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxVelocity);
        }
    }
    
    // Zıplama fonksiyonu
    public void Jump()
    {
        if (isDead) return;
        
        // Dikey hızı sıfırla ve kuvvet uygula
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        // Zıplama sesi çal
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Engele çarptığında
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!isDead)
            {
                Die();
            }
        }
    }
    
    // Ölüm fonksiyonu
    void Die()
    {
        isDead = true;
        
        // Çarpışma sesi çal
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        
        // Game Over olayını tetikle
        if (gameManager != null)
        {
            gameManager.GameOver();
            Debug.Log("GameOver çağrıldı");
        }
        else
        {
            Debug.LogError("FlappyGameManager bulunamadı!");
        }
    }
} 