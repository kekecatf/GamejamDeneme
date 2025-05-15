using UnityEngine;

public class BrainSpawner : MonoBehaviour
{
    public GameObject brainPrefab; // Brain prefabını buraya sürükleyin
    public float spawnDelay = 5f; // Brain'in sahneye eklenme gecikmesi (saniye)
    
    private void Start()
    {
        // Belirtilen gecikme sonrası Brain'i spawn et
        Invoke("SpawnBrain", spawnDelay);
    }
    
    private void SpawnBrain()
    {
        // Brain prefabını belirtilen konumda oluştur
        Instantiate(brainPrefab);
    }
} 