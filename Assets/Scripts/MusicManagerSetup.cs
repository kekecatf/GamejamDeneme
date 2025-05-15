using UnityEngine;

public class MusicManagerSetup : MonoBehaviour
{
    [Header("Music Prefab")]
    public GameObject musicManagerPrefab;
    
    [Header("Background Musics")]
    public AudioClip menuMusic;
    public AudioClip dialogueMusic;
    public AudioClip parkourMusic;
    
    [Range(0f, 1f)]
    public float menuVolume = 0.5f;
    [Range(0f, 1f)]
    public float dialogueVolume = 0.5f;
    [Range(0f, 1f)]
    public float parkourVolume = 0.5f;
    
    void Awake()
    {
        // Müzik yöneticisi var mı kontrol et
        if (BackgroundMusicManager.Instance == null && musicManagerPrefab != null)
        {
            // Prefab'dan müzik yöneticisi oluştur
            GameObject musicManagerObj = Instantiate(musicManagerPrefab);
            BackgroundMusicManager musicManager = musicManagerObj.GetComponent<BackgroundMusicManager>();
            
            if (musicManager != null)
            {
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
            }
        }
    }
} 