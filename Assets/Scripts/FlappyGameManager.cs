using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FlappyGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float initialDelay = 2f;     // Oyun başlangıç gecikmesi
    
    [Header("UI Elements")]
    public Text scoreText;              // Skor yazısı
    public GameObject gameOverPanel;    // Oyun sonu paneli
    public Button restartButton;        // Yeniden başlat butonu
    public Button mainMenuButton;       // Ana menü butonu
    
    [Header("Component References")]
    public FlappyPlayerController player; // Oyuncu referansı
    public ObstacleGenerator obstacleGenerator; // Engel üretici referansı
    
    [Header("Audio Settings")]
    public AudioClip gameStartSound;    // Oyun başlangıç sesi
    [Range(0f, 1f)]
    public float startSoundVolume = 0.7f;  // Ses seviyesi
    private AudioSource audioSource;
    
    [Header("Debug")]
    public bool showDebugInfo = true;   // Debug bilgisi göster/gizle
    
    private int score = 0;              // Mevcut skor
    private bool isGameOver = false;    // Oyun bitti mi?
    private bool hasGameStarted = false; // Oyun başladı mı?
    
    void Start()
    {
        // Oyun başında UI'ı ayarla
        UpdateScoreUI();
        
        // Oyun sonu panelini gizle
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("GameOverPanel atanmamış! Lütfen Inspector'da ayarlayın.");
        }
        
        // Butonlara event listener'ları ekle
        SetupButtons();
        
        // AudioSource bileşeni al veya oluştur
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Oyunu gecikme ile başlat
        Invoke("StartGame", initialDelay);
    }
    
    // Butonları ayarla
    private void SetupButtons()
    {
        // Restart butonunu ayarla
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners(); // Önceki listener'ları temizle
            restartButton.onClick.AddListener(RestartGame);
            Debug.Log("Restart butonuna listener eklendi");
        }
        else
        {
            Debug.LogError("Restart butonu atanmamış!");
        }
        
        // Ana menü butonunu ayarla
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners(); // Önceki listener'ları temizle
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            Debug.Log("Ana menü butonuna listener eklendi");
        }
        else
        {
            Debug.LogError("Ana menü butonu atanmamış!");
        }
    }
    
    // Oyunu başlat
    private void StartGame()
    {
        // Oyun başlangıç sesini çal
        PlayGameStartSound();
        
        // Engel üretmeyi başlat
        if (obstacleGenerator != null)
        {
            obstacleGenerator.StartGenerating();
        }
        else
        {
            Debug.LogError("ObstacleGenerator atanmamış! Lütfen Inspector'da ayarlayın.");
        }
        
        hasGameStarted = true;
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
    
    void Update()
    {
        // Skor artırma işlemini burada yapabilirsiniz
        // Örnek: 1 saniyede 1 puan artırma
        if (hasGameStarted && !isGameOver)
        {
            score += 1;
            UpdateScoreUI();
        }
        
        // Debug bilgisi göster
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F1))
        {
            DebugGameState();
        }
        
        // Test için Escape tuşu ile ana menüye dön
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
        
        // Test için R tuşu ile yeniden başlat
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    // Skoru artır (engelden geçince çağrılabilir)
    public void AddScore(int points = 1)
    {
        if (!isGameOver)
        {
            score += points;
            UpdateScoreUI();
        }
    }
    
    // Skor metnini güncelle
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Skor: " + score;
        }
    }
    
    // Oyun bitti işlemi
    public void GameOver()
    {
        Debug.Log("GameOver metodu çağrıldı - ÖNCESİ: isGameOver=" + isGameOver);
        
        if (isGameOver) return;
        
        isGameOver = true;
        
        // Engel üretimini durdur
        if (obstacleGenerator != null)
        {
            obstacleGenerator.StopGenerating();
        }
        
        // Oyun sonu panelini göster
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("Game Over Panel aktif edildi!");
            
            // Oyun sonunda butonların çalıştığından emin olmak için yeniden ayarla
            SetupButtons();
        }
        else
        {
            Debug.LogError("GameOverPanel NULL! Panel gösterilemiyor.");
        }
        
        Debug.Log("Oyun bitti! Skorunuz: " + score);
    }
    
    // Debug bilgisi göster
    private void DebugGameState()
    {
        Debug.Log("===== OYUN DURUMU =====");
        Debug.Log("Oyun başladı mı: " + hasGameStarted);
        Debug.Log("Oyun bitti mi: " + isGameOver);
        Debug.Log("Skor: " + score);
        Debug.Log("GameOverPanel atanmış mı: " + (gameOverPanel != null));
        Debug.Log("GameOverPanel aktif mi: " + (gameOverPanel != null ? gameOverPanel.activeSelf : false));
        Debug.Log("ObstacleGenerator atanmış mı: " + (obstacleGenerator != null));
        Debug.Log("Player atanmış mı: " + (player != null));
        Debug.Log("Restart butonu atanmış mı: " + (restartButton != null));
        Debug.Log("Ana menü butonu atanmış mı: " + (mainMenuButton != null));
        Debug.Log("=======================");
    }
    
    // Oyunu yeniden başlat
    public void RestartGame()
    {
        Debug.Log("Oyun yeniden başlatılıyor...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Ana menüye dön
    public void ReturnToMainMenu()
    {
        Debug.Log("Ana menüye dönülüyor...");
        SceneManager.LoadScene("AnaMenu");
    }
    
    // Editör üzerinden panel referansını test et
    public void TestGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(!gameOverPanel.activeSelf);
            Debug.Log("Test: Game Over Panel durumu değiştirildi: " + gameOverPanel.activeSelf);
        }
        else
        {
            Debug.LogError("Game Over Panel NULL!");
        }
    }
} 