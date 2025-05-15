using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Scene yönetimi için

public class GameManager : MonoBehaviour
{
    [Header("Game Time")]
    public float gameTime = 60f;       // Oyun süresi (saniye)
    private float currentTime;         // Kalan süre
    
    [Header("UI References")]
    public Text timeText;              // Süre metni
    public GameObject gameOverPanel;   // Oyun sonu paneli
    public Text gameOverText;          // Oyun sonu mesajı
    
    [Header("Scene Management")]
    public string sceneToLoad = "DenemeSahnesi"; // Yüklenecek sahne adı
    public int sceneIndexToLoad = 0;             // Yüklenecek sahne index
    
    [Header("Audio Settings")]
    public AudioClip gameStartSound;    // Oyun başlangıç sesi
    [Range(0f, 1f)]
    public float startSoundVolume = 0.7f;  // Ses seviyesi
    private AudioSource audioSource;
    
    void Start()
    {
        // Oyunu başlat
        currentTime = gameTime;
        
        // Oyun sonu panelini gizle
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Oyun zamanını normal hızına ayarla
        Time.timeScale = 1;
        
        // Sahne kontrolü - mevcut sahneleri logla
        Debug.Log("Yüklü sahneler:");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log("Sahne " + i + ": " + sceneName);
        }
        
        // AudioSource bileşeni al veya oluştur
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Oyun başlangıç sesini çal
        PlayGameStartSound();
    }
    
    void Update()
    {
        // Süreyi azalt
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            
            // Süre 0'ın altına düşmesin
            if (currentTime < 0)
            {
                currentTime = 0;
                GameOver();
            }
            
            // UI'ı güncelle
            UpdateTimeUI();
        }
    }
    
    // Başlangıç sesini çal
    public void PlayGameStartSound()
    {
        if (gameStartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameStartSound, startSoundVolume);
            Debug.Log("Oyun başlangıç sesi çalınıyor: " + gameStartSound.name);
        }
    }
    
    // Glikoz güçlendirmesi için çağrılır
    public void AddTime(float amount)
    {
        currentTime += amount;
        UpdateTimeUI();
        
        Debug.Log(amount + " saniye eklendi! Yeni süre: " + currentTime);
    }
    
    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    // Oyun bitti fonksiyonu - Obstacle tarafından çağrılır
    public void GameOver()
    {
        // Oyun zamanını durdur
        Time.timeScale = 0;
        
        // Oyun bitti mesajını göster
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverText != null)
        {
            gameOverText.text = "KAYBETTİN!";
        }
        
        Debug.Log("Oyun Bitti! Kaybettin!");
    }
    
    // Oyunu yeniden başlat
    public void RestartGame()
    {
        // Oyun zamanını normale çevir
        Time.timeScale = 1;
        
        // Aktif sahneyi yeniden yükle - bu her zaman çalışır
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Belirli bir sahneyi isim ile yükle
    public void LoadSpecificScene(string sceneName)
    {
        Debug.Log("Sahne yükleniyor (isim): " + sceneName);
        
        // Oyun zamanını normale çevir
        Time.timeScale = 1;
        
        // Sahneyi yükle
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Sahne yükleme hatası: " + e.Message);
            Debug.LogError("Lütfen '" + sceneName + "' sahnesinin Build Settings'de ekli olduğunu kontrol edin!");
            // Alternatif olarak aktif sahneyi yeniden yükle
            RestartGame();
        }
    }
    
    // Belirli bir sahneyi index ile yükle (daha güvenli)
    public void LoadSceneByIndex(int sceneIndex)
    {
        Debug.Log("Sahne yükleniyor (index): " + sceneIndex);
        
        // Oyun zamanını normale çevir
        Time.timeScale = 1;
        
        try
        {
            // Geçerli bir index mi kontrol et
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogError("Geçersiz sahne index: " + sceneIndex);
                Debug.LogError("Build Settings'de " + SceneManager.sceneCountInBuildSettings + " sahne var.");
                // Aktif sahneyi yeniden yükle
                RestartGame();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Sahne yükleme hatası: " + e.Message);
            // Aktif sahneyi yeniden yükle
            RestartGame();
        }
    }
    
    // Aktif sahneyi yeniden yükle
    public void ReloadCurrentScene()
    {
        // Oyun zamanını normale çevir
        Time.timeScale = 1;
        
        // Aktif sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 