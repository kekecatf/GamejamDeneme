using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrainBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    public float fallSpeed = 3f;
    public float delayBeforeFalling = 2f;
    public Vector2 startPosition = new Vector2(0f, 6f);
    
    [Header("Scene Transition")]
    public string targetSceneName = "DiyalogSahnesi";
    public float transitionDelay = 0.5f; // Geçiş öncesi kısa bir bekleme süresi
    
    [Header("Audio")]
    public AudioClip collisionSound; // Çarpışma sesi
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;
    
    private bool isMoving = false;
    private bool hasCollided = false; // Çarpışma kontrolü
    private AudioSource audioSource;
    
    void Start()
    {
        // Set the initial position
        transform.position = startPosition;
        
        // Ses kaynağını ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collisionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Start the countdown before falling
        StartCoroutine(StartFallingAfterDelay());
    }
    
    void Update()
    {
        if (isMoving)
        {
            // Move downward at the specified speed
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        }
    }
    
    private IEnumerator StartFallingAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeFalling);
        
        // Start falling
        isMoving = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided with player
        if (other.CompareTag("Player") && !hasCollided)
        {
            PlayerCollision();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with player
        if (collision.gameObject.CompareTag("Player") && !hasCollided)
        {
            PlayerCollision();
        }
    }
    
    // Oyuncu ile çarpışma olduğunda çağrılır
    private void PlayerCollision()
    {
        hasCollided = true;
        
        // Çarpışma sesini çal
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound, soundVolume);
        }
        
        // Yeni SceneController'ı kullan
        if (SceneController.Instance != null)
        {
            // SceneController ile diyalog sahnesine dön (otomatik olarak diyalog state arttırılır)
            StartCoroutine(ReturnToDialogueWithDelay());
        }
        else
        {
            // Eski yöntem - DiyalogGameManager kullan
            if (DiyalogGameManager.Instance != null)
            {
                string previousNode = DiyalogGameManager.Instance.currentNodeID;
                DiyalogGameManager.Instance.AdvanceDialogueState();
                Debug.Log("Diyalog durumu arttırıldı: " + DiyalogGameManager.Instance.dialogueState);
                Debug.Log("Diyalog ağacı: " + previousNode + " → " + DiyalogGameManager.Instance.currentNodeID);
            }
            
            // Kısa bir gecikme ile sahne geçişi yap (eski yöntem)
            StartCoroutine(LoadSceneWithDelay());
        }
    }
    
    // SceneController ile diyalog sahnesine dönme
    private IEnumerator ReturnToDialogueWithDelay()
    {
        yield return new WaitForSeconds(transitionDelay);
        
        // SceneController ile diyalog sahnesine dön
        SceneController.Instance.ReturnToDialogueScene();
    }
    
    // Kısa bir gecikme ile sahne geçişi (eski yöntem)
    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(transitionDelay);
        
        // DiyalogSahnesi'ne kaldığı yerden devam etmek için geç
        SceneManager.LoadScene(targetSceneName);
    }
} 