using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject obstaclePrefab;        // Engel prefab'ı
    public float obstacleSpeed = 3f;         // Engelin hareket hızı
    public float obstacleGap = 3f;           // Engeller arasındaki dikey boşluk
    public float spawnInterval = 2f;         // Engel oluşturma aralığı (saniye)
    public float obstacleWidth = 1f;         // Engel genişliği
    
    [Header("Spawn Area")]
    public float minY = -2f;                 // Minimum Y pozisyonu
    public float maxY = 2f;                  // Maksimum Y pozisyonu
    public float spawnX = 10f;               // Engellerin oluşturulacağı X pozisyonu
    public float despawnX = -10f;            // Engellerin yok edileceği X pozisyonu
    
    [Header("Difficulty")]
    public float difficultyIncreaseInterval = 10f;  // Zorluğun artacağı süre (saniye)
    public float speedIncreaseAmount = 0.2f;        // Her zorluk artışında hız artışı
    public float minSpawnIntervalLimit = 1f;        // Minimum oluşturma aralığı limiti
    
    private List<GameObject> obstacles = new List<GameObject>();
    private float timer = 0f;
    private float difficultyTimer = 0f;
    private bool isGenerating = false;
    private Coroutine generationCoroutine;
    
    void Start()
    {
        // Başlangıçta engel üretmeyi başlatma - FlappyGameManager bunu çağıracak
    }
    
    void Update()
    {
        // Tüm engelleri hareket ettir
        MoveObstacles();
        
        // Zorluk artışı zamanlaması
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }
    
    // Engel oluşturmayı başlat
    public void StartGenerating()
    {
        if (!isGenerating)
        {
            isGenerating = true;
            
            // Eğer daha önce coroutine varsa durdur
            if (generationCoroutine != null)
            {
                StopCoroutine(generationCoroutine);
            }
            
            // Yeni coroutine başlat
            generationCoroutine = StartCoroutine(GenerateObstacles());
            Debug.Log("Engel oluşturma başlatıldı.");
        }
    }
    
    private IEnumerator GenerateObstacles()
    {
        while (isGenerating)
        {
            SpawnObstacle();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    // Engel oluşturma
    private void SpawnObstacle()
    {
        // Rastgele alt engel yüksekliği belirle
        float randomHeight = Random.Range(minY, maxY - obstacleGap);
        
        // Üst engelin pozisyonu (alt engel + boşluk)
        float topObstacleY = randomHeight + obstacleGap + obstacleWidth;
        
        // Alt engel
        GameObject bottomObstacle = Instantiate(obstaclePrefab, new Vector3(spawnX, randomHeight, 0), Quaternion.identity);
        bottomObstacle.transform.parent = transform; // Düzenli tutmak için bu objenin altına koy
        
        // Üst engel
        GameObject topObstacle = Instantiate(obstaclePrefab, new Vector3(spawnX, topObstacleY, 0), Quaternion.identity);
        topObstacle.transform.parent = transform;
        
        // Engeller Obstacle tag'ine sahip olmalı
        bottomObstacle.tag = "Obstacle";
        topObstacle.tag = "Obstacle";
        
        // Fiziksel özelliklerini kontrol et
        CheckObstacleComponents(bottomObstacle);
        CheckObstacleComponents(topObstacle);
        
        // Engel listesine ekle
        obstacles.Add(bottomObstacle);
        obstacles.Add(topObstacle);
        
        Debug.Log("Yeni engeller oluşturuldu ve 'Obstacle' tag'i atandı.");
    }
    
    // Engelin gerekli bileşenlere sahip olup olmadığını kontrol et
    private void CheckObstacleComponents(GameObject obstacle)
    {
        // Collider yoksa ekle
        Collider2D collider = obstacle.GetComponent<Collider2D>();
        if (collider == null)
        {
            BoxCollider2D boxCollider = obstacle.AddComponent<BoxCollider2D>();
            Debug.LogWarning("Engele collider eklendi!");
        }
        
        // Sprite renderer'ın olduğundan emin ol
        SpriteRenderer renderer = obstacle.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.LogError("Engelde SpriteRenderer yok!");
        }
    }
    
    // Engelleri hareket ettir ve gerekirse yok et
    private void MoveObstacles()
    {
        List<GameObject> obstaclesToRemove = new List<GameObject>();
        
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null)
            {
                // Engeli sola doğru hareket ettir
                obstacle.transform.Translate(Vector3.left * obstacleSpeed * Time.deltaTime);
                
                // Ekranın solundan çıktıysa yok edilecek listeye ekle
                if (obstacle.transform.position.x < despawnX)
                {
                    obstaclesToRemove.Add(obstacle);
                }
            }
        }
        
        // Ekrandan çıkan engelleri yok et
        foreach (GameObject obstacle in obstaclesToRemove)
        {
            obstacles.Remove(obstacle);
            Destroy(obstacle);
        }
    }
    
    // Zorluğu artır
    private void IncreaseDifficulty()
    {
        // Hızı artır
        obstacleSpeed += speedIncreaseAmount;
        
        // Oluşturma aralığını azalt (minimum limitten az olamaz)
        spawnInterval = Mathf.Max(minSpawnIntervalLimit, spawnInterval - 0.1f);
        
        Debug.Log("Zorluk arttı! Yeni hız: " + obstacleSpeed + ", Yeni aralık: " + spawnInterval);
    }
    
    // Oyun bittiğinde engel oluşturmayı durdur
    public void StopGenerating()
    {
        isGenerating = false;
        
        if (generationCoroutine != null)
        {
            StopCoroutine(generationCoroutine);
            generationCoroutine = null;
        }
        
        Debug.Log("Engel oluşturma durduruldu.");
    }
    
    // Tüm engelleri temizle
    public void ClearAllObstacles()
    {
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        
        obstacles.Clear();
        Debug.Log("Tüm engeller temizlendi.");
    }
} 