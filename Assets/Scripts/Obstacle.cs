using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;
    
    void Update()
    {
        // Aşağı doğru hareket et (Y ekseninde negatif yönde)
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        
        // Ekranın altından çıkınca engeli yok et
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
    
    // 2D fizik için çarpışma kontrolü
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Oyuncu ghost modunda mı kontrol et
            PlayerPowerUpManager playerPowerUp = collision.gameObject.GetComponent<PlayerPowerUpManager>();
            if (playerPowerUp != null && playerPowerUp.IsInGhostMode())
            {
                // Ghost modundaysa çarpışmayı yok say
                Debug.Log("Oyuncu ghost modunda olduğu için engele çarpmayı yok sayıyoruz.");
                return;
            }
            
            // Oyuncu ile çarpıştık, oyunu durdur
            GameOver();
        }
    }
    
    // 3D fizik için çarpışma kontrolü
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Oyuncu ghost modunda mı kontrol et
            PlayerPowerUpManager playerPowerUp = collision.gameObject.GetComponent<PlayerPowerUpManager>();
            if (playerPowerUp != null && playerPowerUp.IsInGhostMode())
            {
                // Ghost modundaysa çarpışmayı yok say
                Debug.Log("Oyuncu ghost modunda olduğu için engele çarpmayı yok sayıyoruz.");
                return;
            }
            
            // Oyuncu ile çarpıştık, oyunu durdur
            GameOver();
        }
    }
    
    // Trigger collider için çarpışma kontrolü
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncu ghost modunda mı kontrol et
            PlayerPowerUpManager playerPowerUp = other.GetComponent<PlayerPowerUpManager>();
            if (playerPowerUp != null && playerPowerUp.IsInGhostMode())
            {
                // Ghost modundaysa çarpışmayı yok say
                Debug.Log("Oyuncu ghost modunda olduğu için engele çarpmayı yok sayıyoruz.");
                return;
            }
            
            // Oyuncu ile çarpıştık, oyunu durdur
            GameOver();
        }
    }
    
    // 3D Trigger için çarpışma kontrolü
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncu ghost modunda mı kontrol et
            PlayerPowerUpManager playerPowerUp = other.GetComponent<PlayerPowerUpManager>();
            if (playerPowerUp != null && playerPowerUp.IsInGhostMode())
            {
                // Ghost modundaysa çarpışmayı yok say
                Debug.Log("Oyuncu ghost modunda olduğu için engele çarpmayı yok sayıyoruz.");
                return;
            }
            
            // Oyuncu ile çarpıştık, oyunu durdur
            GameOver();
        }
    }
    
    void GameOver()
    {
        // Oyun zamanını 0'a ayarla (oyunu durdur)
        Time.timeScale = 0;
        
        // GameManager'ı bul ve oyun bitti fonksiyonunu çağır
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogWarning("GameManager bulunamadı!");
        }
    }
} 