using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("PowerUp Prefabs")]
    public GameObject alyuvarPrefab;        // Alyuvar prefabı
    public GameObject glikozPrefab;         // Glikoz prefabı
    public GameObject norotransmitterPrefab; // Nörotransmitter prefabı
    
    [Header("Spawn Settings")]
    public float minSpawnTime = 5f;    // Minimum oluşturma süresi
    public float maxSpawnTime = 15f;   // Maximum oluşturma süresi
    
    public float minX = -8f;           // Oluşturma alanının sol sınırı
    public float maxX = 8f;            // Oluşturma alanının sağ sınırı
    public float spawnY = 6f;          // Oluşturma yüksekliği
    
    [Header("Spawn Duration")]
    public float spawnDuration = 30f;  // Toplam buff oluşturma süresi (saniye)
    private float elapsedTime = 0f;    // Geçen süre
    private bool isSpawningActive = true; // Buff oluşturma aktif mi?
    
    [Header("Spawn Probabilities (%)")]
    [Range(0, 100)]
    public int alyuvarChance = 33;        // Alyuvar oluşturma şansı
    [Range(0, 100)]
    public int glikozChance = 33;         // Glikoz oluşturma şansı
    [Range(0, 100)]
    public int norotransmitterChance = 34; // Nörotransmitter oluşturma şansı
    
    private float timeUntilSpawn;      // Bir sonraki oluşturma için kalan süre
    
    void Start()
    {
        // İlk güçlendirme oluşturma zamanını ayarla
        ResetSpawnTime();
        
        // Şansların toplamının 100 olduğunu kontrol et
        int totalChance = alyuvarChance + glikozChance + norotransmitterChance;
        if (totalChance != 100)
        {
            Debug.LogWarning("PowerUpSpawner: Şansların toplamı 100 değil! (" + totalChance + ")");
        }
    }
    
    void Update()
    {
        // Toplam süreyi ölç
        elapsedTime += Time.deltaTime;
        
        // Eğer belirlenen süre dolmuşsa, güçlendirme oluşturmayı durdur
        if (elapsedTime >= spawnDuration)
        {
            isSpawningActive = false;
            return;
        }
        
        // Güçlendirme oluşturma aktif değilse çık
        if (!isSpawningActive)
            return;
            
        // Zamanlayıcıyı azalt
        timeUntilSpawn -= Time.deltaTime;
        
        // Zaman 0'ın altına düşerse güçlendirme oluştur
        if (timeUntilSpawn <= 0)
        {
            SpawnRandomPowerUp();
            ResetSpawnTime();
        }
    }
    
    void SpawnRandomPowerUp()
    {
        // Rastgele X konumu seç
        float randomX = Random.Range(minX, maxX);
        
        // Oluşturma pozisyonu
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);
        
        // Güçlendirme tipini şanslara göre belirle
        int randomValue = Random.Range(1, 101); // 1-100 arası rastgele değer
        GameObject prefabToSpawn = null;
        
        // Hangi prefabın oluşturulacağını belirle
        if (randomValue <= alyuvarChance)
        {
            prefabToSpawn = alyuvarPrefab;
        }
        else if (randomValue <= alyuvarChance + glikozChance)
        {
            prefabToSpawn = glikozPrefab;
        }
        else
        {
            prefabToSpawn = norotransmitterPrefab;
        }
        
        // Eğer prefab atanmışsa oluştur
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("PowerUpSpawner: Oluşturulacak prefab atanmamış!");
        }
    }
    
    void ResetSpawnTime()
    {
        // Rastgele bir sonraki oluşturma zamanını ayarla
        timeUntilSpawn = Random.Range(minSpawnTime, maxSpawnTime);
    }
    
    // Güçlendirme oluşturmayı yeniden başlat (public, diğer scriptlerden erişilebilir)
    public void RestartSpawning()
    {
        elapsedTime = 0f;
        isSpawningActive = true;
        ResetSpawnTime();
    }
} 