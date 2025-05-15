using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager Instance;
    
    [Header("Müzik Dosyaları")]
    public AudioClip menuMusic;
    public AudioClip dialogueMusic;
    public AudioClip parkourMusic;
    
    [Header("Ses Ayarları")]
    [Range(0f, 1f)]
    public float menuVolume = 0.5f;
    [Range(0f, 1f)]
    public float dialogueVolume = 0.5f;
    [Range(0f, 1f)]
    public float parkourVolume = 0.5f;
    
    private AudioSource audioSource;
    private string currentScene;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // AudioSource ekle
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            
            // Sahne değişikliklerini dinle
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Başlangıçta aktif sahnenin müziğini çal
        PlayMusicForCurrentScene();
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sahne değiştiğinde müziği güncelle
        PlayMusicForScene(scene.name);
    }
    
    // Mevcut sahnenin müziğini çal
    public void PlayMusicForCurrentScene()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }
    
    // Belirli bir sahnenin müziğini çal
    public void PlayMusicForScene(string sceneName)
    {
        // Aynı sahne ise tekrar çalma
        if (currentScene == sceneName) return;
        
        currentScene = sceneName;
        AudioClip musicToPlay = null;
        float volume = 0.5f;
        
        // Sahne adına göre müziği belirle
        switch (sceneName)
        {
            case "AnaMenu":
                musicToPlay = menuMusic;
                volume = menuVolume;
                break;
                
            case "DiyalogSahnesi":
                musicToPlay = dialogueMusic;
                volume = dialogueVolume;
                break;
                
            case "Parkur":
            case "Ziplama":
                musicToPlay = parkourMusic;
                volume = parkourVolume;
                break;
        }
        
        // Müziği değiştir ve çal
        if (musicToPlay != null)
        {
            if (audioSource.clip != musicToPlay)
            {
                audioSource.clip = musicToPlay;
                audioSource.volume = volume;
                audioSource.Play();
                Debug.Log(sceneName + " için müzik çalınıyor: " + musicToPlay.name);
            }
        }
        else
        {
            audioSource.Stop();
            Debug.Log(sceneName + " için müzik bulunamadı.");
        }
    }
    
    // Ses seviyesini ayarla
    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }
    
    // Geçerli müziği durdur
    public void StopMusic()
    {
        audioSource.Stop();
    }
    
    // Geçerli müziği duraklat
    public void PauseMusic()
    {
        audioSource.Pause();
    }
    
    // Duraklatılmış müziği devam ettir
    public void ResumeMusic()
    {
        audioSource.UnPause();
    }
} 