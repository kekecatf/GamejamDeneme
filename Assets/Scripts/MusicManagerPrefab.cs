using UnityEngine;

// Ana Menü sahnesinde Unity Editor'de kullanmak için bu script
public class MusicManagerPrefab : MonoBehaviour
{
    [Header("Müzik Ayarları")]
    public AudioClip menuMusic;
    public AudioClip dialogueMusic;
    public AudioClip parkourMusic;
    
    [Range(0f, 1f)]
    public float menuVolume = 0.5f;
    [Range(0f, 1f)]
    public float dialogueVolume = 0.5f;
    [Range(0f, 1f)]
    public float parkourVolume = 0.5f;
    
    void Start()
    {
        // BackgroundMusicManager zaten varsa, bunu kur
        if (BackgroundMusicManager.Instance != null)
        {
            SetupMusicManager();
        }
    }
    
    // BackgroundMusicManager oluşturulursa bunu çağırabilirsiniz
    public void SetupMusicManager()
    {
        var musicManager = BackgroundMusicManager.Instance;
        
        // Tüm sahneleri ve müzikleri ayarla
        musicManager.sceneMusicList = new BackgroundMusicManager.SceneMusic[]
        {
            new BackgroundMusicManager.SceneMusic
            {
                sceneName = "AnaMenu",
                musicClip = menuMusic,
                volume = menuVolume
            },
            new BackgroundMusicManager.SceneMusic
            {
                sceneName = "DiyalogSahnesi",
                musicClip = dialogueMusic,
                volume = dialogueVolume
            },
            new BackgroundMusicManager.SceneMusic
            {
                sceneName = "Parkur",
                musicClip = parkourMusic,
                volume = parkourVolume
            },
            new BackgroundMusicManager.SceneMusic
            {
                sceneName = "Ziplama",
                musicClip = parkourMusic, // Zıplama sahnesinde de parkur müziğini kullan
                volume = parkourVolume
            }
        };
        
        // Mevcut sahnenin müziğini başlat
        musicManager.PlayMusicForScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
} 