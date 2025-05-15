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
    public string targetSceneName = "GameOver";
    
    private bool isMoving = false;
    
    void Start()
    {
        // Set the initial position
        transform.position = startPosition;
        
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
        if (other.CompareTag("Player"))
        {
            // Load the target scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Load the target scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
} 