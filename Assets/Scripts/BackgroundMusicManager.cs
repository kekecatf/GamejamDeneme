using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;
    
    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
        [Range(0f, 1f)]
        public float volume = 0.5f;
    }
    
    public SceneMusic[] sceneMusicList;
    public bool playMusicOnAwake = true;
    
    private AudioSource audioSource;
    private string currentScene;
    
    private void Awake()
    {
        // Singleton pattern uygulanması
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // AudioSource bileşenini ekle
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Sahne değişikliğini dinle
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Start()
    {
        if (playMusicOnAwake)
        {
            PlayMusicForScene(SceneManager.GetActiveScene().name);
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // Sahne yüklendiğinde çağrılır
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }
    
    // Belirtilen sahne için müziği çal
    public void PlayMusicForScene(string sceneName)
    {
        if (currentScene == sceneName) return;
        
        currentScene = sceneName;
        
        // Sahneye göre müziği bul
        SceneMusic sceneMusic = FindSceneMusic(sceneName);
        
        // Eğer bu sahne için müzik tanımlanmışsa çal
        if (sceneMusic != null && sceneMusic.musicClip != null)
        {
            if (audioSource.clip != sceneMusic.musicClip)
            {
                audioSource.clip = sceneMusic.musicClip;
                audioSource.volume = sceneMusic.volume;
                audioSource.Play();
                Debug.Log("Scene müziği çalınıyor: " + sceneName + " - " + sceneMusic.musicClip.name);
            }
        }
        else
        {
            // Bu sahne için müzik yoksa mevcut müziği durdur
            audioSource.Stop();
            Debug.Log("Bu sahne için müzik bulunamadı: " + sceneName);
        }
    }
    
    // Sahne adına göre müzik ayarlarını bul
    private SceneMusic FindSceneMusic(string sceneName)
    {
        foreach (SceneMusic sceneMusic in sceneMusicList)
        {
            if (sceneMusic.sceneName == sceneName)
            {
                return sceneMusic;
            }
        }
        return null;
    }
    
    // Geçerli müzik sesini ayarla
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