using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;  // Engel prefabı
    
    public float minSpawnTime = 1f;   // Minimum engel oluşturma süresi
    public float maxSpawnTime = 3f;   // Maximum engel oluşturma süresi
    
    public float minX = -8f;          // Oluşturma alanının sol sınırı
    public float maxX = 8f;           // Oluşturma alanının sağ sınırı
    public float spawnY = 6f;         // Oluşturma yüksekliği
    
    [Header("Spawn Duration")]
    public float spawnDuration = 30f;  // Toplam engel oluşturma süresi (saniye)
    private float elapsedTime = 0f;    // Geçen süre
    private bool isSpawningActive = true; // Engel oluşturma aktif mi?
    
    private float timeUntilSpawn;     // Bir sonraki oluşturma için kalan süre
    
    void Start()
    {
        // İlk engel oluşturma zamanını ayarla
        ResetSpawnTime();
    }
    
    void Update()
    {
        // Toplam süreyi ölç
        elapsedTime += Time.deltaTime;
        
        // Eğer belirlenen süre dolmuşsa, engel oluşturmayı durdur
        if (elapsedTime >= spawnDuration)
        {
            isSpawningActive = false;
            return;
        }
        
        // Engel oluşturma aktif değilse çık
        if (!isSpawningActive)
            return;
            
        // Zamanlayıcıyı azalt
        timeUntilSpawn -= Time.deltaTime;
        
        // Zaman 0'ın altına düşerse engel oluştur
        if (timeUntilSpawn <= 0)
        {
            SpawnObstacle();
            ResetSpawnTime();
        }
    }
    
    void SpawnObstacle()
    {
        // Rastgele X konumu seç
        float randomX = Random.Range(minX, maxX);
        
        // Oluşturma pozisyonu
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);
        
        // Engeli oluştur
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
    
    void ResetSpawnTime()
    {
        // Rastgele bir sonraki oluşturma zamanını ayarla
        timeUntilSpawn = Random.Range(minSpawnTime, maxSpawnTime);
    }
    
    // Engel oluşturmayı yeniden başlat (public, diğer scriptlerden erişilebilir)
    public void RestartSpawning()
    {
        elapsedTime = 0f;
        isSpawningActive = true;
        ResetSpawnTime();
    }
} 