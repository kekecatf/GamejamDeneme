using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject sceneControllerPrefab;
    public GameObject gameMusicManagerPrefab;
    
    void Awake()
    {
        // Sahne kontrolcüsünü oluştur
        if (SceneController.Instance == null && sceneControllerPrefab != null)
        {
            Instantiate(sceneControllerPrefab);
        }
        
        // Müzik yöneticisini oluştur
        if (GameMusicManager.Instance == null && gameMusicManagerPrefab != null)
        {
            Instantiate(gameMusicManagerPrefab);
        }
    }
} 