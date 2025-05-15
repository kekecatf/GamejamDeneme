using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        Alyuvar,        // Geçici hız artışı
        Glikoz,         // Ek süre
        Norotransmitter // Engellerden geçebilme özelliği (eski: Cooldown azaltma)
    }
    
    public PowerUpType powerUpType;   // Güçlendirme türü
    public float speed = 3f;          // Düşme hızı
    
    private void Start()
    {
        // Başlangıçta buff türünü log'a yaz
        Debug.Log(gameObject.name + " başlatıldı. Tür: " + powerUpType);
        
        // Collider var mı kontrol et
        Collider2D collider2D = GetComponent<Collider2D>();
        Collider collider3D = GetComponent<Collider>();
        
        if (collider2D == null && collider3D == null)
        {
            Debug.LogError(gameObject.name + " objesinde Collider bileşeni yok! Çarpışma algılanmayacak.");
        }
        else
        {
            if (collider2D != null && !collider2D.isTrigger)
            {
                Debug.LogWarning(gameObject.name + " objesindeki Collider2D'nin 'Is Trigger' özelliği aktif değil!");
            }
            
            if (collider3D != null && !collider3D.isTrigger)
            {
                Debug.LogWarning(gameObject.name + " objesindeki Collider'ın 'Is Trigger' özelliği aktif değil!");
            }
        }
    }
    
    void Update()
    {
        // Aşağı doğru hareket et
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        
        // Ekranın altından çıkınca yok et
        if (transform.position.y < -6f)
        {
            Debug.Log(gameObject.name + " ekranın altına düştüğü için yok edildi.");
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject.name + " için OnTriggerEnter2D çağrıldı. Çarpışan: " + other.gameObject.name);
        
        // Oyuncu ile çarpışma kontrolü
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player ile çarpışma algılandı! PowerUp uygulanıyor...");
            
            // Güçlendirme etkisini uygula
            ApplyPowerUp(other.gameObject);
            
            // Güçlendirmeyi yok et
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Çarpışılan nesne Player tag'ine sahip değil: " + other.gameObject.name + " - Tag: " + other.tag);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " için OnTriggerEnter çağrıldı. Çarpışan: " + other.gameObject.name);
        
        // 3D fizik için
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player ile çarpışma algılandı! PowerUp uygulanıyor...");
            ApplyPowerUp(other.gameObject);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Çarpışılan nesne Player tag'ine sahip değil: " + other.gameObject.name + " - Tag: " + other.tag);
        }
    }
    
    void ApplyPowerUp(GameObject player)
    {
        Debug.Log("ApplyPowerUp çağrıldı: " + powerUpType + " tipinde güçlendirme uygulanıyor...");
        
        PlayerPowerUpManager powerUpManager = player.GetComponent<PlayerPowerUpManager>();
        
        // Eğer oyuncuda PowerUpManager yoksa, oluştur
        if (powerUpManager == null)
        {
            Debug.LogWarning("Oyuncuda PowerUpManager bulunamadı! Otomatik oluşturuluyor...");
            powerUpManager = player.AddComponent<PlayerPowerUpManager>();
        }
        
        // Güçlendirme türüne göre işlem yap
        switch (powerUpType)
        {
            case PowerUpType.Alyuvar:
                Debug.Log("Alyuvar güçlendirmesi uygulanıyor... (%50 hız artışı, 3 saniye)");
                powerUpManager.ApplySpeedBoost();
                break;
                
            case PowerUpType.Glikoz:
                Debug.Log("Glikoz güçlendirmesi uygulanıyor... (+2 saniye ek süre)");
                powerUpManager.AddExtraTime();
                break;
                
            case PowerUpType.Norotransmitter:
                Debug.Log("Nörotransmitter güçlendirmesi uygulanıyor... (4 saniye engellerden geçebilme)");
                powerUpManager.ActivateGhostMode();
                break;
        }
    }
} 